using System;
using System.Linq;
using System.Web.Mvc;
using mvc_full.Models;
using mvc_full.Helpers;

namespace mvc_full.Controllers
{
    /// <summary>
    /// Setup controller for creating admin account and auto-login.
    /// This should be removed or secured in production.
    /// </summary>
    public class SetupController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Setup/CreateAdmin
        public ActionResult CreateAdmin()
        {
            try
            {
                // Check if admin already exists
                var adminUser = db.Musteriler.FirstOrDefault(m => m.Email == "admin@trendyol.com");
                
                if (adminUser == null)
                {
                    // Create new admin account with properly hashed password
                    adminUser = new Musteri
                    {
                        Ad = "Admin",
                        Soyad = "User",
                        Email = "admin@trendyol.com",
                        Sifre = SecurityHelper.HashPassword("admin123"), // Properly hashed
                        Telefon = "5551234567",
                        Adres = "Admin Address, Istanbul",
                        KayitTarihi = DateTime.Now,
                        IsAdmin = true
                    };

                    db.Musteriler.Add(adminUser);
                    db.SaveChanges();
                    
                    TempData["SetupMessage"] = "Admin hesabi olusturuldu!";
                }
                else
                {
                    // Update existing admin - reset password and ensure IsAdmin is true
                    adminUser.Sifre = SecurityHelper.HashPassword("admin123");
                    adminUser.IsAdmin = true;
                    db.SaveChanges();
                    
                    TempData["SetupMessage"] = "Admin hesabi guncellendi!";
                }

                // Auto-login as admin
                Session["MusteriId"] = adminUser.MusteriId;
                Session["MusteriAd"] = adminUser.TamAd;
                Session["IsAdmin"] = true;

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                TempData["SetupError"] = "Hata: " + ex.Message;
                return RedirectToAction("Success");
            }
        }

        // GET: Setup/Success
        public ActionResult Success()
        {
            ViewBag.Message = TempData["SetupMessage"];
            ViewBag.Error = TempData["SetupError"];
            ViewBag.IsLoggedIn = Session["MusteriId"] != null;
            ViewBag.IsAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
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
