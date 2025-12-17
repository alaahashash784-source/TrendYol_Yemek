# TrendYol Yemek - Proje Özet Raporu
## Online Yemek Sipariş Sistemi

---

## 1. Proje Tanımı

TrendYol Yemek, ASP.NET MVC 5 ve Entity Framework 6 kullanılarak geliştirilmiş kapsamlı bir online yemek sipariş sistemidir.

### Kullanıcı Türleri
| Tür | Yetkiler |
|-----|----------|
| **Müşteri** | Restoran görüntüleme, sipariş verme, sipariş takibi |
| **Restoran Yöneticisi** | Yemek yönetimi, sipariş görüntüleme, müşteri listesi |
| **Genel Admin** | Tüm sistem yönetimi |

---

## 2. Teknolojiler

- **Backend:** ASP.NET MVC 5, C#, Entity Framework 6
- **Veritabanı:** SQL Server (TrendYolYemekDB)
- **Frontend:** Razor Views, Bootstrap 5, jQuery
- **Güvenlik:** PBKDF2 şifreleme, CSRF koruması, Session tabanlı kimlik doğrulama

---

## 3. Veritabanı Yapısı

### Ana Tablolar
- **Musteriler:** Kullanıcı bilgileri (ad, email, şifre, salt, telefon, adres, yetki)
- **Restoranlar:** Restoran bilgileri (ad, adres, telefon, puan)
- **Yemekler:** Yemek bilgileri (ad, fiyat, kategori, hazırlanma süresi, stok)
- **OrderPages:** Sipariş bilgileri (müşteri, tutar, adres, ödeme yöntemi, durum)

---

## 4. Sepet Sistemi (Sepetim)

### Çalışma Mantığı
1. Sepet, veritabanında değil **Session**'da saklanır
2. Her yemek `YemekId` ile benzersiz şekilde tanımlanır
3. Aynı yemek eklenirse miktar artırılır
4. Session timeout'ta sepet silinir

### Neden Session?
- Daha hızlı okuma/yazma
- Giriş yapmamış kullanıcılar da kullanabilir
- Veritabanı yükü azalır

---

## 5. Ödeme Sistemi

### Ödeme Yöntemleri
| Yöntem | Açıklama |
|--------|----------|
| **Kapıda Nakit** | Teslimatta nakit ödeme |
| **Kapıda Kart** | Teslimatta kart ile ödeme (VISA, MasterCard, TROY) |
| **Online Ödeme** | İnternet üzerinden ödeme |

### Nasıl Çalışır?
1. Kullanıcı radio button ile yöntem seçer
2. Form POST ile sunucuya gönderilir
3. `OdemeYontemi` değeri doğrudan veritabanına kaydedilir
4. Radio button mantığı gereği sadece bir seçenek işaretlenebilir

---

## 6. Sipariş Takip Sistemi

### Dört Aşama
| Aşama | Süre | Açıklama |
|-------|------|----------|
| **1. Onaylandı** | 10 dk (sabit) | Sipariş alındı |
| **2. Hazırlanıyor** | Değişken | Veritabanından (HazırlanmaSüresi) |
| **3. Yolda** | 35 dk (sabit) | Kurye yola çıktı |
| **4. Teslim Edildi** | - | Sipariş tamamlandı |

### Süre Hesaplama
```
Toplam Süre = 10 dk + Hazırlama Süresi + 35 dk
Tahmini Teslim = Sipariş Saati + Toplam Süre
```

---

## 7. Filtreleme Sistemi

### Filtre Kriterleri
- **Kategori:** Sadece veritabanında mevcut kategoriler gösterilir
- **Min/Max Fiyat:** Fiyat aralığı belirleme
- **Max Süre:** Maksimum hazırlanma süresi
- **Sıralama:** Fiyat artan/azalan, hızlı hazırlanan

### Dinamik Kategori Listesi
Kategoriler artık sabit değil, veritabanındaki gerçek yemek kategorilerinden dinamik olarak çekilir. Boş kategori seçildiğinde "restoran bulunamadı" hatası gösterilmez.

---

## 8. Güvenlik Önlemleri

### Şifre Güvenliği
- **Algoritma:** PBKDF2 (10.000 iterasyon)
- **Salt:** Her kullanıcı için 16 byte rastgele
- **Hash Uzunluğu:** 32 byte (Base64 formatında saklanır)

### CSRF Koruması
- Tüm POST formlarında `@Html.AntiForgeryToken()` kullanılır
- Controller'larda `[ValidateAntiForgeryToken]` attribute'u ile doğrulama yapılır

### Yetkilendirme
- `AdminAuthorizationFilter`: Sadece admin erişimi
- `AnyRestaurantAdminFilter`: Restoran yöneticisi veya admin erişimi

---

## 9. Sayfa Yönlendirme (Routing)

### URL Yapısı
```
/{Controller}/{Action}/{id}
```

### Örnek Yollar
| URL | Açıklama |
|-----|----------|
| `/Restaurant` | Restoran listesi |
| `/Food/Details/5` | 5 numaralı yemek detayı |
| `/OrderPage/AddToCart?yemekId=5` | Sepete ekleme |
| `/OrderPage/Checkout` | Ödeme sayfası |
| `/Account/Orders` | Kullanıcı siparişleri |

---

## 10. Dosya Yapısı

```
mvc full/
├── Controllers/           # İş mantığı
│   ├── HomeController.cs
│   ├── RestaurantController.cs
│   ├── FoodController.cs
│   ├── OrderPageController.cs
│   ├── AccountController.cs
│   └── RestaurantAdminController.cs
├── Models/               # Veri modelleri
│   ├── Musteri.cs
│   ├── Restoran.cs
│   ├── Yemek.cs
│   └── OrderPage.cs
├── Views/                # Kullanıcı arayüzü
├── Helpers/              # Yardımcı sınıflar
│   └── SecurityHelper.cs
└── Filters/              # Yetkilendirme filtreleri
```

---

## 11. Önemli Notlar

1. **Yemek Seçimi:** Her yemek benzersiz `YemekId` ile tanımlanır, karışıklık olmaz
2. **Ödeme:** Radio button ile seçim yapılır, sadece seçilen değer kaydedilir
3. **Kategoriler:** Sadece yemek bulunan kategoriler listelenir
4. **Süre Hesabı:** Otomatik olarak yemek verilerinden hesaplanır

---

**Hazırlayan:** ELE HASHASH
**Tarih:** 17 Aralık 2025
