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
    // Restaurant controller - CRUD operations
    public class RestaurantController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Restaurant
        public ActionResult Index(string kategori = null, decimal? minFiyat = null, decimal? maxFiyat = null, 
                                   int? maxHazirlanmaSuresi = null, string siralama = null)
        {
            var restaurants = db.Restoranlar.AsQueryable();
            
            // البحث حسب نوع الطعام (فئة)
            if (!string.IsNullOrEmpty(kategori))
            {
                var restoranIds = db.Yemekler
                    .Where(y => y.Kategori != null && y.Kategori.ToLower().Contains(kategori.ToLower()))
                    .Select(y => y.RestoranId)
                    .Distinct()
                    .ToList();
                restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
                ViewBag.SearchKategori = kategori;
            }
            
            // فلترة حسب السعر الأدنى
            if (minFiyat.HasValue)
            {
                var restoranIds = db.Yemekler
                    .Where(y => y.Fiyat >= minFiyat.Value)
                    .Select(y => y.RestoranId)
                    .Distinct()
                    .ToList();
                restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
                ViewBag.MinFiyat = minFiyat;
            }
            
            // فلترة حسب السعر الأعلى
            if (maxFiyat.HasValue)
            {
                var restoranIds = db.Yemekler
                    .Where(y => y.Fiyat <= maxFiyat.Value)
                    .Select(y => y.RestoranId)
                    .Distinct()
                    .ToList();
                restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
                ViewBag.MaxFiyat = maxFiyat;
            }
            
            // فلترة حسب سرعة التحضير
            if (maxHazirlanmaSuresi.HasValue)
            {
                var restoranIds = db.Yemekler
                    .Where(y => y.HazirlanmaSuresi <= maxHazirlanmaSuresi.Value)
                    .Select(y => y.RestoranId)
                    .Distinct()
                    .ToList();
                restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
                ViewBag.MaxHazirlanmaSuresi = maxHazirlanmaSuresi;
            }
            
            // ترتيب النتائج
            var restaurantList = restaurants.ToList();
            
            // حساب متوسط السعر ووقت التحضير لكل مطعم
            var restaurantStats = new Dictionary<int, dynamic>();
            foreach (var r in restaurantList)
            {
                var yemekler = db.Yemekler.Where(y => y.RestoranId == r.RestoranId).ToList();
                restaurantStats[r.RestoranId] = new {
                    AvgFiyat = yemekler.Any() ? yemekler.Average(y => y.Fiyat) : 0,
                    AvgSure = yemekler.Any() ? yemekler.Average(y => y.HazirlanmaSuresi > 0 ? y.HazirlanmaSuresi : 15) : 0,
                    YemekSayisi = yemekler.Count
                };
            }
            ViewBag.RestaurantStats = restaurantStats;
            
            // ترتيب
            switch (siralama)
            {
                case "fiyat_artan":
                    restaurantList = restaurantList.OrderBy(r => restaurantStats[r.RestoranId].AvgFiyat).ToList();
                    break;
                case "fiyat_azalan":
                    restaurantList = restaurantList.OrderByDescending(r => restaurantStats[r.RestoranId].AvgFiyat).ToList();
                    break;
                case "hizli":
                    restaurantList = restaurantList.OrderBy(r => restaurantStats[r.RestoranId].AvgSure).ToList();
                    break;
                case "cok_yemek":
                    restaurantList = restaurantList.OrderByDescending(r => restaurantStats[r.RestoranId].YemekSayisi).ToList();
                    break;
            }
            ViewBag.Siralama = siralama;
            
            // قائمة الفئات للبحث
            ViewBag.Kategoriler = GetKategoriler();
            
            return View(restaurantList);
        }

        // GET: Restaurant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var restaurant = db.Restoranlar.Find(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            // Get foods for this restaurant
            var foods = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            ViewBag.Foods = foods;

            return View(restaurant);
        }

        // GET: Restaurant/Create
        [AdminAuthorizationFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Restaurant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult Create([Bind(Include = "RestoranId,Ad,Adres,Telefon,Email,Aciklama,ResimUrl")] Restoran restoran)
        {
            if (ModelState.IsValid)
            {
                db.Restoranlar.Add(restoran);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(restoran);
        }

        // GET: Restaurant/Edit/5
        [AdminAuthorizationFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            
            return View(restoran);
        }

        // POST: Restaurant/Edit/5 - Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult Edit([Bind(Include = "RestoranId,Ad,Adres,Telefon,Email,Aciklama,ResimUrl")] Restoran restoran)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restoran).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(restoran);
        }

        // GET: Restaurant/Delete/5
        [AdminAuthorizationFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            
            return View(restoran);
        }

        // POST: Restaurant/Delete/5 - Admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult DeleteConfirmed(int id)
        {
            var restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            
            // Delete related foods first
            var yemekler = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            foreach (var yemek in yemekler)
            {
                // Remove from cart
                var sepetItems = db.Sepetler.Where(s => s.YemekId == yemek.YemekId).ToList();
                db.Sepetler.RemoveRange(sepetItems);
                
                db.Yemekler.Remove(yemek);
            }
            
            // Then delete restaurant
            db.Restoranlar.Remove(restoran);
            db.SaveChanges();
            
            TempData["Message"] = $"{restoran.Ad} ve tüm yemekleri başarıyla silindi.";
            return RedirectToAction("Index");
        }
        
        // قائمة فئات الطعام - تعرض فقط الفئات الموجودة في قاعدة البيانات
        private List<SelectListItem> GetKategoriler()
        {
            // جلب الفئات الموجودة فعلياً في قاعدة البيانات
            var existingCategories = db.Yemekler
                .Where(y => y.Kategori != null && y.Kategori != "")
                .Select(y => y.Kategori)
                .Distinct()
                .OrderBy(k => k)
                .ToList();
            
            var kategoriler = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Tümü" }
            };
            
            foreach (var kat in existingCategories)
            {
                kategoriler.Add(new SelectListItem { Value = kat, Text = kat });
            }
            
            return kategoriler;
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