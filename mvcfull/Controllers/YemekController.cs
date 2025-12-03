using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;

namespace mvc_full.Controllers
{
    public class YemekController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Yemek
        public ActionResult Index()
        {
            var yemekler = db.Yemekler.Include(y => y.Restoran);
            return View(yemekler.ToList());
        }

        // GET: Yemek/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yemek yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == id);
            if (yemek == null)
            {
                return HttpNotFound();
            }
            return View(yemek);
        }

        // GET: Yemek/Create
        public ActionResult Create()
        {
            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad");
            return View();
        }

        // POST: Yemek/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Yemek/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yemek yemek = db.Yemekler.Find(id);
            if (yemek == null)
            {
                return HttpNotFound();
            }
            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            return View(yemek);
        }

        // POST: Yemek/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Yemek/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Yemek yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == id);
            if (yemek == null)
            {
                return HttpNotFound();
            }
            return View(yemek);
        }

        // POST: Yemek/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Yemek yemek = db.Yemekler.Find(id);
            db.Yemekler.Remove(yemek);
            db.SaveChanges();
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