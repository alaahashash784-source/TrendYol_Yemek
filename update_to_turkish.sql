
USE TrendYolYemekDB;
GO

-- Restoranlari guncelle
UPDATE Restoranlar SET 
    Ad = 'Turk Kebap Evi', 
    Adres = 'Istanbul, Turkiye', 
    Aciklama = 'Geleneksel kebaplarla otantik Turk mutfagi'
WHERE Ad = 'Turkish Kebab House';

UPDATE Restoranlar SET 
    Ad = 'Pizza Cenneti', 
    Adres = 'Ankara, Turkiye', 
    Aciklama = 'Italyan tarifleriyle sehrin en iyi pizzasi'
WHERE Ad = 'Pizza Paradise';

UPDATE Restoranlar SET 
    Ad = 'Burger Bolgesi', 
    Adres = 'Izmir, Turkiye', 
    Aciklama = 'Lezzetli burgerler ve citir patates kizartmasi'
WHERE Ad = 'Burger Zone';

UPDATE Restoranlar SET 
    Ad = 'Deniz Lezzetleri', 
    Adres = 'Antalya, Turkiye', 
    Aciklama = 'Her gun taze deniz urunleri'
WHERE Ad = 'Seafood Delight';

UPDATE Restoranlar SET 
    Ad = 'Asya Tatlari', 
    Adres = 'Bursa, Turkiye', 
    Aciklama = 'Otantik Asya mutfagi'
WHERE Ad = 'Asian Flavors';

UPDATE Restoranlar SET 
    Ad = 'Tatli Ruyalar Kafe', 
    Adres = 'Istanbul, Turkiye', 
    Aciklama = 'Geleneksel Osmanli tatlilari'
WHERE Ad = 'Sweet Dreams Cafe';

-- Yemekleri guncelle
-- Kebap Evi
UPDATE Yemekler SET Ad = 'Adana Kebap', Aciklama = 'Baharatli kiyma kebabi' WHERE Ad = 'Adana Kebab';
UPDATE Yemekler SET Ad = 'Karisik Izgara', Aciklama = 'Cesitli izgara etler' WHERE Ad = 'Mixed Grill';
UPDATE Yemekler SET Ad = 'Tavuk Sis', Aciklama = 'Izgara tavuk sisleri' WHERE Ad = 'Chicken Shish';

-- Pizza
UPDATE Yemekler SET Aciklama = 'Klasik peynir ve domates' WHERE Ad = 'Margherita Pizza';
UPDATE Yemekler SET Ad = 'Sucuklu Pizza', Aciklama = 'Sucuk ve peynirli' WHERE Ad = 'Pepperoni Pizza';
UPDATE Yemekler SET Ad = 'Vejetaryen Pizza', Aciklama = 'Taze sebzeler' WHERE Ad = 'Vegetarian Pizza';

-- Burger
UPDATE Yemekler SET Ad = 'Klasik Burger', Aciklama = 'Dana koftesi ve marulla' WHERE Ad = 'Classic Burger';
UPDATE Yemekler SET Ad = 'Tavuk Burger', Aciklama = 'Citir tavuk burger' WHERE Ad = 'Chicken Burger';
UPDATE Yemekler SET Ad = 'Duble Burger', Aciklama = 'Cift katli dana koftesi' WHERE Ad = 'Double Burger';

-- Deniz Urunleri
UPDATE Yemekler SET Ad = 'Izgara Somon', Aciklama = 'Taze Atlantik somonu' WHERE Ad = 'Grilled Salmon';
UPDATE Yemekler SET Ad = 'Karidesli Makarna', Aciklama = 'Kremali karidesli makarna' WHERE Ad = 'Shrimp Pasta';

-- Asya
UPDATE Yemekler SET Aciklama = 'Tayland usulu pirinc eristesi' WHERE Ad = 'Pad Thai';
UPDATE Yemekler SET Ad = 'Sushi Tabagi', Aciklama = 'Taze sushi tabagi' WHERE Ad = 'Sushi Roll';

-- Tatli
UPDATE Yemekler SET Aciklama = 'Geleneksel Turk baklavasi' WHERE Ad = 'Baklava';
UPDATE Yemekler SET Aciklama = 'Sicak peynirli tatli' WHERE Ad = 'Kunefe';
UPDATE Yemekler SET Ad = 'Turk Lokumu', Aciklama = 'Cesitli lokum' WHERE Ad = 'Turkish Delight';

PRINT 'Tum veriler Turkceye guncellendi!';
GO
