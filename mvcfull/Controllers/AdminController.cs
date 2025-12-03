using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
