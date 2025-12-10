namespace mvc_full.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Faturas",
                c => new
                    {
                        FaturaId = c.Int(nullable: false, identity: true),
                        SiparisId = c.Int(nullable: false),
                        MusteriId = c.Int(nullable: false),
                        ToplamTutar = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OdemeYontemi = c.String(),
                        FaturaTarihi = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FaturaId)
                .ForeignKey("dbo.Musteris", t => t.MusteriId, cascadeDelete: true)
                .Index(t => t.MusteriId);
            
            CreateTable(
                "dbo.Musteris",
                c => new
                    {
                        MusteriId = c.Int(nullable: false, identity: true),
                        AdSoyad = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Sifre = c.String(nullable: false),
                        Telefon = c.String(),
                        Adres = c.String(),
                        KayitTarihi = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MusteriId);
            
            CreateTable(
                "dbo.OrderPages",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryAddress = c.String(),
                        PaymentMethod = c.String(),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.Restorans",
                c => new
                    {
                        RestoranId = c.Int(nullable: false, identity: true),
                        Ad = c.String(nullable: false),
                        Adres = c.String(),
                        Telefon = c.String(),
                        Email = c.String(),
                        Aciklama = c.String(),
                        ResimUrl = c.String(),
                    })
                .PrimaryKey(t => t.RestoranId);
            
            CreateTable(
                "dbo.Yemeks",
                c => new
                    {
                        YemekId = c.Int(nullable: false, identity: true),
                        Ad = c.String(nullable: false),
                        Aciklama = c.String(),
                        Fiyat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResimUrl = c.String(),
                        RestaurantId = c.Int(nullable: false),
                        Restoran_RestoranId = c.Int(),
                    })
                .PrimaryKey(t => t.YemekId)
                .ForeignKey("dbo.Restorans", t => t.Restoran_RestoranId)
                .Index(t => t.Restoran_RestoranId);
            
            CreateTable(
                "dbo.Sepets",
                c => new
                    {
                        SepetId = c.Int(nullable: false, identity: true),
                        MusteriId = c.Int(nullable: false),
                        YemekId = c.Int(nullable: false),
                        Miktar = c.Int(nullable: false),
                        Fiyat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Yemek_Enabled = c.Boolean(nullable: false),
                        Yemek_ImageUrl = c.String(),
                        Yemek_NavigateUrl = c.String(),
                        Yemek_PopOutImageUrl = c.String(),
                        Yemek_Selectable = c.Boolean(nullable: false),
                        Yemek_Selected = c.Boolean(nullable: false),
                        Yemek_SeparatorImageUrl = c.String(),
                        Yemek_Target = c.String(),
                        Yemek_Text = c.String(),
                        Yemek_ToolTip = c.String(),
                        Yemek_Value = c.String(),
                    })
                .PrimaryKey(t => t.SepetId)
                .ForeignKey("dbo.Musteris", t => t.MusteriId, cascadeDelete: true)
                .Index(t => t.MusteriId);
            
            CreateTable(
                "dbo.TrendYol_Platformu",
                c => new
                    {
                        PlatformId = c.Int(nullable: false, identity: true),
                        PlatformAdi = c.String(nullable: false),
                        KurulusTarihi = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PlatformId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sepets", "MusteriId", "dbo.Musteris");
            DropForeignKey("dbo.Yemeks", "Restoran_RestoranId", "dbo.Restorans");
            DropForeignKey("dbo.Faturas", "MusteriId", "dbo.Musteris");
            DropIndex("dbo.Sepets", new[] { "MusteriId" });
            DropIndex("dbo.Yemeks", new[] { "Restoran_RestoranId" });
            DropIndex("dbo.Faturas", new[] { "MusteriId" });
            DropTable("dbo.TrendYol_Platformu");
            DropTable("dbo.Sepets");
            DropTable("dbo.Yemeks");
            DropTable("dbo.Restorans");
            DropTable("dbo.OrderPages");
            DropTable("dbo.Musteris");
            DropTable("dbo.Faturas");
        }
    }
}
