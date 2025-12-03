using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;

namespace mvc_full.Controllers
{
    public class SearchController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: Search
        public ActionResult Index(string query, string type = "all")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            var searchResult = new SearchResultViewModel
            {
                Query = query
            };

            query = query.ToLower();

            if (type == "all" || type == "restaurants")
            {
                searchResult.Restaurants = db.Restoranlar
                    .Where(r => r.Ad.ToLower().Contains(query) ||
                               r.Aciklama.ToLower().Contains(query) ||
                               r.Adres.ToLower().Contains(query))
                    .Take(10)
                    .ToList();
            }

            if (type == "all" || type == "foods")
            {
                searchResult.Foods = db.Yemekler
                    .Where(y => y.Ad.ToLower().Contains(query) ||
                               y.Aciklama.ToLower().Contains(query))
                    .Take(10)
                    .ToList();
            }

            ViewBag.SearchType = type;
            return View(searchResult);
        }

        // AJAX Search
        [HttpPost]
        public JsonResult QuickSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false });
            }

            query = query.ToLower();

            var restaurants = db.Restoranlar
                .Where(r => r.Ad.ToLower().Contains(query))
                .Take(5)
                .Select(r => new
                {
                    id = r.RestoranId,
                    name = r.Ad,
                    type = "restaurant",
                    image = r.ResimUrl
                })
                .ToList();

            var foods = db.Yemekler
                .Where(y => y.Ad.ToLower().Contains(query))
                .Take(5)
                .Select(y => new
                {
                    id = y.YemekId,
                    name = y.Ad,
                    type = "food",
                    image = y.ResimUrl,
                    price = y.Fiyat
                })
                .ToList();

            return Json(new
            {
                success = true,
                restaurants = restaurants,
                foods = foods
            });
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

    public class SearchResultViewModel
    {
        public string Query { get; set; }
        public List<Restoran> Restaurants { get; set; } = new List<Restoran>();
        public List<Yemek> Foods { get; set; } = new List<Yemek>();
    }
}
