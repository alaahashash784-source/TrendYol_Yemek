
// ملف: ABCDbContext.cs
// الغرض: الاتصال بقاعدة البيانات SQL Server
// الشرح: هذا هو "الجسر" بين الكود C# وقاعدة البيانات - يحول الجداول لـ Objects

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    // كلاس قاعدة البيانات - يرث من DbContext (Entity Framework)
    public class ABCDbContext : DbContext
    {
       
        // Constructor - إعداد الاتصال بقاعدة البيانات
        // "ABCConnectionstring" = اسم الاتصال في Web.config
        
        public ABCDbContext() : base("ABCConnectionstring")
        { 
            // إنشاء قاعدة البيانات تلقائياً إذا لم تكن موجودة
            Database.SetInitializer(new CreateDatabaseIfNotExists<ABCDbContext>());
            // تفعيل Lazy Loading - تحميل العلاقات عند الحاجة فقط
            this.Configuration.LazyLoadingEnabled = true;
            // تفعيل Proxy - لعمل الـ Lazy Loading
            this.Configuration.ProxyCreationEnabled = true;
        }
    
        
        public DbSet<OrderPage> OrderPages { get; set; }        // جدول الطلبات
        public DbSet<Fatura> Faturalar { get; set; }            // جدول الفواتير
        public DbSet<Musteri> Musteriler { get; set; }          // جدول الزبائن
        public DbSet<Restoran> Restoranlar { get; set; }        // جدول المطاعم
        public DbSet<Sepet> Sepetler { get; set; }              // جدول السلة
        public DbSet<TrendYol_Platformu> TrendYolPlatformlar { get; set; } // جدول المنصة
        public DbSet<Yemek> Yemekler { get; set; }              // جدول الأطعمة

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // إزالة الجمع التلقائي لأسماء الجداول
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            // تحديد دقة الأرقام العشرية (للأسعار)
            modelBuilder.Entity<Yemek>()
                .Property(y => y.Fiyat)
                .HasPrecision(18, 2);  // 18 رقم، 2 بعد الفاصلة
                
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

