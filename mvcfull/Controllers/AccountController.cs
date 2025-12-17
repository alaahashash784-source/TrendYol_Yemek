
// ملف: AccountController.cs
// الغرض: إدارة حسابات المستخدمين (تسجيل، دخول، خروج، ملف شخصي)
// الشرح: هذا الكنترولر يتعامل مع كل عمليات المصادقة والتفويض

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using mvc_full.Models;
using mvc_full.Models.ViewModels;
using mvc_full.Helpers;

namespace mvc_full.Controllers
{
    // كنترولر الحسابات - تسجيل الدخول والخروج والملف الشخصي
    public class AccountController : Controller
    {
        
        // الاتصال بقاعدة البيانات
       
        private ABCDbContext db = new ABCDbContext();

        
        // صفحة التسجيل - عرض النموذج (GET)
        // الرابط: /Account/Register
        
        public ActionResult Register()
        {
            // إذا مسجل دخول بالفعل = توجيه للرئيسية
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        
        // معالجة التسجيل - استلام البيانات (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]  // حماية من هجمات CSRF
        public ActionResult Register(RegisterViewModel model)
        {
            // التحقق من صحة البيانات
            if (ModelState.IsValid)
            {
                // تحويل الإيميل لحروف صغيرة للمقارنة
                string email = model.Email?.ToLowerInvariant();
                
                // التحقق: هل الإيميل مستخدم من قبل؟
                var user = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);
                if (user != null)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullaniliyor");
                    return View(model);
                }

                // إنشاء مستخدم جديد مع تشفير كلمة المرور
                var musteri = new Musteri
                {
                    Ad = model.Ad?.Trim(),
                    Soyad = model.Soyad?.Trim(),
                    Email = email,
                    Telefon = model.Telefon?.Trim(),
                    Adres = model.Adres?.Trim(),
                    Sifre = SecurityHelper.HashPassword(model.Sifre),  // تشفير!
                    KayitTarihi = DateTime.Now
                };

                // حفظ في قاعدة البيانات
                db.Musteriler.Add(musteri);
                db.SaveChanges();

                // تسجيل الدخول تلقائياً بعد التسجيل
                Session["MusteriId"] = musteri.MusteriId;
                Session["MusteriAd"] = musteri.TamAd;

                TempData["Message"] = "Kayit basarili! Hos geldiniz.";
                return RedirectToAction("Index", "Home");
            }

            // إذا في أخطاء = نعيد عرض النموذج
            return View(model);
        }

        
        // صفحة تسجيل الدخول - عرض النموذج (GET)
        // الرابط: /Account/Login
        
        public ActionResult Login()
        {
            // إذا مسجل دخول = توجيه للرئيسية
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        
        // معالجة تسجيل الدخول (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email?.ToLowerInvariant();
                // البحث عن المستخدم بالإيميل
                var musteri = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);

                if (musteri != null)
                {
                    // التحقق من كلمة المرور المشفرة
                    if (SecurityHelper.VerifyPassword(model.Sifre, musteri.Sifre))
                    {
                        // حفظ بيانات الجلسة
                        Session["MusteriId"] = musteri.MusteriId;  // رقم المستخدم
                        Session["MusteriAd"] = musteri.TamAd;      // اسم المستخدم
                        Session["IsAdmin"] = musteri.IsAdmin;      // هل أدمن؟
                        
                        // بيانات مدير المطعم
                        Session["IsRestoranAdmin"] = musteri.IsRestoranAdmin;  // هل مدير مطعم؟
                        Session["RestoranId"] = musteri.RestoranId;  // رقم المطعم المدار

                        // التوجيه للصفحة السابقة إن وجدت
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                // رسالة خطأ عامة (للأمان - لا نكشف أي البيانات خاطئة)
                ModelState.AddModelError("", "Email veya sifre hatali");
            }

            return View(model);
        }

       
        // تسجيل الخروج
        // الرابط: /Account/Logout
        
        public ActionResult Logout()
        {
            // مسح جميع بيانات الجلسة
            Session.Clear();
            Session.Abandon();
            
            // مسح الكوكيز
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            
            TempData["Message"] = "Basariyla cikis yaptiniz.";
            return RedirectToAction("Index", "Home");
        }

       
        // صفحة الملف الشخصي - عرض البيانات (GET)
        // الرابط: /Account/Profile
        public new ActionResult Profile()
        {
            // التحقق من تسجيل الدخول
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            // جلب بيانات المستخدم من قاعدة البيانات
            int musteriId = (int)Session["MusteriId"];
            var musteri = db.Musteriler.Find(musteriId);

            if (musteri == null)
            {
                Session.Clear();
                return RedirectToAction("Login");
            }

            return View(musteri);
        }

        
        // تحديث الملف الشخصي (POST)
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Musteri musteri)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            // التأكد أن المستخدم يعدل ملفه فقط (أمان)
            int currentUserId = (int)Session["MusteriId"];
            if (musteri.MusteriId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "Forbidden");
            }

            // تخطي التحقق من كلمة المرور إذا لم تُعدَّل
            if (string.IsNullOrEmpty(musteri.Sifre))
            {
                ModelState.Remove("Sifre");
            }

            if (ModelState.IsValid)
            {
                var existingMusteri = db.Musteriler.Find(musteri.MusteriId);
                if (existingMusteri != null)
                {
                    // تحديث البيانات
                    existingMusteri.Ad = musteri.Ad?.Trim();
                    existingMusteri.Soyad = musteri.Soyad?.Trim();
                    existingMusteri.Email = musteri.Email?.ToLowerInvariant();
                    existingMusteri.Telefon = musteri.Telefon?.Trim();
                    existingMusteri.Adres = musteri.Adres?.Trim();

                    // تشفير كلمة المرور الجديدة إن وُجدت
                    if (!string.IsNullOrEmpty(musteri.Sifre))
                    {
                        existingMusteri.Sifre = SecurityHelper.HashPassword(musteri.Sifre);
                    }

                    db.SaveChanges();
                    Session["MusteriAd"] = existingMusteri.TamAd;
                    TempData["Message"] = "Profil basariyla guncellendi";
                }

                return RedirectToAction("Profile");
            }

            return View(musteri);
        }

        
        // عرض طلبات المستخدم
        // الرابط: /Account/Orders
        
        public ActionResult Orders()
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            int musteriId = (int)Session["MusteriId"];
            // جلب الطلبات مرتبة من الأحدث للأقدم
            var orders = db.OrderPages.Where(o => o.MusteriId == musteriId)
                                      .OrderByDescending(o => o.OrderDate)
                                      .ToList();

            return View(orders);
        }

        
        // صفحة نسيت كلمة المرور (GET)
        
        public ActionResult ForgotPassword()
        {
            return View();
        }

        
        // معالجة طلب استعادة كلمة المرور (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["Error"] = "Lutfen e-posta adresinizi girin";
                return View();
            }

            var email = Email.ToLowerInvariant();
            var musteri = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);

            if (musteri != null)
            {
                // في التطبيق الحقيقي: إرسال إيميل مع رابط إعادة التعيين
                TempData["Success"] = "Sifre sifirlama linki e-posta adresinize gonderildi.";
            }
            else
            {
                // لا نكشف إذا كان الإيميل موجوداً أم لا (أمان)
                TempData["Success"] = "Eger bu e-posta adresi kayitliysa, link gonderilecektir.";
            }

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
