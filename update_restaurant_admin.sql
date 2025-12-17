-- ═══════════════════════════════════════════════════════════════════════════════
-- ملف: update_restaurant_admin.sql
-- الغرض: إضافة نظام مدير المطعم ووقت التحضير
-- التاريخ: 2024
-- ═══════════════════════════════════════════════════════════════════════════════

USE TrendYolYemekDB;
GO

-- ═══════════════════════════════════════════════════════════════════════════════
-- 1. إضافة أعمدة مدير المطعم للمستخدمين
-- ═══════════════════════════════════════════════════════════════════════════════

-- التحقق وإضافة عمود IsRestoranAdmin
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Musteriler') AND name = 'IsRestoranAdmin')
BEGIN
    ALTER TABLE Musteriler ADD IsRestoranAdmin BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsRestoranAdmin column to Musteriler';
END
GO

-- التحقق وإضافة عمود RestoranId (المطعم المدار)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Musteriler') AND name = 'RestoranId')
BEGIN
    ALTER TABLE Musteriler ADD RestoranId INT NULL;
    PRINT 'Added RestoranId column to Musteriler';
    
    -- إضافة Foreign Key
    ALTER TABLE Musteriler ADD CONSTRAINT FK_Musteriler_Restoranlar 
        FOREIGN KEY (RestoranId) REFERENCES Restoranlar(RestoranId) ON DELETE SET NULL;
    PRINT 'Added FK_Musteriler_Restoranlar constraint';
END
GO

-- ═══════════════════════════════════════════════════════════════════════════════
-- 2. إضافة أعمدة وقت التحضير والفئة للوجبات
-- ═══════════════════════════════════════════════════════════════════════════════

-- التحقق وإضافة عمود HazirlanmaSuresi (وقت التحضير بالدقائق)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Yemekler') AND name = 'HazirlanmaSuresi')
BEGIN
    ALTER TABLE Yemekler ADD HazirlanmaSuresi INT NOT NULL DEFAULT 15;
    PRINT 'Added HazirlanmaSuresi column to Yemekler';
END
GO

-- التحقق وإضافة عمود Kategori (فئة الطعام)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Yemekler') AND name = 'Kategori')
BEGIN
    ALTER TABLE Yemekler ADD Kategori NVARCHAR(50) NULL;
    PRINT 'Added Kategori column to Yemekler';
END
GO

-- ═══════════════════════════════════════════════════════════════════════════════
-- 3. تحديث الوجبات الموجودة بفئات افتراضية
-- ═══════════════════════════════════════════════════════════════════════════════

-- تحديث الكباب
UPDATE Yemekler SET Kategori = 'Kebap', HazirlanmaSuresi = 25 
WHERE Kategori IS NULL AND (Ad LIKE '%kebap%' OR Ad LIKE '%Kebap%' OR Ad LIKE '%adana%' OR Ad LIKE '%iskender%');

-- تحديث البيتزا
UPDATE Yemekler SET Kategori = 'Pizza', HazirlanmaSuresi = 20 
WHERE Kategori IS NULL AND (Ad LIKE '%pizza%' OR Ad LIKE '%Pizza%');

-- تحديث البرغر
UPDATE Yemekler SET Kategori = 'Burger', HazirlanmaSuresi = 15 
WHERE Kategori IS NULL AND (Ad LIKE '%burger%' OR Ad LIKE '%Burger%');

-- تحديث الدونر
UPDATE Yemekler SET Kategori = 'Döner', HazirlanmaSuresi = 10 
WHERE Kategori IS NULL AND (Ad LIKE '%döner%' OR Ad LIKE '%doner%' OR Ad LIKE '%Döner%');

-- تحديث الحلويات
UPDATE Yemekler SET Kategori = 'Tatlı', HazirlanmaSuresi = 10 
WHERE Kategori IS NULL AND (Ad LIKE '%baklava%' OR Ad LIKE '%kunefe%' OR Ad LIKE '%künefe%' OR Ad LIKE '%tatli%' OR Ad LIKE '%Tatlı%');

-- تحديث المشروبات
UPDATE Yemekler SET Kategori = 'İçecek', HazirlanmaSuresi = 5 
WHERE Kategori IS NULL AND (Ad LIKE '%cola%' OR Ad LIKE '%ayran%' OR Ad LIKE '%çay%' OR Ad LIKE '%kahve%' OR Ad LIKE '%içecek%');

-- تحديث الشوربات
UPDATE Yemekler SET Kategori = 'Çorba', HazirlanmaSuresi = 10 
WHERE Kategori IS NULL AND (Ad LIKE '%çorba%' OR Ad LIKE '%Çorba%' OR Ad LIKE '%corba%');

-- تحديث السلطات
UPDATE Yemekler SET Kategori = 'Salata', HazirlanmaSuresi = 8 
WHERE Kategori IS NULL AND (Ad LIKE '%salata%' OR Ad LIKE '%Salata%');

-- الباقي يكون "أخرى"
UPDATE Yemekler SET Kategori = 'Diğer', HazirlanmaSuresi = 15 
WHERE Kategori IS NULL;

PRINT 'Updated existing foods with categories';
GO

-- ═══════════════════════════════════════════════════════════════════════════════
-- 4. إنشاء مدير مطعم تجريبي
-- ═══════════════════════════════════════════════════════════════════════════════

-- جلب أول مطعم
DECLARE @FirstRestoranId INT;
SELECT TOP 1 @FirstRestoranId = RestoranId FROM Restoranlar;

IF @FirstRestoranId IS NOT NULL
BEGIN
    -- التحقق من وجود مدير تجريبي
    IF NOT EXISTS (SELECT * FROM Musteriler WHERE Email = 'restoran1@trendyol.com')
    BEGIN
        INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi, IsAdmin, IsRestoranAdmin, RestoranId)
        VALUES (
            'Restoran', 
            'Yöneticisi', 
            'restoran1@trendyol.com', 
            -- كلمة المرور: admin123 (مشفرة)
            'eJwBTACz/6C0YxqUcL0xAAAAABAAAAAlJhHBkFAALUuQ28qR18sMnbQ0Y3tPsNQ6OdTG5FpZT3Q=',
            '5551234567',
            'İstanbul',
            GETDATE(),
            0,  -- ليس أدمن رئيسي
            1,  -- مدير مطعم
            @FirstRestoranId
        );
        PRINT 'Created test restaurant admin: restoran1@trendyol.com / admin123';
    END
    ELSE
    BEGIN
        -- تحديث المستخدم الموجود ليكون مدير مطعم
        UPDATE Musteriler 
        SET IsRestoranAdmin = 1, RestoranId = @FirstRestoranId
        WHERE Email = 'restoran1@trendyol.com';
        PRINT 'Updated restoran1@trendyol.com as restaurant admin';
    END
END
GO

-- ═══════════════════════════════════════════════════════════════════════════════
-- 5. عرض النتائج
-- ═══════════════════════════════════════════════════════════════════════════════

PRINT '';
PRINT '=== Restaurant Admin System Setup Complete ===';
PRINT '';

SELECT 'Musteriler Columns' AS Info, name AS ColumnName, TYPE_NAME(user_type_id) AS DataType 
FROM sys.columns WHERE object_id = OBJECT_ID('Musteriler') AND name IN ('IsRestoranAdmin', 'RestoranId');

SELECT 'Yemekler Columns' AS Info, name AS ColumnName, TYPE_NAME(user_type_id) AS DataType 
FROM sys.columns WHERE object_id = OBJECT_ID('Yemekler') AND name IN ('HazirlanmaSuresi', 'Kategori');

SELECT 'Restaurant Admins' AS Info, m.Email, m.TamAd, r.Ad AS RestoranAdi
FROM Musteriler m
LEFT JOIN Restoranlar r ON m.RestoranId = r.RestoranId
WHERE m.IsRestoranAdmin = 1;

SELECT 'Food Categories' AS Info, Kategori, COUNT(*) AS Count
FROM Yemekler
GROUP BY Kategori;

PRINT '';
PRINT 'Login credentials for restaurant admin:';
PRINT 'Email: restoran1@trendyol.com';
PRINT 'Password: admin123';
GO
