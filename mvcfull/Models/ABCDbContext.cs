// سياق قاعدة البيانات - يدير الاتصال بـ SQL Server ويعرف الجداول
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    // سياق قاعدة البيانات - يدير كل العمليات مع SQL Server
    public class ABCDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of ABCDbContext.
        /// Uses the connection string named "ABCConnectionstring" from Web.config.
        /// </summary>
        public ABCDbContext() : base("ABCConnectionstring")
        { 
            // Enable automatic database creation if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<ABCDbContext>());
            
            // Enable lazy loading for navigation properties
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }
    
        // DbSet properties for each entity
        public DbSet<OrderPage> OrderPages { get; set; }
        public DbSet<Fatura> Faturalar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Restoran> Restoranlar { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<TrendYol_Platformu> TrendYolPlatformlar { get; set; }
        public DbSet<Yemek> Yemekler { get; set; }

        /// <summary>
        /// Configures the model relationships and constraints.
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralizing table names convention (we use explicit table names)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            // Configure decimal precision for price fields
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

