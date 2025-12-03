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
    public class RestoranController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Restoran
        public ActionResult Index()
        {
            return View(db.Restoranlar.ToList());
        }

        // GET: Restoran/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restoran restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            return View(restoran);
        }

        // GET: Restoran/Menu/5
        public ActionResult Menu(int? id)
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

            var yemekler = db.Yemekler.Where(y => y.RestoranId == id).ToList();
            ViewBag.RestoranAdi = restoran.Ad;
            ViewBag.RestoranId = id;
            
            return View(yemekler);
        }

        // GET: Restoran/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Restoran/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Restoran/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restoran restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            return View(restoran);
        }

        // POST: Restoran/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Restoran/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restoran restoran = db.Restoranlar.Find(id);
            if (restoran == null)
            {
                return HttpNotFound();
            }
            return View(restoran);
        }

        // POST: Restoran/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restoran restoran = db.Restoranlar.Find(id);
            db.Restoranlar.Remove(restoran);
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