// ═══════════════════════════════════════════════════════════════════════════════
// ملف: Yemek.cs (Model)
// الغرض: تمثيل جدول الأطعمة/الوجبات في قاعدة البيانات
// العلاقات: Restoran ← Yemek (Many-to-One) - كل طعام ينتمي لمطعم واحد
// ═══════════════════════════════════════════════════════════════════════════════
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    // ═══════════════════════════════════════════════════════════════════════════════
    // [Table("Yemekler")] = اسم الجدول في SQL Server
    // ═══════════════════════════════════════════════════════════════════════════════
    [Table("Yemekler")]
    public class Yemek
    {
        // ═══════════════════════════════════════════════════════════════════
        // Primary Key - المفتاح الأساسي
        // ═══════════════════════════════════════════════════════════════════
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int YemekId { get; set; }           

        // ═══════════════════════════════════════════════════════════════════
        // بيانات الطعام
        // ═══════════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "Yemek ismi gerekli")]   // اسم الطعام إجباري
        [Display(Name = "Yemek Adi")]
        public string Ad { get; set; }           

        [Display(Name = "Aciklama")]
        public string Aciklama { get; set; }              // وصف الطعام (اختياري)

        [Required]
        [Range(0, 10000)]                                 // السعر بين 0 و 10000
        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }                // سعر الطعام

        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; }              // رابط صورة الطعام

        // ═══════════════════════════════════════════════════════════════════
        // وقت التحضير - Hazırlanma Süresi
        // ═══════════════════════════════════════════════════════════════════
        [Display(Name = "Hazırlanma Süresi (dk)")]
        [Range(1, 180, ErrorMessage = "Hazırlanma süresi 1-180 dakika arasında olmalıdır")]
        public int HazirlanmaSuresi { get; set; } = 15;  // وقت التحضير بالدقائق (افتراضي 15 دقيقة)

        // ═══════════════════════════════════════════════════════════════════
        // نوع/صنف الطعام للبحث
        // ═══════════════════════════════════════════════════════════════════
        [Display(Name = "Yemek Kategorisi")]
        public string Kategori { get; set; }  // نوع الطعام (Pizza, Kebap, Tatlı, etc.)
        
        // ═══════════════════════════════════════════════════════════════════
        // كمية المخزون - Stok Miktarı
        // ═══════════════════════════════════════════════════════════════════
        [Display(Name = "Stok Miktarı")]
        [Range(0, 10000, ErrorMessage = "Stok 0-10000 arasında olmalıdır")]
        public int Stok { get; set; } = 100;  // الكمية المتاحة (افتراضي 100)
        
        [Display(Name = "Stok Durumu")]
        public bool StokAktif { get; set; } = true;  // هل المنتج متاح للطلب
        
        // ═══════════════════════════════════════════════════════════════════
        // Foreign Key - المفتاح الأجنبي (يربط الطعام بالمطعم)
        // ═══════════════════════════════════════════════════════════════════
        [Required]
        public int RestoranId { get; set; }               // رقم المطعم
        
        // خاصية بديلة للتوافق
        [NotMapped]
        public int RestaurantId 
        { 
            get { return RestoranId; } 
            set { RestoranId = value; } 
        }

        // ═══════════════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // [ForeignKey] = يحدد العمود الذي يربط الجدولين
        // ═══════════════════════════════════════════════════════════════════
        [ForeignKey("RestoranId")]
        public virtual Restoran Restoran { get; set; }    // المطعم المالك للطعام

        public virtual ICollection<Sepet> Sepetler { get; set; }  // السلات التي تحتوي هذا الطعام
    }
}