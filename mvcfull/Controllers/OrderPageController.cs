// كنترولر السلة والطلبات - إدارة عملية الشراء
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mvc_full.Models;

namespace mvc_full.Controllers
{
    // كنترولر السلة والطلبات - إدارة عملية الشراء
    // TODO: إضافة خاصية حفظ السلة في قاعدة البيانات لاحقاً
    public class OrderPageController : Controller
    {
        // الاتصال بالقاعدة
        private ABCDbContext db = new ABCDbContext();

        // عرض سلة التسوق
        public ActionResult Index()
        {
            var cartItems = GetCartItems();
            ViewBag.CartCount = cartItems.Sum(x => x.Miktar);
            ViewBag.CartTotal = cartItems.Sum(x => x.ToplamFiyat);
            return View(cartItems);
        }

        // صفحة إتمام الطلب
        public ActionResult Checkout()
        {
            var cartItems = GetCartItems();
            // التحقق من وجود منتجات في السلة
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetiniz bos. Lutfen sepete urun ekleyin.";
                return RedirectToAction("Index", "Home");
            }

            var araToplam = cartItems.Sum(x => x.ToplamFiyat);
            var kdv = araToplam * 0.08m; // ضريبة 8%
            var teslimatUcreti = 0m; // التوصيل مجاني

            var checkoutModel = new CheckoutViewModel
            {
                SepetUrunleri = cartItems,
                AraToplam = araToplam,
                KDV = kdv,
                TeslimatUcreti = teslimatUcreti,
                GenelToplam = araToplam + kdv + teslimatUcreti
            };

            // ملء بيانات المستخدم إن كان مسجل الدخول
            if (Session["MusteriId"] != null)
            {
                var musteriId = Convert.ToInt32(Session["MusteriId"]);
                var musteri = db.Musteriler.Find(musteriId);
                if (musteri != null)
                {
                    checkoutModel.AdSoyad = musteri.Ad + " " + musteri.Soyad;
                    checkoutModel.Telefon = musteri.Telefon;
                    checkoutModel.Adres = musteri.Adres;
                }
            }

            return View(checkoutModel);
        }

        // معالجة الطلب - إنشاء الطلب في قاعدة البيانات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessCheckout(CheckoutViewModel model)
        {
            try
            {
                var cartItems = GetCartItems();
                
                // حساب السعر مع الضريبة
                decimal totalPrice = cartItems.Any() 
                    ? cartItems.Sum(x => x.ToplamFiyat) * 1.08m 
                    : 0m;

                if (totalPrice == 0)
                {
                    TempData["Error"] = "Sepetiniz bos. Lutfen sepete urun ekleyin.";
                    return RedirectToAction("Index", "Food");
                }

                // جلب أو إنشاء حساب للطلب
                int musteriId;
                if (Session["MusteriId"] != null)
                {
                    musteriId = Convert.ToInt32(Session["MusteriId"]);
                }
                else
                {
                    // إنشاء حساب مستخدم ضيف للطلبات بدون تسجيل
                    var guestMusteri = db.Musteriler.FirstOrDefault(m => m.Email == "misafir@trendyol.com");
                    if (guestMusteri == null)
                    {
                        guestMusteri = new Musteri
                        {
                            Ad = "Misafir",
                            Soyad = "Kullanici",
                            Email = "misafir@trendyol.com",
                            Sifre = "guest_account_no_login",
                            Telefon = model.Telefon ?? "Belirtilmedi",
                            Adres = model.Adres ?? "Belirtilmedi",
                            KayitTarihi = DateTime.Now
                        };
                        db.Musteriler.Add(guestMusteri);
                        db.SaveChanges();
                    }
                    musteriId = guestMusteri.MusteriId;
                }

                // إنشاء الطلب
                var order = new OrderPage
                {
                    MusteriId = musteriId,
                    TotalPrice = totalPrice,
                    DeliveryAddress = model.Adres ?? "Belirtilmedi",
                    PaymentMethod = model.OdemeYontemi ?? "Kapida Odeme",
                    OrderDate = DateTime.Now,
                    OrderStatus = "Onaylandi"
                };

                db.OrderPages.Add(order);
                db.SaveChanges();

                // تفريغ السلة
                ClearCart();

                TempData["Message"] = "Siparisiz basariyla alindi! Siparis numaraniz: " + order.OrderId;
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                // Get the deepest inner exception for detailed error message
                var innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }
                
                // تسجيل الخطأ
                System.Diagnostics.Debug.WriteLine("Checkout Error: " + innerEx.Message);
                
                TempData["Error"] = "Siparis islenirken bir hata olustu. Lutfen tekrar deneyin.";
                return RedirectToAction("Checkout");
            }
        }

        // إضافة منتج للسلة
        public ActionResult AddToCart(int yemekId, int miktar = 1)
        {
            try
            {
                var yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == yemekId);
                if (yemek != null)
                {
                    var cartItems = GetCartItems();
                    var existingItem = cartItems.FirstOrDefault(x => x.YemekId == yemekId);

                    if (existingItem != null)
                    {
                        // زيادة الكمية
                        existingItem.Miktar += miktar;
                    }
                    else
                    {
                        // إضافة منتج جديد
                        cartItems.Add(new CartItemViewModel
                        {
                            YemekId = yemek.YemekId,
                            YemekAdi = yemek.Ad,
                            Fiyat = yemek.Fiyat,
                            Miktar = miktar,
                            ResimUrl = yemek.ResimUrl,
                            RestoranAdi = yemek.Restoran?.Ad ?? "Bilinmeyen Restoran",
                            RestoranId = yemek.RestoranId
                        });
                    }

                    SaveCartItems(cartItems);
                    TempData["Message"] = $"{yemek.Ad} sepete eklendi!";
                }
                else
                {
                    TempData["Error"] = "Yemek bulunamadi.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sepete eklenirken hata olustu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // إضافة منتج للسلة عبر AJAX
        [HttpPost]
        public JsonResult AddToCartAjax(int yemekId, int miktar = 1)
        {
            try
            {
                var yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == yemekId);
                if (yemek != null)
                {
                    var cartItems = GetCartItems();
                    var existingItem = cartItems.FirstOrDefault(x => x.YemekId == yemekId);

                    if (existingItem != null)
                    {
                        existingItem.Miktar += miktar;
                    }
                    else
                    {
                        cartItems.Add(new CartItemViewModel
                        {
                            YemekId = yemek.YemekId,
                            YemekAdi = yemek.Ad,
                            Fiyat = yemek.Fiyat,
                            Miktar = miktar,
                            ResimUrl = yemek.ResimUrl,
                            RestoranAdi = yemek.Restoran?.Ad ?? "Bilinmeyen Restoran",
                            RestoranId = yemek.RestoranId
                        });
                    }

                    SaveCartItems(cartItems);
                    
                    return Json(new { 
                        success = true, 
                        message = yemek.Ad + " sepete eklendi",
                        cartCount = cartItems.Sum(x => x.Miktar),
                        cartTotal = cartItems.Sum(x => x.ToplamFiyat)
                    }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { success = false, message = "Yemek bulunamadi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // حذف منتج من السلة
        public ActionResult RemoveFromCart(int yemekId)
        {
            try
            {
                var cartItems = GetCartItems();
                var item = cartItems.FirstOrDefault(x => x.YemekId == yemekId);
                if (item != null)
                {
                    cartItems.Remove(item);
                    SaveCartItems(cartItems);
                    TempData["Message"] = $"{item.YemekAdi} sepetten cikarildi.";
                }
                else
                {
                    TempData["Error"] = "Urun sepette bulunamadi.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Urun cikarilirken hata olustu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // تحديث الكمية عبر AJAX
        [HttpPost]
        public JsonResult UpdateQuantity(int yemekId, int miktar)
        {
            try
            {
                if (miktar <= 0)
                {
                    return Json(new { success = false, message = "Miktar en az 1 olmalidir" });
                }

                if (miktar > 100)
                {
                    return Json(new { success = false, message = "Miktar en fazla 100 olabilir" });
                }

                var cartItems = GetCartItems();
                var item = cartItems.FirstOrDefault(x => x.YemekId == yemekId);
                
                if (item != null)
                {
                    item.Miktar = miktar;
                    SaveCartItems(cartItems);
                    
                    return Json(new { 
                        success = true, 
                        itemTotal = item.ToplamFiyat,
                        cartTotal = cartItems.Sum(x => x.ToplamFiyat),
                        cartCount = cartItems.Sum(x => x.Miktar)
                    });
                }
                
                return Json(new { success = false, message = "Urun sepette bulunamadi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        // صفحة تأكيد الطلب
        public ActionResult OrderConfirmation(int id)
        {
            var order = db.OrderPages.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                TempData["Error"] = "Siparis bulunamadi.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        // تفريغ السلة بالكامل
        public ActionResult ClearCartAction()
        {
            ClearCart();
            TempData["Message"] = "Sepet temizlendi.";
            return RedirectToAction("Index");
        }

        // عدد العناصر للعرض في النافبار
        public JsonResult GetCartCount()
        {
            var cartItems = GetCartItems();
            return Json(new { 
                count = cartItems.Sum(x => x.Miktar),
                total = cartItems.Sum(x => x.ToplamFiyat)
            }, JsonRequestBehavior.AllowGet);
        }

        // معاينة السلة
        public ActionResult GetCartPreview()
        {
            var cartItems = GetCartItems();
            return PartialView("_CartPreview", cartItems);
        }

        #region دوال مساعدة
        
        // جلب عناصر السلة من الجلسة
        private List<CartItemViewModel> GetCartItems()
        {
            var cartItems = Session["CartItems"] as List<CartItemViewModel>;
            return cartItems ?? new List<CartItemViewModel>();
        }

        // حفظ السلة في الجلسة
        private void SaveCartItems(List<CartItemViewModel> cartItems)
        {
            Session["CartItems"] = cartItems;
        }

        // تفريغ السلة
        private void ClearCart()
        {
            Session["CartItems"] = null;
        }

        #endregion

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
