USE TrendYolYemekDB;
GO

-- Drop tables if they exist (in correct order due to foreign keys)
IF OBJECT_ID('dbo.Sepetler', 'U') IS NOT NULL DROP TABLE dbo.Sepetler;
IF OBJECT_ID('dbo.Yemekler', 'U') IS NOT NULL DROP TABLE dbo.Yemekler;
IF OBJECT_ID('dbo.Faturalar', 'U') IS NOT NULL DROP TABLE dbo.Faturalar;
IF OBJECT_ID('dbo.OrderPages', 'U') IS NOT NULL DROP TABLE dbo.OrderPages;
IF OBJECT_ID('dbo.Restoranlar', 'U') IS NOT NULL DROP TABLE dbo.Restoranlar;
IF OBJECT_ID('dbo.Musteriler', 'U') IS NOT NULL DROP TABLE dbo.Musteriler;
IF OBJECT_ID('dbo.TrendYolPlatformlar', 'U') IS NOT NULL DROP TABLE dbo.TrendYolPlatformlar;
GO

-- Create Musteriler (Customers) table
CREATE TABLE dbo.Musteriler (
    MusteriId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(50) NOT NULL,
    Soyad NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Sifre NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20),
    Adres NVARCHAR(500),
    KayitTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    IsAdmin BIT NOT NULL DEFAULT 0
);

-- Create Restoranlar (Restaurants) table
CREATE TABLE dbo.Restoranlar (
    RestoranId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Adres NVARCHAR(500),
    Telefon NVARCHAR(20),
    Email NVARCHAR(100),
    Aciklama NVARCHAR(MAX),
    ResimUrl NVARCHAR(500)
);

-- Create Yemekler (Foods) table
CREATE TABLE dbo.Yemekler (
    YemekId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Aciklama NVARCHAR(MAX),
    Fiyat DECIMAL(18,2) NOT NULL,
    ResimUrl NVARCHAR(500),
    RestoranId INT NOT NULL,
    FOREIGN KEY (RestoranId) REFERENCES dbo.Restoranlar(RestoranId)
);

-- Create OrderPages table
CREATE TABLE dbo.OrderPages (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId INT NOT NULL,
    ToplamTutar DECIMAL(18,2) NOT NULL,
    TeslimatAdresi NVARCHAR(500),
    OdemeYontemi NVARCHAR(50),
    SiparisTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteriler(MusteriId)
);

-- Create Sepetler (Cart) table
CREATE TABLE dbo.Sepetler (
    SepetId INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId INT NOT NULL,
    YemekId INT NOT NULL,
    Miktar INT NOT NULL,
    Fiyat DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteriler(MusteriId),
    FOREIGN KEY (YemekId) REFERENCES dbo.Yemekler(YemekId)
);

-- Create Faturalar (Invoices) table
CREATE TABLE dbo.Faturalar (
    FaturaId INT IDENTITY(1,1) PRIMARY KEY,
    SiparisId INT NOT NULL,
    MusteriId INT NOT NULL,
    ToplamTutar DECIMAL(18,2) NOT NULL,
    OdemeYontemi NVARCHAR(50),
    FaturaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteriler(MusteriId),
    FOREIGN KEY (SiparisId) REFERENCES dbo.OrderPages(OrderId)
);

-- Create TrendYolPlatformlar table
CREATE TABLE dbo.TrendYolPlatformlar (
    PlatformId INT IDENTITY(1,1) PRIMARY KEY,
    PlatformAdi NVARCHAR(100) NOT NULL,
    KurulusTarihi DATETIME NOT NULL
);
GO

PRINT 'Database schema created successfully!';
