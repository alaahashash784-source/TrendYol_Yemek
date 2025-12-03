// كنترولر الحسابات - تسجيل الدخول والخروج وإدارة المستخدمين
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
    // هذا الكلاس مسؤول عن إدارة حسابات المستخدمين
    // يتضمن: تسجيل الدخول، إنشاء حساب جديد، تعديل الملف الشخصي
    // TODO: إضافة خاصية استعادة كلمة المرور لاحقاً
    public class AccountController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // صفحة التسجيل - GET
        public ActionResult Register()
        {
            // إذا كان المستخدم مسجل دخول، نحوله للصفحة الرئيسية
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        // Fixed: Added proper password hashing for security
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // تحويل الإيميل لحروف صغيرة للمقارنة الصحيحة
                string email = model.Email?.ToLowerInvariant(); // TODO: ممكن نضيف تحقق أفضل
                
                // نتحقق إذا الإيميل موجود مسبقاً
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
                    Sifre = SecurityHelper.HashPassword(model.Sifre), // تشفير كلمة المرور
                    KayitTarihi = DateTime.Now
                };

                db.Musteriler.Add(musteri);
                db.SaveChanges();

                // حفظ بيانات المستخدم في الجلسة
                Session["MusteriId"] = musteri.MusteriId;
                Session["MusteriAd"] = musteri.TamAd;

                TempData["Message"] = "Kayit basarili! Hos geldiniz.";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // صفحة تسجيل الدخول
        public ActionResult Login()
        {
            // لو مسجل دخول، روح للصفحة الرئيسية
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // معالجة تسجيل الدخول
        // نتحقق من كلمة المرور المشفرة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // تحويل الإيميل لحروف صغيرة
                string email = model.Email?.ToLowerInvariant();
                
                // البحث عن المستخدم بالإيميل
                var musteri = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);

                if (musteri != null)
                {
                    // التحقق من كلمة المرور
                    if (SecurityHelper.VerifyPassword(model.Sifre, musteri.Sifre))
                    {
                        // حفظ الجلسة
                        Session["MusteriId"] = musteri.MusteriId;
                        Session["MusteriAd"] = musteri.TamAd;
                        Session["IsAdmin"] = musteri.IsAdmin;

                        // إعادة التوجيه للصفحة المطلوبة
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                // رسالة خطأ عامة لأسباب أمنية
                ModelState.AddModelError("", "Email veya sifre hatali");
            }

            return View(model);
        }

        // تسجيل الخروج
        public ActionResult Logout()
        {
            // مسح جميع بيانات الجلسة
            Session.Clear();
            Session.Abandon();
            
            // مسح الكوكيز - TODO: ممكن نضيف تأكيد قبل الخروج
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            
            TempData["Message"] = "Basariyla cikis yaptiniz.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        // Note: Using 'new' keyword because Controller base class has a Profile property
        public new ActionResult Profile()
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            int musteriId = (int)Session["MusteriId"];
            var musteri = db.Musteriler.Find(musteriId);

            if (musteri == null)
            {
                // Session has invalid user, clear it
                Session.Clear();
                return RedirectToAction("Login");
            }

            return View(musteri);
        }

        // POST: Account/Profile
        // Fixed: Proper password update with hashing
        // Note: Using 'new' keyword because Controller base class has a Profile property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Musteri musteri)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            // Ensure user can only edit their own profile
            int currentUserId = (int)Session["MusteriId"];
            if (musteri.MusteriId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "Forbidden");
            }

            // Remove Sifre from validation if not being updated
            if (string.IsNullOrEmpty(musteri.Sifre))
            {
                ModelState.Remove("Sifre");
            }

            if (ModelState.IsValid)
            {
                var existingMusteri = db.Musteriler.Find(musteri.MusteriId);
                if (existingMusteri != null)
                {
                    // Update basic info
                    existingMusteri.Ad = musteri.Ad?.Trim();
                    existingMusteri.Soyad = musteri.Soyad?.Trim();
                    existingMusteri.Email = musteri.Email?.ToLowerInvariant();
                    existingMusteri.Telefon = musteri.Telefon?.Trim();
                    existingMusteri.Adres = musteri.Adres?.Trim();

                    // Only update password if provided (and hash it)
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

        // GET: Account/Orders
        public ActionResult Orders()
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            int musteriId = (int)Session["MusteriId"];
            var orders = db.OrderPages.Where(o => o.MusteriId == musteriId)
                                      .OrderByDescending(o => o.OrderDate)
                                      .ToList();

            return View(orders);
        }

        // GET: Account/ForgotPassword - صفحة استعادة كلمة المرور
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword - معالجة طلب استعادة كلمة المرور
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
                // في التطبيق الحقيقي: إرسال إيميل برابط إعادة التعيين
                // هنا نعرض رسالة نجاح فقط
                TempData["Success"] = "Sifre sifirlama linki e-posta adresinize gonderildi. Lutfen e-postanizi kontrol edin.";
            }
            else
            {
                // لا نخبر المستخدم أن الإيميل غير موجود لأسباب أمنية
                TempData["Success"] = "Eger bu e-posta adresi sistemimizde kayitliysa, sifre sifirlama linki gonderilecektir.";
            }

            return View();
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
