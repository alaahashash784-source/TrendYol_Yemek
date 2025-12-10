USE TrendYolYemekDB;
GO

-- Clear existing data
DELETE FROM Sepetler;
DELETE FROM Yemekler;
DELETE FROM Faturalar;
DELETE FROM OrderPages;
DELETE FROM Musteriler;
DELETE FROM Restoranlar;
GO

-- Insert Turkish Restaurants
INSERT INTO Restoranlar (Ad, Adres, Telefon, Email, Aciklama, ResimUrl) VALUES
('Kebapci Iskender', 'Bursa, Turkiye', '+90 212 123 4567', 'info@iskender.com', 'Meshur Iskender kebap ve Turk mutfagi', 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800'),
('Pizza Milano', 'Istanbul, Turkiye', '+90 216 987 6543', 'siparis@pizzamilano.com', 'En lezzetli Italyan pizzalari', 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800'),
('Burger House', 'Izmir, Turkiye', '+90 212 555 0123', 'info@burgerhouse.com', 'Taze malzemelerle hazirlanan burgerler', 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=800'),
('Deniz Urunleri', 'Antalya, Turkiye', '+90 212 444 7890', 'info@denizurunleri.com', 'Her gun taze balik ve deniz urunleri', 'https://images.unsplash.com/photo-1559339352-11d035aa65de?w=800'),
('Asya Mutfagi', 'Ankara, Turkiye', '+90 212 333 2211', 'merhaba@asyamutfagi.com', 'Otantik Uzak Dogu lezzetleri', 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800'),
('Tatli Dunyasi', 'Istanbul, Turkiye', '+90 212 111 9999', 'info@tatlidunyasi.com', 'Geleneksel Turk tatlilari ve pastalar', 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=800');

-- Get Restaurant IDs
DECLARE @KebapId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Kebapci Iskender');
DECLARE @PizzaId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Pizza Milano');
DECLARE @BurgerId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Burger House');
DECLARE @BalikId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Deniz Urunleri');
DECLARE @AsyaId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Asya Mutfagi');
DECLARE @TatliId INT = (SELECT RestoranId FROM Restoranlar WHERE Ad = 'Tatli Dunyasi');

-- Insert Turkish Foods
INSERT INTO Yemekler (Ad, Aciklama, Fiyat, ResimUrl, RestoranId) VALUES
-- Kebapci
('Iskender Kebap', 'Yogurtlu Iskender kebap, tereyagi ve pide', 85.50, 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=600', @KebapId),
('Karisik Izgara', 'Tavuk, kofte, kebap karisimi', 95.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @KebapId),
('Adana Kebap', 'Aci kiyma kebap, salata ve pilav', 75.00, 'https://images.unsplash.com/photo-1571197119282-7c4da777c8b4?w=600', @KebapId),

-- Pizza
('Margherita Pizza', 'Klasik domates, mozzarella ve fesleyen', 65.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @PizzaId),
('Sucuklu Pizza', 'Turk sucugu ve kasar peyniri', 75.00, 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=600', @PizzaId),
('Vejeteryan Pizza', 'Taze sebzeler ve peynir', 60.00, 'https://images.unsplash.com/photo-1571407970349-bc81e7e96d47?w=600', @PizzaId),

-- Burger
('Klasik Burger', 'Dana koftesi, marul, domates, ozel sos', 55.00, 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600', @BurgerId),
('Tavuk Burger', 'Cıtır tavuk, tursu, mayonez', 50.00, 'https://images.unsplash.com/photo-1553979459-d2229ba7433a?w=600', @BurgerId),
('Duble Burger', 'Cift kat kofte, kasar peyniri', 70.00, 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=600', @BurgerId),

-- Balik
('Izgara Levrek', 'Taze Akdeniz levrek, salata', 120.00, 'https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600', @BalikId),
('Karides Tava', 'Tereyaginda karides, pilav', 110.00, 'https://images.unsplash.com/photo-1559847844-5315695dadae?w=600', @BalikId),

-- Asya
('Pad Thai', 'Tayland tarzı pirinc eriste', 80.00, 'https://images.unsplash.com/photo-1591814468924-caf88d1232e1?w=600', @AsyaId),
('Sushi Tabagi', 'Karisik sushi cesitleri', 90.00, 'https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=600', @AsyaId),

-- Tatli
('Fistikli Baklava', 'El acmasi fistikli baklava', 45.00, 'https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=600', @TatliId),
('Kunefe', 'Sicak tel kadayif, peynir', 50.00, 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600', @TatliId),
('Sutlac', 'Firinda sutlac', 35.00, 'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600', @TatliId);

-- Insert Turkish Customers
INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi) VALUES
('Ahmet', 'Yilmaz', 'ahmet@example.com', 'sifre123', '+90 533 123 4567', 'Istanbul, Turkiye', GETDATE()),
('Ayse', 'Demir', 'ayse@example.com', 'sifre123', '+90 534 987 6543', 'Ankara, Turkiye', GETDATE());

PRINT 'Turkce veri basariyla eklendi!';
