# PROJE RAPORU - TrendYol Yemek Sistemi
## ASP.NET MVC ile Online Yemek Sipariş Uygulaması

---

# İÇİNDEKİLER
1. [Proje Tanıtımı](#1-proje-tanıtımı)
2. [Kullanılan Teknolojiler](#2-kullanılan-teknolojiler)
3. [Veritabanı Yapısı](#3-veritabanı-yapısı)
4. [Kimlik Doğrulama ve Giriş Sistemi](#4-kimlik-doğrulama-ve-giriş-sistemi)
5. [Sepet ve Sipariş Sistemi](#5-sepet-ve-sipariş-sistemi)
6. [Ödeme Sistemi](#6-ödeme-sistemi)
7. [Restoran Yönetim Sistemi](#7-restoran-yönetim-sistemi)
8. [Güvenlik ve Koruma](#8-güvenlik-ve-koruma)
9. [Filtreleme ve Arama](#9-filtreleme-ve-arama)
10. [Sayfa Yönlendirme (Routing)](#10-sayfa-yönlendirme-routing)
11. [Sonuç](#11-sonuç)

---

# 1. Proje Tanıtımı

## 1.1 Proje Açıklaması
**TrendYol Yemek**, ASP.NET MVC 5 ve Entity Framework 6 kullanılarak geliştirilmiş kapsamlı bir online yemek sipariş sistemidir. Sistem üç farklı kullanıcı seviyesi sunar:

| Kullanıcı Tipi | Yetkiler |
|----------------|----------|
| **Müşteri** | Restoran görüntüleme, yemek siparişi, sipariş takibi |
| **Restoran Yöneticisi** | Yemek yönetimi, sipariş görüntüleme, müşteri yönetimi |
| **Genel Yönetici (Admin)** | Tüm restoranlar, kullanıcılar ve sistem yönetimi |

## 1.2 Ana Özellikler
-  Güvenli giriş ve kayıt sistemi
-  Session tabanlı akıllı sepet
-  Çoklu ödeme seçenekleri
-  Her restoran için yönetim paneli
-  Gelişmiş filtreleme (fiyat, hazırlanma süresi, kategori)
-  PBKDF2 ile şifre şifreleme
-  CSRF saldırılarına karşı koruma

---

# 2. Kullanılan Teknolojiler

## 2.1 Backend (Sunucu Tarafı)
```
├── ASP.NET MVC 5          → Ana framework
├── Entity Framework 6     → Veritabanı ORM
├── C# 7.3                 → Programlama dili
└── SQL Server             → Veritabanı
```

## 2.2 Frontend (İstemci Tarafı)
```
├── Razor Views (.cshtml)  → View motoru
├── Bootstrap 5            → CSS framework
├── jQuery                 → JavaScript kütüphanesi
└── Font Awesome          → İkonlar
```

## 2.3 Güvenlik
```
├── PBKDF2                 → Şifre şifreleme
├── AntiForgeryToken       → CSRF koruması
├── Session-based Auth     → Oturum tabanlı kimlik doğrulama
└── Custom Filters         → Özel yetki filtreleri
```

---

# 3. Veritabanı Yapısı

## 3.1 Ana Tablolar

### Müşteriler Tablosu (Musteriler)
```sql
CREATE TABLE Musteriler (
    MusteriId INT PRIMARY KEY IDENTITY,
    Ad NVARCHAR(50) NOT NULL,           -- İsim
    Soyad NVARCHAR(50) NOT NULL,        -- Soyisim
    Email NVARCHAR(100) UNIQUE NOT NULL, -- E-posta
    Sifre NVARCHAR(MAX) NOT NULL,       -- Şifrelenmiş şifre
    Salt NVARCHAR(MAX),                 -- Şifreleme için Salt
    Telefon NVARCHAR(20),               -- Telefon
    Adres NVARCHAR(500),                -- Adres
    IsAdmin BIT DEFAULT 0,              -- Admin mi?
    IsRestoranAdmin BIT DEFAULT 0,      -- Restoran yöneticisi mi?
    RestoranId INT NULL,                -- Bağlı restoran
    KayitTarihi DATETIME DEFAULT GETDATE()
);
```

### Restoranlar Tablosu (Restoranlar)
```sql
CREATE TABLE Restoranlar (
    RestoranId INT PRIMARY KEY IDENTITY,
    Ad NVARCHAR(100) NOT NULL,          -- Restoran adı
    Adres NVARCHAR(500),                -- Adres
    Telefon NVARCHAR(20),               -- Telefon
    Email NVARCHAR(100),                -- E-posta
    ResimUrl NVARCHAR(500),             -- Restoran resmi
    Puan DECIMAL(3,2) DEFAULT 4.5       -- Değerlendirme puanı
);
```

### Yemekler Tablosu (Yemekler)
```sql
CREATE TABLE Yemekler (
    YemekId INT PRIMARY KEY IDENTITY,
    Ad NVARCHAR(100) NOT NULL,          -- Yemek adı
    Aciklama NVARCHAR(500),             -- Açıklama
    Fiyat DECIMAL(10,2) NOT NULL,       -- Fiyat
    ResimUrl NVARCHAR(500),             -- Yemek resmi
    HazirlanmaSuresi INT DEFAULT 15,    -- Hazırlanma süresi (dakika)
    Kategori NVARCHAR(50),              -- Kategori (Pizza, Kebap, vb.)
    Stok INT DEFAULT 100,               -- Mevcut stok
    StokAktif BIT DEFAULT 1,            -- Satışa açık mı?
    RestoranId INT FOREIGN KEY          -- Sahibi restoran
);
```

### Siparişler Tablosu (OrderPages)
```sql
CREATE TABLE OrderPages (
    OrderId INT PRIMARY KEY IDENTITY,
    MusteriId INT FOREIGN KEY,          -- Müşteri
    ToplamTutar DECIMAL(10,2),          -- Toplam tutar
    TeslimatAdresi NVARCHAR(500),       -- Teslimat adresi
    OdemeYontemi NVARCHAR(50),          -- Ödeme yöntemi
    SiparisDurumu NVARCHAR(50),         -- Sipariş durumu
    SiparisTarihi DATETIME,             -- Sipariş tarihi
    TahminiHazirlanmaSuresi INT         -- Tahmini hazırlanma süresi
);
```

## 3.2 Tablolar Arası İlişkiler
```
Musteriler (1) ←→ (N) OrderPages      Bir müşterinin birden fazla siparişi olabilir
Restoranlar (1) ←→ (N) Yemekler       Bir restoranın birden fazla yemeği olabilir
Musteriler (1) ←→ (N) Sepetler        Bir müşterinin sepeti vardır
Yemekler (1) ←→ (N) Sepetler          Bir yemek birden fazla sepette olabilir
```

---

# 4. Kimlik Doğrulama ve Giriş Sistemi

## 4.1 Kayıt (Register)

### Adımlar:
```
1. Kullanıcı kayıt formunu doldurur
2. Sistem veri doğrulaması yapar (Model Validation)
3. E-postanın daha önce kayıtlı olmadığını kontrol eder
4. Şifreyi PBKDF2 ile şifreler
5. Verileri veritabanına kaydeder
6. Kullanıcı için Session oluşturur
7. Ana sayfaya yönlendirir
```

### Şifreleme Kodu (SecurityHelper.cs):
```csharp
public static string HashPassword(string password, out string salt)
{
    // Rastgele Salt oluştur (16 byte)
    byte[] saltBytes = new byte[16];
    using (var rng = new RNGCryptoServiceProvider())
    {
        rng.GetBytes(saltBytes);
    }
    salt = Convert.ToBase64String(saltBytes);

    // PBKDF2 ile şifreyi şifrele
    using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
    {
        byte[] hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }
}
```

## 4.2 Giriş (Login)

### Adımlar:
```
1. Kullanıcı e-posta ve şifre girer
2. Sistem e-posta ile kullanıcıyı arar
3. Kayıtlı Salt değerini alır
4. Girilen şifreyi aynı Salt ile şifreler
5. Sonucu kayıtlı şifre ile karşılaştırır
6. Eşleşirse: Session oluşturur
7. Kullanıcı tipine göre yönlendirir
```

### Doğrulama Kodu:
```csharp
public static bool VerifyPassword(string password, string storedHash, string salt)
{
    byte[] saltBytes = Convert.FromBase64String(salt);
    
    using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
    {
        byte[] hash = pbkdf2.GetBytes(32);
        string inputHash = Convert.ToBase64String(hash);
        return inputHash == storedHash;
    }
}
```

## 4.3 Session Yapısı
```csharp
// Başarılı giriş sonrasında:
Session["MusteriId"] = musteri.MusteriId;    // Kullanıcı ID
Session["MusteriAdi"] = musteri.Ad;          // Kullanıcı adı
Session["IsAdmin"] = musteri.IsAdmin;        // Admin mi?
Session["IsRestoranAdmin"] = musteri.IsRestoranAdmin; // Restoran yöneticisi mi?
Session["RestoranId"] = musteri.RestoranId;  // Restoran ID (varsa)
```

---

# 5. Sepet ve Sipariş Sistemi

## 5.1 Sepete Ürün Ekleme

### Yol: `/OrderPage/AddToCart?yemekId=5&miktar=2`

### Adımlar:
```
1. URL'den yemek ID ve miktarı al
2. Veritabanından yemek bilgilerini getir
3. Session'dan mevcut sepeti al
4. Kontrol et: Ürün sepette var mı?
   - Evet: Sadece miktarı artır
   - Hayır: Yeni ürün olarak ekle
5. Sepeti Session'a kaydet
6. Sepet sayfasına yönlendir
```

### Ekleme Kodu:
```csharp
public ActionResult AddToCart(int yemekId, int miktar = 1)
{
    // Yemek bilgilerini getir
    var yemek = db.Yemekler.Include(y => y.Restoran)
                           .FirstOrDefault(y => y.YemekId == yemekId);
    
    if (yemek != null)
    {
        var cartItems = GetCartItems(); // Session'dan sepeti al
        var existingItem = cartItems.FirstOrDefault(x => x.YemekId == yemekId);

        if (existingItem != null)
        {
            // Ürün var - miktarı artır
            existingItem.Miktar += miktar;
        }
        else
        {
            // Yeni ürün - ekle
            cartItems.Add(new CartItemViewModel
            {
                YemekId = yemek.YemekId,
                YemekAdi = yemek.Ad,
                Fiyat = yemek.Fiyat,
                Miktar = miktar,
                ResimUrl = yemek.ResimUrl,
                RestoranAdi = yemek.Restoran?.Ad
            });
        }

        SaveCartItems(cartItems); // Session'a kaydet
    }
    return RedirectToAction("Index");
}
```

## 5.2 Session'da Sepet Yapısı
```csharp
// Sepet Session'da List olarak saklanır
Session["Cart"] = List<CartItemViewModel>
{
    { YemekId=1, YemekAdi="Adana Kebap", Fiyat=85.00, Miktar=2 },
    { YemekId=5, YemekAdi="Pizza", Fiyat=120.00, Miktar=1 },
    ...
}
```

## 5.3 Sepet Görüntüleme
```
Yol: /OrderPage/Index

Görüntülenen bilgiler:
├── Ürün listesi (resim, fiyat, miktar)
├── Miktar değiştirme (+/-)
├── Ürün silme
├── Ara toplam
├── KDV (%8)
└── "Siparişi Tamamla" butonu
```

---

# 6. Ödeme Sistemi

## 6.1 Checkout Sayfası

### Yol: `/OrderPage/Checkout`

### Gerekli Bilgiler:
```
├── Ad Soyad (AdSoyad)
├── Telefon (Telefon)
├── Teslimat Adresi (Adres)
└── Ödeme Yöntemi (OdemeYontemi)
```

## 6.2 Mevcut Ödeme Yöntemleri

| Yöntem | Açıklama | Kod |
|--------|----------|-----|
| **Kapıda Ödeme** | Teslimatta ödeme | `"Kapida Odeme"` |
| **Kredi Kartı** | Kredi kartı ile | `"Kredi Karti"` |
| **Online Ödeme** | Online ödeme | `"Online"` |

## 6.3 Sipariş İşleme (ProcessCheckout)

### Adımlar:
```
1. Form verilerini al (POST)
2. AntiForgeryToken doğrula (CSRF koruması)
3. Session'dan sepeti al
4. Toplam tutarı hesapla (+ %8 KDV)
5. Giriş durumunu kontrol et:
   - Giriş yapmış: MusteriId kullan
   - Misafir: Geçici hesap oluştur
6. OrderPages tablosunda sipariş kaydı oluştur
7. Seçilen ödeme yöntemini kaydet
8. Sepeti temizle
9. Onay sayfasına yönlendir
```

### Ödeme Yöntemi Nasıl Kaydedilir:
```csharp
var order = new OrderPage
{
    MusteriId = musteriId,
    TotalPrice = totalPrice,
    DeliveryAddress = model.Adres,
    PaymentMethod = model.OdemeYontemi,  // ← Kullanıcı seçimi burada kaydedilir
    OrderDate = DateTime.Now,
    OrderStatus = "Onaylandi"
};

db.OrderPages.Add(order);
db.SaveChanges();
```

## 6.4 Ödeme Yöntemi Doğrulaması
Mevcut sistem, kullanıcının seçtiği ödeme yöntemini kabul eder ve veritabanına kaydeder. Ödeme yöntemi, form üzerinden `OdemeYontemi` alanı ile iletilir.

---

# 7. Restoran Yönetim Sistemi

## 7.1 Restoran Yönetici Paneli

### Yol: `/RestaurantAdmin/Dashboard`

### Özellikler:
```
├── Restoran istatistikleri (siparişler, gelir, yemekler)
├── Son siparişler listesi
├── Müşteri listesi
├── Yemek yönetimi (ekleme/düzenleme/silme)
└── Satış raporları
```

## 7.2 Restoran Yöneticisi Filtresi

### Dosya: `Filters/RestaurantAdminFilter.cs`
```csharp
public class AnyRestaurantAdminFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session;
        
        // Kontrol: Restoran yöneticisi veya ana admin mi?
        bool isRestoranAdmin = session["IsRestoranAdmin"] != null 
                               && (bool)session["IsRestoranAdmin"];
        bool isMainAdmin = session["IsAdmin"] != null 
                           && (bool)session["IsAdmin"];
        
        if (!isRestoranAdmin && !isMainAdmin)
        {
            // Yetkisiz - giriş sayfasına yönlendir
            filterContext.Result = new RedirectResult("/Account/Login");
        }
    }
}
```

## 7.3 Stok Yönetimi
```csharp
// Yemek modelinde:
public int Stok { get; set; } = 100;       // Mevcut stok
public bool StokAktif { get; set; } = true; // Satışa açık mı?
```

---

# 8. Güvenlik ve Koruma

## 8.1 Şifre Şifreleme

### Algoritma: PBKDF2 (Password-Based Key Derivation Function 2)
```
├── Salt: Her kullanıcı için 16 byte rastgele
├── Iterations: 10.000 döngü
├── Hash Length: 32 byte
└── Output: Base64 encoded string
```

### Neden PBKDF2?
- Kasıtlı olarak yavaş (Brute Force saldırılarını önler)
- Her kullanıcı için farklı Salt (Rainbow Table'ı önler)
- NIST tarafından onaylı

## 8.2 CSRF Koruması (Cross-Site Request Forgery)

### View'da:
```html
@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()  <!-- Gizli token oluşturur -->
    ...
}
```

### Controller'da:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]  // Token'ı doğrular
public ActionResult ProcessCheckout(CheckoutViewModel model)
{
    ...
}
```

## 8.3 Yetkilendirme Filtreleri

### AdminAuthorizationFilter:
```csharp
// Sadece ana admin'e izin verir
if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
{
    filterContext.Result = new HttpStatusCodeResult(403);
}
```

### AnyRestaurantAdminFilter:
```csharp
// Restoran yöneticisi veya ana admin'e izin verir
if (!isRestoranAdmin && !isMainAdmin)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
```

## 8.4 Girdi Doğrulama (Input Validation)

### Data Annotations ile:
```csharp
[Required(ErrorMessage = "Email gerekli")]
[EmailAddress(ErrorMessage = "Gecerli email giriniz")]
public string Email { get; set; }

[StringLength(100, MinimumLength = 6)]
public string Sifre { get; set; }
```

---

# 9. Filtreleme ve Arama

## 9.1 Restoran Filtreleme

### Yol: `/Restaurant?kategori=Pizza&minFiyat=50&maxFiyat=200`

### Mevcut Filtreler:
| Filtre | Açıklama | Değişken |
|--------|----------|----------|
| **Kategori** | Yemek türü | `kategori` |
| **Min Fiyat** | Minimum fiyat | `minFiyat` |
| **Max Fiyat** | Maksimum fiyat | `maxFiyat` |
| **Hazırlanma Süresi** | Maksimum süre (dakika) | `maxHazirlanmaSuresi` |
| **Sıralama** | Sıralama yöntemi | `siralama` |

### Filtreleme Kodu:
```csharp
public ActionResult Index(string kategori, decimal? minFiyat, 
                          decimal? maxFiyat, int? maxHazirlanmaSuresi)
{
    var restaurants = db.Restoranlar.AsQueryable();
    
    // Kategoriye göre filtrele
    if (!string.IsNullOrEmpty(kategori))
    {
        var restoranIds = db.Yemekler
            .Where(y => y.Kategori.Contains(kategori))
            .Select(y => y.RestoranId)
            .Distinct()
            .ToList();
        restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
    }
    
    // Fiyata göre filtrele
    if (minFiyat.HasValue)
    {
        var restoranIds = db.Yemekler
            .Where(y => y.Fiyat >= minFiyat.Value)
            .Select(y => y.RestoranId)
            .Distinct()
            .ToList();
        restaurants = restaurants.Where(r => restoranIds.Contains(r.RestoranId));
    }
    
    return View(restaurants.ToList());
}
```

## 9.2 Sıralama Seçenekleri
```csharp
switch (siralama)
{
    case "fiyat_artan":
        // Fiyat: Düşükten yükseğe
        break;
    case "fiyat_azalan":
        // Fiyat: Yüksekten düşüğe
        break;
    case "hizli":
        // En hızlı hazırlanan
        break;
    case "cok_yemek":
        // En çok çeşit
        break;
}
```

---

# 10. Sayfa Yönlendirme (Routing)

## 10.1 Temel Routes

### Dosya: `App_Start/RouteConfig.cs`
```csharp
routes.MapRoute(
    name: "Default",
    url: "{controller}/{action}/{id}",
    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
);
```

## 10.2 Sayfa Haritası

### Genel Sayfalar:
| Yol | Controller | Action | Açıklama |
|-----|------------|--------|----------|
| `/` | Home | Index | Ana sayfa |
| `/Restaurant` | Restaurant | Index | Restoran listesi |
| `/Food` | Food | Index | Yemek listesi |
| `/Food/Details/5` | Food | Details | Yemek detayları |

### Hesap Sayfaları:
| Yol | Controller | Action | Açıklama |
|-----|------------|--------|----------|
| `/Account/Login` | Account | Login | Giriş |
| `/Account/Register` | Account | Register | Kayıt |
| `/Account/Logout` | Account | Logout | Çıkış |

### Sepet Sayfaları:
| Yol | Controller | Action | Açıklama |
|-----|------------|--------|----------|
| `/OrderPage` | OrderPage | Index | Sepet görüntüleme |
| `/OrderPage/AddToCart?yemekId=5` | OrderPage | AddToCart | Sepete ekleme |
| `/OrderPage/Checkout` | OrderPage | Checkout | Siparişi tamamla |

### Yönetici Sayfaları:
| Yol | Controller | Action | Açıklama |
|-----|------------|--------|----------|
| `/Admin` | Admin | Index | Admin paneli |
| `/RestaurantAdmin/Dashboard` | RestaurantAdmin | Dashboard | Restoran paneli |
| `/RestaurantAdmin/Siparisler` | RestaurantAdmin | Siparisler | Siparişler |
| `/RestaurantAdmin/Musteriler` | RestaurantAdmin | Musteriler | Müşteriler |

## 10.3 View'larda Bağlantı Oluşturma

### Html.ActionLink ile:
```html
@Html.ActionLink("Yemekler", "Index", "Food")
<!-- Çıktı: <a href="/Food">Yemekler</a> -->
```

### Url.Action ile:
```html
<a href="@Url.Action("Details", "Food", new { id = item.YemekId })">Detaylar</a>
<!-- Çıktı: <a href="/Food/Details/5">Detaylar</a> -->
```

### Form ile:
```html
@using (Html.BeginForm("ProcessCheckout", "OrderPage", FormMethod.Post))
{
    @Html.AntiForgeryToken()
    <button type="submit">Sipariş Ver</button>
}
```

---

# 11. Sonuç

## 11.1 Proje Özeti
TrendYol Yemek sistemi, MVC (Model-View-Controller) deseni kullanılarak geliştirilmiştir:

- **Model**: Varlıklar (Musteri, Restoran, Yemek, OrderPage) ve veritabanı işlemleri
- **View**: Veri görüntüleme için Razor sayfaları (.cshtml)
- **Controller**: İş mantığı ve istek işleme

## 11.2 Güçlü Yönler
1. **Yüksek Güvenlik**: PBKDF2 şifreleme + CSRF koruması + Yetkilendirme filtreleri
2. **Temiz Mimari**: Separation of Concerns (Kaygıların Ayrımı)
3. **Kolay Bakım**: Düzenli ve yorumlu kod
4. **İyi Kullanıcı Deneyimi**: Bootstrap 5 ile responsive arayüz

## 11.3 Kullanılan Teknolojiler
- ASP.NET MVC 5
- Entity Framework 6 (Code First)
- SQL Server
- Bootstrap 5
- jQuery
- PBKDF2 Şifreleme

---    


**Hazırlayan:** ELE HASHASH
**Tarih:** 15 Aralık 2025
**Proje:** TrendYol Yemek - Online Yemek Sipariş Sistemi
