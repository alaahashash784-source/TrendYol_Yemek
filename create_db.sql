USE TrendYolYemekDB;
GO

-- Drop tables if they exist (in correct order due to foreign keys)
IF OBJECT_ID('dbo.Sepets', 'U') IS NOT NULL DROP TABLE dbo.Sepets;
IF OBJECT_ID('dbo.Yemeks', 'U') IS NOT NULL DROP TABLE dbo.Yemeks;
IF OBJECT_ID('dbo.Faturas', 'U') IS NOT NULL DROP TABLE dbo.Faturas;
IF OBJECT_ID('dbo.OrderPages', 'U') IS NOT NULL DROP TABLE dbo.OrderPages;
IF OBJECT_ID('dbo.Restorans', 'U') IS NOT NULL DROP TABLE dbo.Restorans;
IF OBJECT_ID('dbo.Musteris', 'U') IS NOT NULL DROP TABLE dbo.Musteris;
IF OBJECT_ID('dbo.TrendYol_Platformu', 'U') IS NOT NULL DROP TABLE dbo.TrendYol_Platformu;
GO

-- Create Musteris (Customers) table
CREATE TABLE dbo.Musteris (
    MusteriId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(50) NOT NULL,
    Soyad NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Sifre NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20),
    Adres NVARCHAR(500),
    KayitTarihi DATETIME NOT NULL DEFAULT GETDATE()
);

-- Create Restorans (Restaurants) table
CREATE TABLE dbo.Restorans (
    RestoranId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Adres NVARCHAR(500),
    Telefon NVARCHAR(20),
    Email NVARCHAR(100),
    Aciklama NVARCHAR(MAX),
    ResimUrl NVARCHAR(500)
);

-- Create Yemeks (Foods) table
CREATE TABLE dbo.Yemeks (
    YemekId INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Aciklama NVARCHAR(MAX),
    Fiyat DECIMAL(18,2) NOT NULL,
    ResimUrl NVARCHAR(500),
    RestoranId INT NOT NULL,
    FOREIGN KEY (RestoranId) REFERENCES dbo.Restorans(RestoranId)
);

-- Create OrderPages table
CREATE TABLE dbo.OrderPages (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId INT NOT NULL,
    ToplamTutar DECIMAL(18,2) NOT NULL,
    TeslimatAdresi NVARCHAR(500),
    OdemeYontemi NVARCHAR(50),
    SiparisTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteris(MusteriId)
);

-- Create Sepets (Cart) table
CREATE TABLE dbo.Sepets (
    SepetId INT IDENTITY(1,1) PRIMARY KEY,
    MusteriId INT NOT NULL,
    YemekId INT NOT NULL,
    Miktar INT NOT NULL,
    Fiyat DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteris(MusteriId),
    FOREIGN KEY (YemekId) REFERENCES dbo.Yemeks(YemekId)
);

-- Create Faturas (Invoices) table
CREATE TABLE dbo.Faturas (
    FaturaId INT IDENTITY(1,1) PRIMARY KEY,
    SiparisId INT NOT NULL,
    MusteriId INT NOT NULL,
    ToplamTutar DECIMAL(18,2) NOT NULL,
    OdemeYontemi NVARCHAR(50),
    FaturaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MusteriId) REFERENCES dbo.Musteris(MusteriId),
    FOREIGN KEY (SiparisId) REFERENCES dbo.OrderPages(OrderId)
);

-- Create TrendYol_Platformu table
CREATE TABLE dbo.TrendYol_Platformu (
    PlatformId INT IDENTITY(1,1) PRIMARY KEY,
    PlatformAdi NVARCHAR(100) NOT NULL,
    KurulusTarihi DATETIME NOT NULL
);
GO

PRINT 'Database schema created successfully!';
