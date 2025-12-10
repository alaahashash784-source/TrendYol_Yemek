USE TrendYolYemekDB;
GO

-- Insert Restaurants
INSERT INTO Restoranlar (Ad, Adres, Telefon, Email, Aciklama, ResimUrl) VALUES
('Turk Kebap Evi', 'Istanbul, Turkiye', '+90 212 123 4567', 'info@kebap.com', 'Geleneksel kebaplarla otantik Turk mutfagi', 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800'),
('Pizza Cenneti', 'Ankara, Turkiye', '+90 216 987 6543', 'siparisler@pizzacenneti.com', 'Italyan tarifleriyle sehrin en iyi pizzasi', 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800'),
('Burger Bolgesi', 'Izmir, Turkiye', '+90 212 555 0123', 'info@burgerbolgesi.com', 'Lezzetli burgerler ve citir patates kizartmasi', 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=800'),
('Deniz Lezzetleri', 'Antalya, Turkiye', '+90 212 444 7890', 'iletisim@denizlezzetleri.com', 'Her gun taze deniz urunleri', 'https://images.unsplash.com/photo-1559339352-11d035aa65de?w=800'),
('Asya Tatlari', 'Bursa, Turkiye', '+90 212 333 2211', 'merhaba@asyatatlari.com', 'Otantik Asya mutfagi', 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800'),
('Tatli Ruyalar Kafe', 'Istanbul, Turkiye', '+90 212 111 9999', 'info@tatliruyalar.com', 'Geleneksel Osmanli tatlilari', 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=800');

-- Get Restaurant IDs
DECLARE @KebabId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Turk Kebap Evi');
DECLARE @PizzaId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Pizza Cenneti');
DECLARE @BurgerId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Burger Bolgesi');
DECLARE @SeafoodId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Deniz Lezzetleri');
DECLARE @AsianId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Asya Tatlari');
DECLARE @CafeId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Tatli Ruyalar Kafe');

-- Insert Foods
INSERT INTO Yemekler (Ad, Aciklama, Fiyat, ResimUrl, RestoranId) VALUES
-- Kebap Evi
('Adana Kebap', 'Baharatli kiyma kebabi', 45.50, 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=600', @KebabId),
('Karisik Izgara', 'Cesitli izgara etler', 65.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @KebabId),
('Tavuk Sis', 'Izgara tavuk sisleri', 38.75, 'https://images.unsplash.com/photo-1571197119282-7c4da777c8b4?w=600', @KebabId),

-- Pizza Cenneti
('Margherita Pizza', 'Klasik peynir ve domates', 38.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @PizzaId),
('Sucuklu Pizza', 'Sucuk ve peynirli', 42.50, 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=600', @PizzaId),
('Vejetaryen Pizza', 'Taze sebzeler', 35.75, 'https://images.unsplash.com/photo-1571407970349-bc81e7e96d47?w=600', @PizzaId),

-- Burger Bolgesi
('Klasik Burger', 'Dana koftesi ve marulla', 29.90, 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600', @BurgerId),
('Tavuk Burger', 'Citir tavuk burger', 27.50, 'https://images.unsplash.com/photo-1553979459-d2229ba7433a?w=600', @BurgerId),
('Duble Burger', 'Cift katli dana koftesi', 33.25, 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=600', @BurgerId),

-- Deniz Lezzetleri
('Izgara Somon', 'Taze Atlantik somonu', 65.00, 'https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600', @SeafoodId),
('Karidesli Makarna', 'Kremali karidesli makarna', 58.75, 'https://images.unsplash.com/photo-1559847844-5315695dadae?w=600', @SeafoodId),

-- Asya Tatlari
('Pad Thai', 'Tayland usulu pirinc eristesl', 41.50, 'https://images.unsplash.com/photo-1591814468924-caf88d1232e1?w=600', @AsianId),
('Sushi Tabagi', 'Taze sushi tabagi', 39.25, 'https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=600', @AsianId),

-- Tatli Ruyalar Kafe
('Baklava', 'Geleneksel Turk baklavasi', 18.50, 'https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=600', @CafeId),
('Kunefe', 'Sicak peynirli tatli', 22.00, 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600', @CafeId),
('Turk Lokumu', 'Cesitli lokum', 25.75, 'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600', @CafeId);

-- Insert Sample Customers (IsAdmin column added)
-- Note: In production, passwords should be hashed. These are plain text for seed data only.
INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi, IsAdmin) VALUES
('Admin', 'Kullanici', 'admin@example.com', 'admin123', '+90 532 000 0000', 'Istanbul, Turkiye', GETDATE(), 1),
('Ahmet', 'Yilmaz', 'ahmet@example.com', 'sifre123', '+90 533 123 4567', 'Istanbul, Turkiye', GETDATE(), 0),
('Ayse', 'Demir', 'ayse@example.com', 'sifre123', '+90 534 987 6543', 'Ankara, Turkiye', GETDATE(), 0);

PRINT 'Ornek veriler basariyla eklendi!';
