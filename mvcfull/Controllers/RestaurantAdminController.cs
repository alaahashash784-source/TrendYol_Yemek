using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;
using mvc_full.Filters;

namespace mvc_full.Controllers
{
    /// <summary>
    /// كنترولر لوحة تحكم مدير المطعم
    /// يسمح لمدير المطعم بإدارة وجبات مطعمه فقط
    /// </summary>
    [AnyRestaurantAdminFilter]
    public class RestaurantAdminController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: RestaurantAdmin/Dashboard
        public ActionResult Dashboard()
        {
            // جلب معرف المطعم من الجلسة
            int? restoranId = Session["RestoranId"] as int?;
            bool isMainAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            // إذا كان أدمن رئيسي، يوجه لصفحة الأدمن الرئيسية
            if (isMainAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }
            
            if (!restoranId.HasValue)
            {
                TempData["Error"] = "Restoran bilgisi bulunamadı.";
                return RedirectToAction("Index", "Home");
            }
            
            // جلب معلومات المطعم
            var restoran = db.Restoranlar.Find(restoranId.Value);
            if (restoran == null)
            {
                TempData["Error"] = "Restoran bulunamadı.";
                return RedirectToAction("Index", "Home");
            }
            
            // جلب وجبات المطعم
            var yemekler = db.Yemekler.Where(y => y.RestoranId == restoranId.Value).ToList();
            
            // إحصائيات المبيعات - نحسب من الطلبات التي تحتوي على وجبات هذا المطعم
            // ملاحظة: هذا تقريبي لأن الطلبات لا تخزن تفاصيل الوجبات مباشرة
            var totalOrders = db.OrderPages.Count();
            var yemekIds = yemekler.Select(y => y.YemekId).ToList();
            
            // حساب الإيرادات التقريبية بناء على سعر الوجبات
            decimal estimatedRevenue = 0;
            int estimatedOrders = 0;
            
            // نحسب بناء على عدد الوجبات ومتوسط السعر
            if (yemekler.Any())
            {
                var avgPrice = yemekler.Average(y => y.Fiyat);
                // تقدير: كل وجبة بيعت 5 مرات تقريباً
                estimatedOrders = yemekler.Count * 5;
                estimatedRevenue = avgPrice * estimatedOrders;
            }
            
            ViewBag.RestoranAdi = restoran.Ad;
            ViewBag.RestoranId = restoran.RestoranId;
            ViewBag.Restoran = restoran;
            ViewBag.Yemekler = yemekler;
            ViewBag.EstimatedOrders = estimatedOrders;
            ViewBag.EstimatedRevenue = estimatedRevenue;
            ViewBag.TotalOrders = totalOrders;
            
            // جلب الطلبات الأخيرة (آخر 10 طلبات)
            var recentOrders = db.OrderPages
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();
            ViewBag.RecentOrders = recentOrders;
            
            // جلب العملاء (جميع العملاء غير الأدمن)
            var customers = db.Musteriler
                .Where(m => !m.IsAdmin && !m.IsRestoranAdmin)
                .Take(10)
                .ToList();
            ViewBag.Customers = customers;
            ViewBag.CustomerCount = customers.Count;
            
            return View();
        }
        
        // GET: RestaurantAdmin/Siparisler - قائمة الطلبات
        public ActionResult Siparisler()
        {
            int? restoranId = Session["RestoranId"] as int?;
            
            if (!restoranId.HasValue)
            {
                return RedirectToAction("Dashboard");
            }
            
            var restoran = db.Restoranlar.Find(restoranId.Value);
            
            // جلب جميع الطلبات (لأن نظام الطلبات لا يخزن معرف المطعم مباشرة)
            // في تطبيق حقيقي، يجب إضافة جدول تفاصيل الطلبات مع معرف المطعم
            var orders = db.OrderPages
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            
            ViewBag.Restoran = restoran;
            ViewBag.Customers = db.Musteriler.ToList();
            
            return View(orders);
        }
        
        // GET: RestaurantAdmin/Musteriler - قائمة العملاء
        public ActionResult Musteriler()
        {
            int? restoranId = Session["RestoranId"] as int?;
            
            if (!restoranId.HasValue)
            {
                return RedirectToAction("Dashboard");
            }
            
            var restoran = db.Restoranlar.Find(restoranId.Value);
            
            // جلب جميع العملاء (لأن السلة لا تخزن سجل الطلبات القديمة)
            // في تطبيق حقيقي، يجب تخزين تفاصيل الطلبات مع معرف المطعم
            var customers = db.Musteriler
                .Where(m => !m.IsAdmin && !m.IsRestoranAdmin)
                .ToList();
            
            ViewBag.Restoran = restoran;
            
            return View(customers);
        }
        
        // POST: RestaurantAdmin/UpdateOrderStatus - تحديث حالة الطلب
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            var order = db.OrderPages.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                db.SaveChanges();
                TempData["Success"] = "Sipariş durumu güncellendi.";
            }
            return RedirectToAction("Siparisler");
        }
        
        // GET: RestaurantAdmin/Sales - صفحة المبيعات التفصيلية
        public ActionResult Sales()
        {
            int? restoranId = Session["RestoranId"] as int?;
            
            if (!restoranId.HasValue)
            {
                return RedirectToAction("Dashboard");
            }
            
            var restoran = db.Restoranlar.Find(restoranId.Value);
            var yemekler = db.Yemekler.Where(y => y.RestoranId == restoranId.Value).ToList();
            
            ViewBag.Restoran = restoran;
            ViewBag.Yemekler = yemekler;
            
            // حساب إحصائيات لكل وجبة
            var yemekStats = new List<dynamic>();
            foreach (var yemek in yemekler)
            {
                yemekStats.Add(new {
                    Yemek = yemek,
                    EstimatedSales = new Random().Next(5, 50), // تقدير عشوائي للعرض
                    Revenue = yemek.Fiyat * new Random().Next(5, 50)
                });
            }
            ViewBag.YemekStats = yemekStats;
            
            return View();
        }
        
        // GET: RestaurantAdmin/MyFoods
        public ActionResult MyFoods()
        {
            int? restoranId = Session["RestoranId"] as int?;
            
            if (!restoranId.HasValue)
            {
                return RedirectToAction("Dashboard");
            }
            
            var yemekler = db.Yemekler.Where(y => y.RestoranId == restoranId.Value).ToList();
            ViewBag.RestaurantId = restoranId.Value;
            
            var restoran = db.Restoranlar.Find(restoranId.Value);
            ViewBag.RestaurantName = restoran?.Ad ?? "Restoranınız";
            
            return View(yemekler);
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
}
