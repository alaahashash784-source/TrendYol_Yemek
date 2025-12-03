using System.Linq;
using System.Web.Mvc;

namespace mvc_full.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            ViewBag.Message = "???? ?????????? ???? ???? ????!";
            return View();
        }

        // GET: Test/CheckDatabase
        public ActionResult CheckDatabase()
        {
            try
            {
                using (var db = new Models.ABCDbContext())
                {
                    var restaurantCount = db.Restoranlar.Count();
                    var foodCount = db.Yemekler.Count();
                    var customerCount = db.Musteriler.Count();

                    ViewBag.RestaurantCount = restaurantCount;
                    ViewBag.FoodCount = foodCount;
                    ViewBag.CustomerCount = customerCount;
                    ViewBag.Status = "????? ???????? ???? ?????!";
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Status = "??? ?? ????? ????????: " + ex.Message;
                ViewBag.RestaurantCount = 0;
                ViewBag.FoodCount = 0;
                ViewBag.CustomerCount = 0;
            }

            return View();
        }
    }
}