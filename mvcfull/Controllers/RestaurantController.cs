// كنترولر المطاعم - عرض وإدارة المطاعم
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
    // إدارة المطاعم - هذا الكنترولر يتحكم في عمليات المطاعم
    public class RestaurantController : Controller
    {
        // كائن الاتصال بالقاعدة
        private ABCDbContext db = new ABCDbContext();

        // الصفحة الرئيسية للمطاعم
        public ActionResult Index()
        {
            var restaurants = db.Restoranlar.ToList();
            return View(restaurants);
        }

        // عرض تفاصيل مطعم واحد مع أطعمته
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

            // جلب أطعمة هذا المطعم
            var foods = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            ViewBag.Foods = foods;

            return View(restaurant);
        }

        // صفحة إضافة مطعم جديد
        // TODO: إضافة خريطة لاختيار الموقع
        [AdminAuthorizationFilter]
        public ActionResult Create()
        {
            return View();
        }

        // حفظ المطعم الجديد
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

        // تعديل بيانات مطعم
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

        // صفحة حذف مطعم
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
            
            // Önce bu restorana ait tüm yemekleri sil
            var yemekler = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            foreach (var yemek in yemekler)
            {
                // Sepetten de bu yemekleri temizle
                var sepetItems = db.Sepetler.Where(s => s.YemekId == yemek.YemekId).ToList();
                db.Sepetler.RemoveRange(sepetItems);
                
                db.Yemekler.Remove(yemek);
            }
            
            // Sonra restoranı sil
            db.Restoranlar.Remove(restoran);
            db.SaveChanges();
            
            TempData["Message"] = $"{restoran.Ad} ve tüm yemekleri başarıyla silindi.";
            return RedirectToAction("Index");
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