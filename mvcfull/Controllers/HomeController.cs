// الكنترولر الرئيسي - الصفحة الرئيسية وصفحات من نحن واتصل بنا
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;

namespace mvc_full.Controllers
{
    // الكنترولر الرئيسي للصفحة الرئيسية
    // يعرض المطاعم المميزة وصفحات "من نحن" و"اتصل بنا"
    public class HomeController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // الصفحة الرئيسية - تعرض أول 6 مطاعم
        // TODO: إضافة نظام ترتيب حسب التقييم
        public ActionResult Index()
        {
            // Get featured restaurants
            var restoranlar = db.Restoranlar.Take(6).ToList();
            return View(restoranlar);
        }

        // صفحة من نحن
        public ActionResult About()
        {
            ViewBag.Message = "TrendYol Yemek - En lezzetli yemekleri kapınıza getiriyoruz!";
            return View();
        }

        // صفحة اتصل بنا
        public ActionResult Contact()
        {
            ViewBag.Message = "Bizimle iletişime geçin.";
            return View();
        }

        // تنظيف الموارد
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