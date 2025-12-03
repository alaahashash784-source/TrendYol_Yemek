USE TrendYolYemekDB;
GO

-- Insert Restaurants
INSERT INTO Restorans (Ad, Adres, Telefon, Email, Aciklama, ResimUrl) VALUES
('Turkish Kebab House', 'Istanbul, Turkey', '+90 212 123 4567', 'info@kebab.com', 'Authentic Turkish cuisine with traditional kebabs', 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800'),
('Pizza Paradise', 'Ankara, Turkey', '+90 216 987 6543', 'orders@pizzaparadise.com', 'Best pizza in town with Italian recipes', 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800'),
('Burger Zone', 'Izmir, Turkey', '+90 212 555 0123', 'info@burgerzone.com', 'Juicy burgers and crispy fries', 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=800'),
('Seafood Delight', 'Antalya, Turkey', '+90 212 444 7890', 'contact@seafood.com', 'Fresh seafood daily', 'https://images.unsplash.com/photo-1559339352-11d035aa65de?w=800'),
('Asian Flavors', 'Bursa, Turkey', '+90 212 333 2211', 'hello@asianflavors.com', 'Authentic Asian cuisine', 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800'),
('Sweet Dreams Cafe', 'Istanbul, Turkey', '+90 212 111 9999', 'info@sweetdreams.com', 'Traditional Ottoman desserts', 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=800');

-- Get Restaurant IDs
DECLARE @KebabId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Turkish Kebab House');
DECLARE @PizzaId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Pizza Paradise');
DECLARE @BurgerId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Burger Zone');
DECLARE @SeafoodId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Seafood Delight');
DECLARE @AsianId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Asian Flavors');
DECLARE @CafeId INT = (SELECT RestoranId FROM Restorans WHERE Ad = 'Sweet Dreams Cafe');

-- Insert Foods
INSERT INTO Yemeks (Ad, Aciklama, Fiyat, ResimUrl, RestoranId) VALUES
-- Kebab House
('Adana Kebab', 'Spicy minced meat kebab', 45.50, 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=600', @KebabId),
('Mixed Grill', 'Assorted grilled meats', 65.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @KebabId),
('Chicken Shish', 'Grilled chicken skewers', 38.75, 'https://images.unsplash.com/photo-1571197119282-7c4da777c8b4?w=600', @KebabId),

-- Pizza Paradise
('Margherita Pizza', 'Classic cheese and tomato', 38.00, 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=600', @PizzaId),
('Pepperoni Pizza', 'With pepperoni and cheese', 42.50, 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=600', @PizzaId),
('Vegetarian Pizza', 'Fresh vegetables', 35.75, 'https://images.unsplash.com/photo-1571407970349-bc81e7e96d47?w=600', @PizzaId),

-- Burger Zone
('Classic Burger', 'Beef patty with lettuce', 29.90, 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600', @BurgerId),
('Chicken Burger', 'Crispy chicken burger', 27.50, 'https://images.unsplash.com/photo-1553979459-d2229ba7433a?w=600', @BurgerId),
('Double Burger', 'Two beef patties', 33.25, 'https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=600', @BurgerId),

-- Seafood Delight
('Grilled Salmon', 'Fresh Atlantic salmon', 65.00, 'https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600', @SeafoodId),
('Shrimp Pasta', 'Creamy shrimp pasta', 58.75, 'https://images.unsplash.com/photo-1559847844-5315695dadae?w=600', @SeafoodId),

-- Asian Flavors
('Pad Thai', 'Thai rice noodles', 41.50, 'https://images.unsplash.com/photo-1591814468924-caf88d1232e1?w=600', @AsianId),
('Sushi Roll', 'Fresh sushi platter', 39.25, 'https://images.unsplash.com/photo-1546833999-b9f581a1996d?w=600', @AsianId),

-- Sweet Dreams Cafe
('Baklava', 'Traditional Turkish baklava', 18.50, 'https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=600', @CafeId),
('Kunefe', 'Hot cheese dessert', 22.00, 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=600', @CafeId),
('Turkish Delight', 'Assorted lokum', 25.75, 'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=600', @CafeId);

-- Insert Sample Customers
INSERT INTO Musteris (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi) VALUES
('John', 'Doe', 'john@example.com', 'password123', '+90 533 123 4567', 'Istanbul, Turkey', GETDATE()),
('Jane', 'Smith', 'jane@example.com', 'password123', '+90 534 987 6543', 'Ankara, Turkey', GETDATE());

PRINT 'Sample data inserted successfully!';
