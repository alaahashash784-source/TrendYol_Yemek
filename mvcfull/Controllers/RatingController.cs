using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;
using mvc_full.Filters;

namespace mvc_full.Controllers
{
    /// <summary>
    /// كنترولر التقييمات والتعليقات
    /// يسمح للمستخدمين بتقييم الأطعمة والمطاعم
    /// </summary>
    public class RatingController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // عرض تقييمات طعام معين
        // GET: /Rating/Food/5
        public ActionResult Food(int id)
        {
            var ratings = db.Ratings
                .Where(r => r.YemekId == id && r.Onaylandi)
                .OrderByDescending(r => r.DegerlendirmeTarihi)
                .ToList();
            
            var yemek = db.Yemekler.Find(id);
            if (yemek == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Yemek = yemek;
            ViewBag.AverageRating = ratings.Any() ? ratings.Average(r => r.Puan) : 0;
            ViewBag.TotalRatings = ratings.Count;
            
            return View(ratings);
        }
        
        // عرض تقييمات مطعم معين
        // GET: /Rating/Restaurant/5
        public ActionResult Restaurant(int id)
        {
            var ratings = db.Ratings
                .Where(r => r.RestoranId == id && r.Onaylandi)
                .OrderByDescending(r => r.DegerlendirmeTarihi)
                .ToList();
            
            var restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Restoran = restoran;
            ViewBag.AverageRating = ratings.Any() ? ratings.Average(r => r.Puan) : 0;
            ViewBag.TotalRatings = ratings.Count;
            
            return View(ratings);
        }
        
        // صفحة إضافة تقييم لطعام
        // GET: /Rating/AddFoodRating/5
        public ActionResult AddFoodRating(int id, int? orderId)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsolutePath });
            }
            
            var yemek = db.Yemekler.Find(id);
            if (yemek == null)
            {
                return HttpNotFound();
            }
            
            int musteriId = (int)Session["MusteriId"];
            
            // التحقق من أن المستخدم لم يقيم هذا الطعام من قبل
            var existingRating = db.Ratings.FirstOrDefault(r => r.MusteriId == musteriId && r.YemekId == id);
            if (existingRating != null)
            {
                TempData["Error"] = "Bu yemegi zaten degerlendirmissiniz.";
                return RedirectToAction("Details", "Food", new { id = id });
            }
            
            ViewBag.Yemek = yemek;
            ViewBag.OrderId = orderId;
            
            return View(new Rating { YemekId = id, OrderId = orderId });
        }
        
        // إضافة تقييم لطعام
        // POST: /Rating/AddFoodRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFoodRating(Rating rating)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            int musteriId = (int)Session["MusteriId"];
            
            // التحقق من أن المستخدم لم يقيم هذا الطعام من قبل
            var existingRating = db.Ratings.FirstOrDefault(r => r.MusteriId == musteriId && r.YemekId == rating.YemekId);
            if (existingRating != null)
            {
                TempData["Error"] = "Bu yemegi zaten degerlendirmissiniz.";
                return RedirectToAction("Details", "Food", new { id = rating.YemekId });
            }
            
            if (rating.Puan < 1 || rating.Puan > 5)
            {
                ModelState.AddModelError("Puan", "Puan 1-5 arasinda olmalidir");
            }
            
            if (ModelState.IsValid)
            {
                rating.MusteriId = musteriId;
                rating.DegerlendirmeTarihi = DateTime.Now;
                rating.Onaylandi = true;
                
                db.Ratings.Add(rating);
                db.SaveChanges();
                
                TempData["Success"] = "Degerlendirmeniz icin tesekkurler!";
                return RedirectToAction("Details", "Food", new { id = rating.YemekId });
            }
            
            var yemek = db.Yemekler.Find(rating.YemekId);
            ViewBag.Yemek = yemek;
            return View(rating);
        }
        
        // صفحة إضافة تقييم لمطعم
        // GET: /Rating/AddRestaurantRating/5
        public ActionResult AddRestaurantRating(int id, int? orderId)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsolutePath });
            }
            
            var restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            
            int musteriId = (int)Session["MusteriId"];
            
            // التحقق من أن المستخدم لم يقيم هذا المطعم من قبل
            var existingRating = db.Ratings.FirstOrDefault(r => r.MusteriId == musteriId && r.RestoranId == id);
            if (existingRating != null)
            {
                TempData["Error"] = "Bu restorani zaten degerlendirmissiniz.";
                return RedirectToAction("Details", "Restaurant", new { id = id });
            }
            
            ViewBag.Restoran = restoran;
            ViewBag.OrderId = orderId;
            
            return View(new Rating { RestoranId = id, OrderId = orderId });
        }
        
        // إضافة تقييم لمطعم
        // POST: /Rating/AddRestaurantRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRestaurantRating(Rating rating)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            int musteriId = (int)Session["MusteriId"];
            
            // التحقق من أن المستخدم لم يقيم هذا المطعم من قبل
            var existingRating = db.Ratings.FirstOrDefault(r => r.MusteriId == musteriId && r.RestoranId == rating.RestoranId);
            if (existingRating != null)
            {
                TempData["Error"] = "Bu restorani zaten degerlendirmissiniz.";
                return RedirectToAction("Details", "Restaurant", new { id = rating.RestoranId });
            }
            
            if (rating.Puan < 1 || rating.Puan > 5)
            {
                ModelState.AddModelError("Puan", "Puan 1-5 arasinda olmalidir");
            }
            
            if (ModelState.IsValid)
            {
                rating.MusteriId = musteriId;
                rating.DegerlendirmeTarihi = DateTime.Now;
                rating.Onaylandi = true;
                
                db.Ratings.Add(rating);
                db.SaveChanges();
                
                TempData["Success"] = "Degerlendirmeniz icin tesekkurler!";
                return RedirectToAction("Details", "Restaurant", new { id = rating.RestoranId });
            }
            
            var restoran = db.Restoranlar.Find(rating.RestoranId);
            ViewBag.Restoran = restoran;
            return View(rating);
        }
        
        // حذف تقييم (للمستخدم صاحب التقييم أو الأدمن)
        // POST: /Rating/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["MusteriId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            
            int musteriId = (int)Session["MusteriId"];
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            if (rating.MusteriId != musteriId && !isAdmin)
            {
                return new HttpStatusCodeResult(403);
            }
            
            int? yemekId = rating.YemekId;
            int? restoranId = rating.RestoranId;
            
            db.Ratings.Remove(rating);
            db.SaveChanges();
            
            TempData["Success"] = "Degerlendirme silindi.";
            
            if (yemekId.HasValue)
            {
                return RedirectToAction("Details", "Food", new { id = yemekId.Value });
            }
            else if (restoranId.HasValue)
            {
                return RedirectToAction("Details", "Restaurant", new { id = restoranId.Value });
            }
            
            return RedirectToAction("Index", "Home");
        }
        
        // API: الحصول على متوسط تقييم طعام (للاستخدام في AJAX)
        [HttpGet]
        public JsonResult GetFoodRating(int id)
        {
            var ratings = db.Ratings.Where(r => r.YemekId == id && r.Onaylandi).ToList();
            return Json(new
            {
                average = ratings.Any() ? Math.Round(ratings.Average(r => r.Puan), 1) : 0,
                count = ratings.Count
            }, JsonRequestBehavior.AllowGet);
        }
        
        // API: الحصول على متوسط تقييم مطعم (للاستخدام في AJAX)
        [HttpGet]
        public JsonResult GetRestaurantRating(int id)
        {
            var ratings = db.Ratings.Where(r => r.RestoranId == id && r.Onaylandi).ToList();
            return Json(new
            {
                average = ratings.Any() ? Math.Round(ratings.Average(r => r.Puan), 1) : 0,
                count = ratings.Count
            }, JsonRequestBehavior.AllowGet);
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
