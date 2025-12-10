
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
    // كنترولر السلة - يدير كل عمليات الشراء
    public class OrderPageController : Controller
    {
       
        private ABCDbContext db = new ABCDbContext();

        
        // عرض صفحة السلة - تعرض كل المنتجات المضافة
        // الرابط: /OrderPage/Index
       
        public ActionResult Index()
        {
            // جلب عناصر السلة من الـ Session
            var cartItems = GetCartItems();
            // حساب العدد الإجمالي
            ViewBag.CartCount = cartItems.Sum(x => x.Miktar);
            // حساب المبلغ الإجمالي
            ViewBag.CartTotal = cartItems.Sum(x => x.ToplamFiyat);
            // إرسال القائمة للـ View
            return View(cartItems);
        }

        
        // صفحة إتمام الطلب (Checkout) - إدخال بيانات التوصيل
        // الرابط: /OrderPage/Checkout
        public ActionResult Checkout()
        {
            var cartItems = GetCartItems();
            
            // التحقق: السلة فارغة؟
            if (!cartItems.Any())
            {
                TempData["Error"] = "Sepetiniz bos. Lutfen sepete urun ekleyin.";
                return RedirectToAction("Index", "Home");
            }

            // حساب الأسعار
            var araToplam = cartItems.Sum(x => x.ToplamFiyat);  // المجموع الفرعي
            var kdv = araToplam * 0.08m;                         // الضريبة 8%
            var teslimatUcreti = 0m;                             // رسوم التوصيل (مجاني)

            // إعداد نموذج الـ Checkout
            var checkoutModel = new CheckoutViewModel
            {
                SepetUrunleri = cartItems,
                AraToplam = araToplam,
                KDV = kdv,
                TeslimatUcreti = teslimatUcreti,
                GenelToplam = araToplam + kdv + teslimatUcreti   // المجموع الكلي
            };

            // إذا المستخدم مسجل دخول = نملأ بياناته تلقائياً
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

        
        // معالجة الطلب - حفظ الطلب في قاعدة البيانات
        // الرابط: POST /OrderPage/ProcessCheckout
        
        [HttpPost]
        [ValidateAntiForgeryToken]  // حماية من هجمات CSRF
        public ActionResult ProcessCheckout(CheckoutViewModel model)
        {
            try
            {
                var cartItems = GetCartItems();
                
                // حساب السعر النهائي مع الضريبة
                decimal totalPrice = cartItems.Any() 
                    ? cartItems.Sum(x => x.ToplamFiyat) * 1.08m 
                    : 0m;

                // التحقق: السلة فارغة؟
                if (totalPrice == 0)
                {
                    TempData["Error"] = "Sepetiniz bos. Lutfen sepete urun ekleyin.";
                    return RedirectToAction("Index", "Food");
                }

                // تحديد رقم الزبون
                int musteriId;
                if (Session["MusteriId"] != null)
                {
                    // مستخدم مسجل
                    musteriId = Convert.ToInt32(Session["MusteriId"]);
                }
                else
                {
                    // مستخدم ضيف - إنشاء حساب مؤقت
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

                // إنشاء سجل الطلب في قاعدة البيانات
                var order = new OrderPage
                {
                    MusteriId = musteriId,
                    TotalPrice = totalPrice,
                    DeliveryAddress = model.Adres ?? "Belirtilmedi",
                    PaymentMethod = model.OdemeYontemi ?? "Kapida Odeme",
                    OrderDate = DateTime.Now,
                    OrderStatus = "Onaylandi"  // الحالة الأولية
                };

                // حفظ في قاعدة البيانات
                db.OrderPages.Add(order);
                db.SaveChanges();

                // تفريغ السلة بعد الطلب
                ClearCart();

                TempData["Message"] = "Siparisiz basariyla alindi! Siparis numaraniz: " + order.OrderId;
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء
                var innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }
                
                System.Diagnostics.Debug.WriteLine("Checkout Error: " + innerEx.Message);
                TempData["Error"] = "Siparis islenirken bir hata olustu. Lutfen tekrar deneyin.";
                return RedirectToAction("Checkout");
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // إضافة منتج للسلة (عادي - مع إعادة تحميل الصفحة)
        // الرابط: /OrderPage/AddToCart?yemekId=5&miktar=2
        // ═══════════════════════════════════════════════════════════════════
        public ActionResult AddToCart(int yemekId, int miktar = 1)
        {
            try
            {
                // جلب بيانات الطعام من قاعدة البيانات
                var yemek = db.Yemekler.Include(y => y.Restoran).FirstOrDefault(y => y.YemekId == yemekId);
                
                if (yemek != null)
                {
                    // جلب السلة الحالية
                    var cartItems = GetCartItems();
                    // البحث عن المنتج في السلة
                    var existingItem = cartItems.FirstOrDefault(x => x.YemekId == yemekId);

                    if (existingItem != null)
                    {
                        // المنتج موجود = زيادة الكمية فقط
                        existingItem.Miktar += miktar;
                    }
                    else
                    {
                        // المنتج جديد = إضافته للقائمة
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

                    // حفظ السلة في الـ Session
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

       
        // إضافة منتج للسلة عبر AJAX (بدون إعادة تحميل الصفحة)
        // الرابط: POST /OrderPage/AddToCartAjax
        
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
                    
                    // إرجاع JSON للـ JavaScript
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

        // ═══════════════════════════════════════════════════════════════════
        // حذف منتج من السلة
        // الرابط: /OrderPage/RemoveFromCart?yemekId=5
        // ═══════════════════════════════════════════════════════════════════
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
        // الرابط: POST /OrderPage/UpdateQuantity
        
        [HttpPost]
        public JsonResult UpdateQuantity(int yemekId, int miktar)
        {
            try
            {
                // التحقق من صحة الكمية
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

        
        // صفحة تأكيد الطلب - تظهر بعد إتمام الطلب بنجاح
       
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

        
        // جلب عدد العناصر في السلة (للـ Navbar)
        // تُستدعى عبر AJAX لتحديث العداد
        
        public JsonResult GetCartCount()
        {
            var cartItems = GetCartItems();
            return Json(new { 
                count = cartItems.Sum(x => x.Miktar),
                total = cartItems.Sum(x => x.ToplamFiyat)
            }, JsonRequestBehavior.AllowGet);
        }

        
        // معاينة السلة - جزء صغير يظهر في القائمة المنسدلة
        
        public ActionResult GetCartPreview()
        {
            var cartItems = GetCartItems();
            return PartialView("_CartPreview", cartItems);
        }

       
        // دوال مساعدة خاصة - للتعامل مع الـ Session
        
        #region دوال مساعدة للسلة
        
        // جلب عناصر السلة من الـ Session
        private List<CartItemViewModel> GetCartItems()
        {
            // محاولة جلب السلة من الـ Session
            var cartItems = Session["CartItems"] as List<CartItemViewModel>;
            // إذا فارغة = نرجع قائمة جديدة فارغة
            return cartItems ?? new List<CartItemViewModel>();
        }

        // حفظ السلة في الـ Session
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

       
        // تنظيف الموارد عند إغلاق الكنترولر
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();  // إغلاق اتصال قاعدة البيانات
            }
            base.Dispose(disposing);
        }
    }
}
