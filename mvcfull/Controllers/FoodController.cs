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
    public class FoodController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Food
        public ActionResult Index()
        {
            var yemekler = db.Yemekler.Include(y => y.Restoran).ToList();
            return View(yemekler);
        }

        // GET: Food/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == id);
            if (yemek == null)
            {
                return HttpNotFound();
            }

            return View(yemek);
        }

        // GET: Food/Create - Admin only
        [AdminAuthorizationFilter]
        public ActionResult Create()
        {
            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad");
            return View();
        }

        // POST: Food/Create - Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult Create([Bind(Include = "YemekId,Ad,Aciklama,Fiyat,ResimUrl,RestoranId")] Yemek yemek)
        {
            if (ModelState.IsValid)
            {
                db.Yemekler.Add(yemek);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            return View(yemek);
        }

        // GET: Food/Edit/5 - Admin only
        [AdminAuthorizationFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var yemek = db.Yemekler.Find(id);
            if (yemek == null)
            {
                return HttpNotFound();
            }

            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            return View(yemek);
        }

        // POST: Food/Edit/5 - Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult Edit([Bind(Include = "YemekId,Ad,Aciklama,Fiyat,ResimUrl,RestoranId")] Yemek yemek)
        {
            if (ModelState.IsValid)
            {
                db.Entry(yemek).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            return View(yemek);
        }

        // GET: Food/Delete/5 - Admin only
        [AdminAuthorizationFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == id);
            if (yemek == null)
            {
                return HttpNotFound();
            }

            return View(yemek);
        }

        // POST: Food/Delete/5 - Admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AdminAuthorizationFilter]
        public ActionResult DeleteConfirmed(int id)
        {
            var yemek = db.Yemekler.Find(id);
            db.Yemekler.Remove(yemek);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Food/QuickDelete - Delete by name (for fixing data issues)
        public ActionResult QuickDelete(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var yemekler = db.Yemekler.Where(y => y.Ad.ToLower().Contains(name.ToLower())).ToList();
            foreach (var yemek in yemekler)
            {
                db.Yemekler.Remove(yemek);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Food/ByRestaurant/5
        public ActionResult ByRestaurant(int? id)
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

            var yemekler = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            ViewBag.RestaurantName = restaurant.Ad;
            ViewBag.RestaurantId = id;

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
