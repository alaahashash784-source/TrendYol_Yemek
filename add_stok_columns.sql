-- إضافة أعمدة الكمية (Stok) لجدول الوجبات
-- تحقق أولاً إذا كانت الأعمدة موجودة قبل إضافتها

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Yemekler') AND name = 'Stok')
BEGIN
    ALTER TABLE Yemekler ADD Stok INT NOT NULL DEFAULT 100;
    PRINT 'تم إضافة عمود Stok بنجاح';
END
ELSE
BEGIN
    PRINT 'عمود Stok موجود مسبقاً';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Yemekler') AND name = 'StokAktif')
BEGIN
    ALTER TABLE Yemekler ADD StokAktif BIT NOT NULL DEFAULT 1;
    PRINT 'تم إضافة عمود StokAktif بنجاح';
END
ELSE
BEGIN
    PRINT 'عمود StokAktif موجود مسبقاً';
END

-- تحديث القيم الموجودة
UPDATE Yemekler SET Stok = 100, StokAktif = 1 WHERE Stok IS NULL OR StokAktif IS NULL;

PRINT 'تم تحديث جميع الوجبات بالكمية الافتراضية 100';
