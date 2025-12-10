using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;

namespace mvc_full.Controllers
{
    public class SeedDataController : Controller
    {
        private ABCDbContext db = new ABCDbContext();

        // GET: SeedData
        public ActionResult Index()
        {
            return View();
        }

        // POST: SeedData/AddSampleData
        [HttpPost]
        public ActionResult AddSampleData()
        {
            try
            {
                // Add sample restaurants
                if (!db.Restoranlar.Any())
                {
                    var restaurants = new List<Restoran>
                    {
                        new Restoran
                        {
                            Ad = "Asala Turk Mutfagi",
                            Adres = "Sultanahmet Meydani, Istanbul",
                            Telefon = "+90 212 123 4567",
                            Email = "info@asala.com",
                            Aciklama = "Geleneksel Turk ve Osmanli mutfagindan lezzetli yemekler sunuyoruz",
                            ResimUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800"
                        },
                        new Restoran
                        {
                            Ad = "Istanbul Pizza Evi",
                            Adres = "Kadikoy, Istanbul",
                            Telefon = "+90 216 987 6543",
                            Email = "orders@istanbulpizza.com",
                            Aciklama = "En lezzetli Italyan pizzalari taze malzemelerle hazirliyoruz",
                            ResimUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800"
                        },
                        new Restoran
                        {
                            Ad = "Turk Burger House",
                            Adres = "Besiktas, Istanbul",
                            Telefon = "+90 212 555 0123",
                            Email = "info@turkishburger.com",
                            Aciklama = "El yapimi burgerler taze ve dogal malzemelerle",
                            ResimUrl = "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=800"
                        },
                        new Restoran
                        {
                            Ad = "Deniz Urunleri Restoran",
                            Adres = "Eminonu, Istanbul",
                            Telefon = "+90 212 444 7890",
                            Email = "contact@seafood.com",
                            Aciklama = "Gunluk taze balik ve deniz urunleri Istanbul Bogazindan",
                            ResimUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=800"
                        },
                        new Restoran
                        {
                            Ad = "Asya Lezzetleri",
                            Adres = "Sisli, Istanbul",
                            Telefon = "+90 212 333 2211",
                            Email = "hello@asianflavors.com",
                            Aciklama = "Cin, Japon ve Kore mutfaklarindan ozel tarifler",
                            ResimUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800"
                        },
                        new Restoran
                        {
                            Ad = "Osmanli Tatli Dunyasi",
                            Adres = "Fatih Merkez, Istanbul",
                            Telefon = "+90 212 111 9999",
                            Email = "info@ottomansweets.com",
                            Aciklama = "Geleneksel Turk ve Osmanli tatlilari ozel tariflerle",
                            ResimUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=800"
                        }
                    };

                    db.Restoranlar.AddRange(restaurants);
                    db.SaveChanges();
                }

                // Add sample foods
                if (!db.Yemekler.Any())
                {
                    var restaurants = db.Restoranlar.ToList();
                    var foods = new List<Yemek>();

                    // Foods for Turkish Restaurant
                    if (restaurants.Count > 0)
                    {
                        var turkishRestaurant = restaurants[0];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Adana Kebap",
                                Aciklama = "Ozel baharatlarla hazirlanan el yapimi Adana kebabi",
                                Fiyat = 45.50m,
                                ResimUrl = "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=600",
                                RestoranId = turkishRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Lahmacun",
                                Aciklama = "Ince hamur uzerinde kiyma ve sebzelerle",
                                Fiyat = 32.00m,
                                ResimUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600",
                                RestoranId = turkishRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Iskender Kebap",
                                Aciklama = "Tereyagi ve yogurtla servis edilen doner kebap",
                                Fiyat = 28.75m,
                                ResimUrl = "https://images.unsplash.com/photo-1571197119282-7c4da777c8b4?w=600",
                                RestoranId = turkishRestaurant.RestoranId
                            }
                        });
                    }

                    // Foods for Pizza Restaurant
                    if (restaurants.Count > 1)
                    {
                        var pizzaRestaurant = restaurants[1];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Pizza Margarita",
                                Aciklama = "Domates sosu, mozzarella peyniri ve feslegenle",
                                Fiyat = 38.00m,
                                ResimUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600",
                                RestoranId = pizzaRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Pizza Pepperoni",
                                Aciklama = "Pepperoni dilimi ve bol mozzarella peyniri ile",
                                Fiyat = 42.50m,
                                ResimUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=600",
                                RestoranId = pizzaRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Pizza Karışık",
                                Aciklama = "Her turlu malzeme ile ozel karisik pizza",
                                Fiyat = 35.75m,
                                ResimUrl = "https://images.unsplash.com/photo-1571407970349-bc81e7e96d47?w=600",
                                RestoranId = pizzaRestaurant.RestoranId
                            }
                        });
                    }

                    // Foods for Burger Restaurant
                    if (restaurants.Count > 2)
                    {
                        var burgerRestaurant = restaurants[2];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Klasik Burger",
                                Aciklama = "Dana eti kofte, marul, domates ve ozel sos ile",
                                Fiyat = 29.90m,
                                ResimUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600",
                                RestoranId = burgerRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Tavuk Burger",
                                Aciklama = "Izgara tavuk gogsu ile hafif ve lezzetli",
                                Fiyat = 27.50m,
                                ResimUrl = "https://images.unsplash.com/photo-1553979459-d2229ba7433a?w=600",
                                RestoranId = burgerRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Cift Katli Burger",
                                Aciklama = "Iki kat dana eti ve cift peynir ile",
                                Fiyat = 33.25m,
                                ResimUrl = "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=600",
                                RestoranId = burgerRestaurant.RestoranId
                            }
                        });
                    }

                    // Foods for Seafood Restaurant
                    if (restaurants.Count > 3)
                    {
                        var seafoodRestaurant = restaurants[3];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Izgara Levrek",
                                Aciklama = "Taze levrek balik soguk mezeler ile servis edilir",
                                Fiyat = 65.00m,
                                ResimUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600",
                                RestoranId = seafoodRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Karides Tava",
                                Aciklama = "Terayagi ve sarimsak ile karides",
                                Fiyat = 58.75m,
                                ResimUrl = "https://images.unsplash.com/photo-1559847844-5315695dadae?w=600",
                                RestoranId = seafoodRestaurant.RestoranId
                            }
                        });
                    }

                    // Foods for Asian Restaurant
                    if (restaurants.Count > 4)
                    {
                        var asianRestaurant = restaurants[4];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Sushi Tabagi",
                                Aciklama = "Karisik sushi cesitleri ile ozel tabak",
                                Fiyat = 41.50m,
                                ResimUrl = "https://images.unsplash.com/photo-1591814468924-caf88d1232e1?w=600",
                                RestoranId = asianRestaurant.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Pad Thai",
                                Aciklama = "Tayland usulu pirinc eriste sebzeler ile",
                                Fiyat = 39.25m,
                                ResimUrl = "https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=600",
                                RestoranId = asianRestaurant.RestoranId
                            }
                        });
                    }

                    // Foods for Dessert Cafe
                    if (restaurants.Count > 5)
                    {
                        var dessertCafe = restaurants[5];
                        foods.AddRange(new[]
                        {
                            new Yemek
                            {
                                Ad = "Baklava",
                                Aciklama = "Geleneksel Turk baklavasi Antep fistigi ile",
                                Fiyat = 18.50m,
                                ResimUrl = "https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=600",
                                RestoranId = dessertCafe.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Kunefe",
                                Aciklama = "Sicak peynirli kunefe kaymak ile servis edilir",
                                Fiyat = 22.00m,
                                ResimUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600",
                                RestoranId = dessertCafe.RestoranId
                            },
                            new Yemek
                            {
                                Ad = "Sutlac",
                                Aciklama = "Firinda pisirilen geleneksel Turk sutlaci",
                                Fiyat = 25.75m,
                                ResimUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600",
                                RestoranId = dessertCafe.RestoranId
                            }
                        });
                    }

                    db.Yemekler.AddRange(foods);
                    db.SaveChanges();
                }

                // Add sample customers
                if (!db.Musteriler.Any())
                {
                    var customers = new List<Musteri>
                    {
                        new Musteri
                        {
                            Ad = "Ahmet",
                            Soyad = "Yilmaz",
                            Email = "ahmet@example.com",
                            Sifre = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("123456salt")),
                            Telefon = "+90 533 123 4567",
                            Adres = "Kadikoy, Istanbul",
                            KayitTarihi = DateTime.Now.AddDays(-30)
                        },
                        new Musteri
                        {
                            Ad = "Fatma",
                            Soyad = "Demir",
                            Email = "fatma@example.com",
                            Sifre = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("123456salt")),
                            Telefon = "+90 534 987 6543",
                            Adres = "Besiktas, Istanbul",
                            KayitTarihi = DateTime.Now.AddDays(-15)
                        }
                    };

                    db.Musteriler.AddRange(customers);
                    db.SaveChanges();
                }

                TempData["Message"] = "Ornek veriler basariyla eklendi! Artik sistemi kullanabilirsiniz.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Veri eklerken bir hata olustu: {ex.Message}";
                return View();
            }
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