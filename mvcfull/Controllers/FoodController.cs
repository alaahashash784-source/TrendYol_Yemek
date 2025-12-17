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

        // GET: Food/Create - Admin or Restaurant Admin only
        [AnyRestaurantAdminFilter]
        public ActionResult Create()
        {
            // إذا كان مدير مطعم، نحدد المطعم تلقائياً
            int? userRestoranId = Session["RestoranId"] as int?;
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            if (isAdmin)
            {
                ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad");
            }
            else if (userRestoranId.HasValue)
            {
                ViewBag.RestoranId = new SelectList(db.Restoranlar.Where(r => r.RestoranId == userRestoranId), "RestoranId", "Ad");
                ViewBag.LockedRestoran = true;
            }
            
            ViewBag.Kategoriler = GetKategoriler();
            return View();
        }

        // POST: Food/Create - Admin or Restaurant Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AnyRestaurantAdminFilter]
        public ActionResult Create([Bind(Include = "YemekId,Ad,Aciklama,Fiyat,ResimUrl,RestoranId,HazirlanmaSuresi,Kategori")] Yemek yemek)
        {
            // التحقق من صلاحية مدير المطعم
            int? userRestoranId = Session["RestoranId"] as int?;
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            if (!isAdmin && userRestoranId.HasValue && yemek.RestoranId != userRestoranId.Value)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            
            if (ModelState.IsValid)
            {
                db.Yemekler.Add(yemek);
                db.SaveChanges();
                
                // إذا كان مدير مطعم، يعود لصفحة مطعمه
                if (!isAdmin && userRestoranId.HasValue)
                {
                    return RedirectToAction("ByRestaurant", new { id = userRestoranId.Value });
                }
                return RedirectToAction("Index");
            }

            if (isAdmin)
            {
                ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            }
            else if (userRestoranId.HasValue)
            {
                ViewBag.RestoranId = new SelectList(db.Restoranlar.Where(r => r.RestoranId == userRestoranId), "RestoranId", "Ad");
                ViewBag.LockedRestoran = true;
            }
            ViewBag.Kategoriler = GetKategoriler();
            return View(yemek);
        }

        // GET: Food/Edit/5 - Admin or Restaurant Admin only
        [RestaurantAdminFilter]
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
            ViewBag.Kategoriler = GetKategoriler();
            return View(yemek);
        }

        // POST: Food/Edit/5 - Admin or Restaurant Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RestaurantAdminFilter]
        public ActionResult Edit([Bind(Include = "YemekId,Ad,Aciklama,Fiyat,ResimUrl,RestoranId,HazirlanmaSuresi,Kategori")] Yemek yemek)
        {
            // التحقق من صلاحية مدير المطعم
            int? userRestoranId = Session["RestoranId"] as int?;
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            if (!isAdmin && userRestoranId.HasValue && yemek.RestoranId != userRestoranId.Value)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            
            if (ModelState.IsValid)
            {
                db.Entry(yemek).State = EntityState.Modified;
                db.SaveChanges();
                
                // إذا كان مدير مطعم، يعود لصفحة مطعمه
                if (!isAdmin && userRestoranId.HasValue)
                {
                    return RedirectToAction("ByRestaurant", new { id = userRestoranId.Value });
                }
                return RedirectToAction("Index");
            }

            ViewBag.RestoranId = new SelectList(db.Restoranlar, "RestoranId", "Ad", yemek.RestoranId);
            ViewBag.Kategoriler = GetKategoriler();
            return View(yemek);
        }

        // GET: Food/Delete/5 - Admin or Restaurant Admin only
        [RestaurantAdminFilter]
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

        // POST: Food/Delete/5 - Admin or Restaurant Admin only
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RestaurantAdminFilter]
        public ActionResult DeleteConfirmed(int id)
        {
            var yemek = db.Yemekler.Find(id);
            int restoranId = yemek.RestoranId;
            
            // التحقق من صلاحية مدير المطعم
            int? userRestoranId = Session["RestoranId"] as int?;
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            
            if (!isAdmin && userRestoranId.HasValue && restoranId != userRestoranId.Value)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            
            db.Yemekler.Remove(yemek);
            db.SaveChanges();
            
            // إذا كان مدير مطعم، يعود لصفحة مطعمه
            if (!isAdmin && userRestoranId.HasValue)
            {
                return RedirectToAction("ByRestaurant", new { id = userRestoranId.Value });
            }
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
            
            // التحقق من صلاحية الإدارة لهذا المطعم
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            bool isRestoranAdmin = Session["IsRestoranAdmin"] != null && (bool)Session["IsRestoranAdmin"];
            int? userRestoranId = Session["RestoranId"] as int?;
            
            ViewBag.CanManage = isAdmin || (isRestoranAdmin && userRestoranId == id);

            return View(yemekler);
        }
        
        // قائمة فئات الطعام
        private List<SelectListItem> GetKategoriler()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Kategori Seçin" },
                new SelectListItem { Value = "Pizza", Text = "Pizza" },
                new SelectListItem { Value = "Kebap", Text = "Kebap" },
                new SelectListItem { Value = "Döner", Text = "Döner" },
                new SelectListItem { Value = "Burger", Text = "Burger" },
                new SelectListItem { Value = "Makarna", Text = "Makarna" },
                new SelectListItem { Value = "Salata", Text = "Salata" },
                new SelectListItem { Value = "Çorba", Text = "Çorba" },
                new SelectListItem { Value = "Tatlı", Text = "Tatlı" },
                new SelectListItem { Value = "İçecek", Text = "İçecek" },
                new SelectListItem { Value = "Kahvaltı", Text = "Kahvaltı" },
                new SelectListItem { Value = "Deniz Ürünleri", Text = "Deniz Ürünleri" },
                new SelectListItem { Value = "Tavuk", Text = "Tavuk" },
                new SelectListItem { Value = "Et Yemekleri", Text = "Et Yemekleri" },
                new SelectListItem { Value = "Vegan", Text = "Vegan" },
                new SelectListItem { Value = "Diğer", Text = "Diğer" }
            };
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
