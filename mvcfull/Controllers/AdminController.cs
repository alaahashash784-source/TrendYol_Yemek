using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;
using mvc_full.Filters;

namespace mvc_full.Controllers
{
    // Admin controller - requires authentication
    [CustomAuthorizationFilter]
    public class AdminController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            var stats = new AdminDashboardViewModel
            {
                TotalRestaurants = db.Restoranlar.Count(),
                TotalFoods = db.Yemekler.Count(),
                TotalCustomers = db.Musteriler.Count(),
                TotalOrders = db.OrderPages.Count(),
                // Fixed: Safe date comparison for SQL Server
                TodayOrders = db.OrderPages.Count(o => 
                    DbFunctions.TruncateTime(o.OrderDate) == DbFunctions.TruncateTime(DateTime.Today)),
                TotalRevenue = db.OrderPages.Any() 
                    ? db.OrderPages.Sum(o => (decimal?)o.TotalPrice) ?? 0 
                    : 0
            };

            return View(stats);
        }

        // GET: Admin/Customers
        public ActionResult Customers()
        {
            var customers = db.Musteriler.OrderByDescending(m => m.KayitTarihi).ToList();
            return View(customers);
        }

        // GET: Admin/Orders
        public ActionResult Orders()
        {
            var orders = db.OrderPages.OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        // GET: Admin/OrderDetails/5
        public ActionResult OrderDetails(int id)
        {
            var order = db.OrderPages.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = db.OrderPages.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                db.SaveChanges();
                TempData["Message"] = "Siparis durumu guncellendi";
            }

            return RedirectToAction("Orders");
        }

        // GET: Admin/Reports
        public ActionResult Reports()
        {
            // Get all orders and process in memory to avoid LINQ to Entities issues
            var allOrders = db.OrderPages.ToList();
            var thirtyDaysAgo = DateTime.Today.AddDays(-30);
            
            var report = new ReportViewModel
            {
                // Process daily orders in memory
                DailyOrders = allOrders
                    .Where(o => o.OrderDate.Date >= thirtyDaysAgo)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new DailyOrderStat
                    {
                        Date = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalPrice)
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList(),

                // Get top restaurants by food count
                TopRestaurants = db.Restoranlar
                    .Select(r => new RestaurantStat
                    {
                        RestaurantName = r.Ad,
                        FoodCount = r.Yemekler.Count,
                        TotalOrders = 0
                    })
                    .OrderByDescending(r => r.FoodCount)
                    .Take(10)
                    .ToList(),

                // Get top customers - process in memory
                TopCustomers = db.Musteriler
                    .Select(m => new
                    {
                        FullName = m.Ad + " " + m.Soyad,
                        Orders = m.Siparisler
                    })
                    .ToList()
                    .Select(m => new CustomerStat
                    {
                        CustomerName = m.FullName,
                        OrderCount = m.Orders != null ? m.Orders.Count() : 0,
                        TotalSpent = m.Orders != null && m.Orders.Any() 
                            ? m.Orders.Sum(o => o.TotalPrice) 
                            : 0
                    })
                    .OrderByDescending(c => c.TotalSpent)
                    .Take(10)
                    .ToList()
            };

            return View(report);
        }

        // GET: Admin/CustomerDetails
        public ActionResult CustomerDetails(int id)
        {
            var customer = db.Musteriler.Find(id);
            if (customer == null)
            {
                TempData["Error"] = "Musteri bulunamadi";
                return RedirectToAction("Customers");
            }
            return View(customer);
        }

        // GET: Admin/DeleteCustomer
        public ActionResult DeleteCustomer(int id)
        {
            var customer = db.Musteriler.Find(id);
            if (customer == null)
            {
                TempData["Error"] = "Musteri bulunamadi";
                return RedirectToAction("Customers");
            }
            
            if (customer.IsAdmin)
            {
                TempData["Error"] = "Admin kullanici silinemez";
                return RedirectToAction("Customers");
            }

            // First delete all orders for this customer
            var customerOrders = db.OrderPages.Where(o => o.MusteriId == id).ToList();
            if (customerOrders.Any())
            {
                db.OrderPages.RemoveRange(customerOrders);
            }

            db.Musteriler.Remove(customer);
            db.SaveChanges();
            TempData["Message"] = "Musteri ve siparisleri basariyla silindi";
            return RedirectToAction("Customers");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Admin/RestaurantAdmins - إدارة مديري المطاعم
        [AdminAuthorizationFilter]
        public ActionResult RestaurantAdmins()
        {
            var restoranAdmins = db.Musteriler
                .Where(m => m.IsRestoranAdmin)
                .ToList();
            
            ViewBag.Restoranlar = new SelectList(db.Restoranlar.ToList(), "RestoranId", "Ad");
            return View(restoranAdmins);
        }
        
        // GET: Admin/CreateRestaurantAdmin
        [AdminAuthorizationFilter]
        public ActionResult CreateRestaurantAdmin()
        {
            ViewBag.Restoranlar = new SelectList(db.Restoranlar.ToList(), "RestoranId", "Ad");
            return View();
        }
        
        // POST: Admin/CreateRestaurantAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult CreateRestaurantAdmin(Musteri model, int restoranId)
        {
            if (ModelState.IsValid)
            {
                // التحقق من عدم وجود إيميل مكرر
                if (db.Musteriler.Any(m => m.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor");
                    ViewBag.Restoranlar = new SelectList(db.Restoranlar.ToList(), "RestoranId", "Ad", restoranId);
                    return View(model);
                }
                
                // تشفير كلمة المرور
                model.Sifre = Helpers.SecurityHelper.HashPassword(model.Sifre);
                model.IsRestoranAdmin = true;
                model.RestoranId = restoranId;
                model.IsAdmin = false;
                model.KayitTarihi = DateTime.Now;
                
                db.Musteriler.Add(model);
                db.SaveChanges();
                
                TempData["Message"] = "Restoran yöneticisi başarıyla oluşturuldu";
                return RedirectToAction("RestaurantAdmins");
            }
            
            ViewBag.Restoranlar = new SelectList(db.Restoranlar.ToList(), "RestoranId", "Ad", restoranId);
            return View(model);
        }
        
        // POST: Admin/SetRestaurantAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult SetRestaurantAdmin(int musteriId, int restoranId)
        {
            var musteri = db.Musteriler.Find(musteriId);
            if (musteri != null)
            {
                musteri.IsRestoranAdmin = true;
                musteri.RestoranId = restoranId;
                db.SaveChanges();
                TempData["Message"] = $"{musteri.TamAd} artık restoran yöneticisi";
            }
            return RedirectToAction("RestaurantAdmins");
        }
        
        // POST: Admin/RemoveRestaurantAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult RemoveRestaurantAdmin(int musteriId)
        {
            var musteri = db.Musteriler.Find(musteriId);
            if (musteri != null)
            {
                musteri.IsRestoranAdmin = false;
                musteri.RestoranId = null;
                db.SaveChanges();
                TempData["Message"] = $"{musteri.TamAd} artık restoran yöneticisi değil";
            }
            return RedirectToAction("RestaurantAdmins");
        }

        // GET: Admin/ExportOrdersCsv - Export orders to CSV
        public ActionResult ExportOrdersCsv()
        {
            var orders = db.OrderPages.OrderByDescending(o => o.OrderDate).ToList();
            
            var csv = new StringBuilder();
            // Add UTF-8 BOM for Excel compatibility
            csv.Append('\uFEFF');
            // Header
            csv.AppendLine("Siparis No,Musteri ID,Toplam Tutar (TL),Teslimat Adresi,Odeme Yontemi,Siparis Tarihi,Siparis Durumu");
            
            foreach (var order in orders)
            {
                csv.AppendLine($"{order.OrderId},{order.MusteriId},{order.TotalPrice:F2},{EscapeCsvField(order.DeliveryAddress)},{EscapeCsvField(order.PaymentMethod)},{order.OrderDate:yyyy-MM-dd HH:mm},{EscapeCsvField(order.OrderStatus)}");
            }

            var fileName = $"Siparisler_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }

        // GET: Admin/ExportCustomersCsv - Export customers to CSV
        public ActionResult ExportCustomersCsv()
        {
            var customers = db.Musteriler.OrderByDescending(m => m.KayitTarihi).ToList();
            
            var csv = new StringBuilder();
            // Add UTF-8 BOM for Excel compatibility
            csv.Append('\uFEFF');
            // Header
            csv.AppendLine("Musteri ID,Ad,Soyad,Email,Telefon,Adres,Kayit Tarihi,Admin Mi");
            
            foreach (var customer in customers)
            {
                csv.AppendLine($"{customer.MusteriId},{EscapeCsvField(customer.Ad)},{EscapeCsvField(customer.Soyad)},{EscapeCsvField(customer.Email)},{EscapeCsvField(customer.Telefon)},{EscapeCsvField(customer.Adres)},{customer.KayitTarihi:yyyy-MM-dd HH:mm},{(customer.IsAdmin ? "Evet" : "Hayir")}");
            }

            var fileName = $"Musteriler_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }

        // GET: Admin/ExportReportsCsv - Export daily reports to CSV
        public ActionResult ExportReportsCsv()
        {
            var allOrders = db.OrderPages.ToList();
            var thirtyDaysAgo = DateTime.Today.AddDays(-30);
            
            var dailyStats = allOrders
                .Where(o => o.OrderDate.Date >= thirtyDaysAgo)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalPrice)
                })
                .OrderByDescending(d => d.Date)
                .ToList();
            
            var csv = new StringBuilder();
            // Add UTF-8 BOM for Excel compatibility
            csv.Append('\uFEFF');
            // Header
            csv.AppendLine("Tarih,Siparis Sayisi,Toplam Gelir (TL)");
            
            foreach (var stat in dailyStats)
            {
                csv.AppendLine($"{stat.Date:yyyy-MM-dd},{stat.OrderCount},{stat.TotalRevenue:F2}");
            }

            var fileName = $"Gunluk_Rapor_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }

        // Helper method to escape CSV fields
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
            
            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }
        
        // إنشاء مديري المطاعم - بدون تسجيل دخول مطلوب
        [AllowAnonymous]
        public ActionResult CreateRestaurantAdmins()
        {
            var results = new List<string>();
            string defaultPassword = "admin123";
            
            var adminsToCreate = new[]
            {
                new { RestoranId = 1, Email = "kebap@admin.com", Ad = "Kebap" },
                new { RestoranId = 2, Email = "pizza@admin.com", Ad = "Pizza" },
                new { RestoranId = 3, Email = "burger@admin.com", Ad = "Burger" },
                new { RestoranId = 4, Email = "deniz@admin.com", Ad = "Deniz" },
                new { RestoranId = 5, Email = "asya@admin.com", Ad = "Asya" },
                new { RestoranId = 6, Email = "tatli@admin.com", Ad = "Tatli" }
            };
            
            foreach (var admin in adminsToCreate)
            {
                var existing = db.Musteriler.FirstOrDefault(m => m.Email == admin.Email);
                
                string hashedPassword = Helpers.SecurityHelper.HashPassword(defaultPassword);
                
                if (existing != null)
                {
                    existing.Sifre = hashedPassword;
                    existing.IsRestoranAdmin = true;
                    existing.RestoranId = admin.RestoranId;
                    results.Add("UPDATED: " + admin.Email);
                }
                else
                {
                    var newAdmin = new Musteri
                    {
                        Ad = admin.Ad,
                        Soyad = "Yönetici",
                        Email = admin.Email,
                        Sifre = hashedPassword,
                        KayitTarihi = DateTime.Now,
                        IsAdmin = false,
                        IsRestoranAdmin = true,
                        RestoranId = admin.RestoranId
                    };
                    db.Musteriler.Add(newAdmin);
                    results.Add("CREATED: " + admin.Email + " / " + defaultPassword);
                }
            }
            
            db.SaveChanges();
            
            ViewBag.Results = results;
            ViewBag.Password = defaultPassword;
            return View();
        }
    }

    // ViewModels for Admin - moved to separate location would be better practice
    public class AdminDashboardViewModel
    {
        public int TotalRestaurants { get; set; }
        public int TotalFoods { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ReportViewModel
    {
        public List<DailyOrderStat> DailyOrders { get; set; } = new List<DailyOrderStat>();
        public List<RestaurantStat> TopRestaurants { get; set; } = new List<RestaurantStat>();
        public List<CustomerStat> TopCustomers { get; set; } = new List<CustomerStat>();
    }

    public class DailyOrderStat
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RestaurantStat
    {
        public string RestaurantName { get; set; }
        public int FoodCount { get; set; }
        public int TotalOrders { get; set; }
    }

    public class CustomerStat
    {
        public string CustomerName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
