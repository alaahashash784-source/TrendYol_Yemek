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
        public ActionResult Index()
        {
            var restaurants = db.Restoranlar.ToList();
            return View(restaurants);
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