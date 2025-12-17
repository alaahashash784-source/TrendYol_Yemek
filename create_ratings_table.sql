-- إنشاء جدول التقييمات والتعليقات
-- Rating System for TrendYolYemek

USE TrendYolYemekDB;
GO

-- إنشاء جدول التقييمات إذا لم يكن موجوداً
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ratings')
BEGIN
    CREATE TABLE Ratings (
        RatingId INT IDENTITY(1,1) PRIMARY KEY,
        MusteriId INT NOT NULL,
        YemekId INT NULL,
        RestoranId INT NULL,
        OrderId INT NULL,
        Puan INT NOT NULL CHECK (Puan >= 1 AND Puan <= 5),
        Yorum NVARCHAR(1000) NULL,
        DegerlendirmeTarihi DATETIME NOT NULL DEFAULT GETDATE(),
        Onaylandi BIT NOT NULL DEFAULT 1,
        
        -- Foreign Keys
        CONSTRAINT FK_Ratings_Musteri FOREIGN KEY (MusteriId) REFERENCES Musteriler(MusteriId),
        CONSTRAINT FK_Ratings_Yemek FOREIGN KEY (YemekId) REFERENCES Yemekler(YemekId),
        CONSTRAINT FK_Ratings_Restoran FOREIGN KEY (RestoranId) REFERENCES Restoranlar(RestoranId),
        CONSTRAINT FK_Ratings_Order FOREIGN KEY (OrderId) REFERENCES OrderPages(OrderId)
    );
    
    PRINT 'Ratings tablosu olusturuldu';
END
ELSE
BEGIN
    PRINT 'Ratings tablosu zaten mevcut';
END
GO

-- إضافة بعض التقييمات التجريبية
-- Sample ratings for testing

-- التحقق من وجود بيانات اختبارية
IF NOT EXISTS (SELECT * FROM Ratings)
BEGIN
    -- تقييمات للأطعمة
    INSERT INTO Ratings (MusteriId, YemekId, Puan, Yorum, DegerlendirmeTarihi)
    SELECT TOP 1 
        m.MusteriId, 
        y.YemekId, 
        5, 
        N'Harika bir lezzet! Kesinlikle tavsiye ederim.', 
        DATEADD(day, -3, GETDATE())
    FROM Musteriler m, Yemekler y
    WHERE m.IsAdmin = 0;
    
    INSERT INTO Ratings (MusteriId, YemekId, Puan, Yorum, DegerlendirmeTarihi)
    SELECT TOP 1 
        m.MusteriId, 
        (SELECT TOP 1 YemekId FROM Yemekler ORDER BY NEWID()), 
        4, 
        N'Cok guzel, tekrar siparis verecegim.', 
        DATEADD(day, -2, GETDATE())
    FROM Musteriler m
    WHERE m.IsAdmin = 0;
    
    -- تقييمات للمطاعم
    INSERT INTO Ratings (MusteriId, RestoranId, Puan, Yorum, DegerlendirmeTarihi)
    SELECT TOP 1 
        m.MusteriId, 
        r.RestoranId, 
        5, 
        N'Hizli teslimat ve kaliteli yemekler. 10 uzerinden 10!', 
        DATEADD(day, -1, GETDATE())
    FROM Musteriler m, Restoranlar r
    WHERE m.IsAdmin = 0;
    
    PRINT 'Ornek degerlendirmeler eklendi';
END
GO

-- عرض التقييمات الموجودة
SELECT 
    r.RatingId,
    m.Ad + ' ' + ISNULL(m.Soyad, '') AS Musteri,
    y.Isim AS Yemek,
    res.Isim AS Restoran,
    r.Puan,
    r.Yorum,
    r.DegerlendirmeTarihi
FROM Ratings r
LEFT JOIN Musteriler m ON r.MusteriId = m.MusteriId
LEFT JOIN Yemekler y ON r.YemekId = y.YemekId
LEFT JOIN Restoranlar res ON r.RestoranId = res.RestoranId
ORDER BY r.DegerlendirmeTarihi DESC;
