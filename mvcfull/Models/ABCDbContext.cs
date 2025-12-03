using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    // Database context - manages SQL Server connection
    public class ABCDbContext : DbContext
    {
        // Constructor - uses ABCConnectionstring from Web.config
        public ABCDbContext() : base("ABCConnectionstring")
        { 
            Database.SetInitializer(new CreateDatabaseIfNotExists<ABCDbContext>());
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }
    
        // DbSet properties
        public DbSet<OrderPage> OrderPages { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Restoran> Restoranlar { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<TrendYol_Platformu> TrendYolPlatformlar { get; set; }
        public DbSet<Yemek> Yemekler { get; set; }

        // Configure model relationships
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Yemek>()
                .Property(y => y.Fiyat)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Sepet>()
                .Property(s => s.Fiyat)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Fatura>()
                .Property(f => f.ToplamTutar)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<OrderPage>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}

