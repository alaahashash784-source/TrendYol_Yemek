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
    // Account controller - login, register, profile management
    public class AccountController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Register
        public ActionResult Register()
        {
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email?.ToLowerInvariant();
                
                var user = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);
                if (user != null)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullaniliyor");
                    return View(model);
                }

                // Create new user with hashed password
                var musteri = new Musteri
                {
                    Ad = model.Ad?.Trim(),
                    Soyad = model.Soyad?.Trim(),
                    Email = email,
                    Telefon = model.Telefon?.Trim(),
                    Adres = model.Adres?.Trim(),
                    Sifre = SecurityHelper.HashPassword(model.Sifre),
                    KayitTarihi = DateTime.Now
                };

                db.Musteriler.Add(musteri);
                db.SaveChanges();

                // Save session
                Session["MusteriId"] = musteri.MusteriId;
                Session["MusteriAd"] = musteri.TamAd;

                TempData["Message"] = "Kayit basarili! Hos geldiniz.";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: Login
        public ActionResult Login()
        {
            if (Session["MusteriId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email?.ToLowerInvariant();
                var musteri = db.Musteriler.FirstOrDefault(m => m.Email.ToLower() == email);

                if (musteri != null)
                {
                    // Verify password
                    if (SecurityHelper.VerifyPassword(model.Sifre, musteri.Sifre))
                    {
                        // Save session
                        Session["MusteriId"] = musteri.MusteriId;
                        Session["MusteriAd"] = musteri.TamAd;
                        Session["IsAdmin"] = musteri.IsAdmin;

                        // Redirect
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                // Generic error for security
                ModelState.AddModelError("", "Email veya sifre hatali");
            }

            return View(model);
        }

        // GET: Logout
        public ActionResult Logout()
        {
            // Clear session
            Session.Clear();
            Session.Abandon();
            
            // Clear cookies
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
            
            TempData["Message"] = "Basariyla cikis yaptiniz.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Profile
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
                Session.Clear();
                return RedirectToAction("Login");
            }

            return View(musteri);
        }

        // POST: Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(Musteri musteri)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login");
            }

            // Ensure user can only edit own profile
            int currentUserId = (int)Session["MusteriId"];
            if (musteri.MusteriId != currentUserId)
            {
                return new HttpStatusCodeResult(403, "Forbidden");
            }

            // Skip password validation if not updating
            if (string.IsNullOrEmpty(musteri.Sifre))
            {
                ModelState.Remove("Sifre");
            }

            if (ModelState.IsValid)
            {
                var existingMusteri = db.Musteriler.Find(musteri.MusteriId);
                if (existingMusteri != null)
                {
                    // Update info
                    existingMusteri.Ad = musteri.Ad?.Trim();
                    existingMusteri.Soyad = musteri.Soyad?.Trim();
                    existingMusteri.Email = musteri.Email?.ToLowerInvariant();
                    existingMusteri.Telefon = musteri.Telefon?.Trim();
                    existingMusteri.Adres = musteri.Adres?.Trim();

                    // Hash password if provided
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

        // GET: ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
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
                // In real app: send email with reset link
                TempData["Success"] = "Sifre sifirlama linki e-posta adresinize gonderildi. Lutfen e-postanizi kontrol edin.";
            }
            else
            {
                // Don't reveal if email exists
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
