// ═══════════════════════════════════════════════════════════════════════════════
// ملف: Restoran.cs (Model)
// الغرض: تمثيل جدول المطاعم في قاعدة البيانات
// العلاقات: Restoran → Yemek (One-to-Many) - كل مطعم له عدة أطعمة
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
    // [Table("Restoranlar")] = اسم الجدول في SQL Server
    // ═══════════════════════════════════════════════════════════════════════════════
    [Table("Restoranlar")]
    public class Restoran
    {
        // ═══════════════════════════════════════════════════════════════════
        // Primary Key - المفتاح الأساسي
        // ═══════════════════════════════════════════════════════════════════
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int RestoranId { get; set; }        
 
        [Required(ErrorMessage = "Restoran adı gerekli")]  // اسم المطعم إجباري
        [Display(Name = "Restoran Adı")]
        public string Ad { get; set; }            

        [Display(Name = "Adres")]
        public string Adres { get; set; }          // عنوان المطعم

        [Display(Name = "Telefon")]
        public string Telefon { get; set; }        // رقم الهاتف

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }          // البريد الإلكتروني

        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }       // وصف المطعم

        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; }       // صورة أو شعار المطعم

        
        // Navigation Property - علاقة واحد لمتعدد
        // virtual = للـ Lazy Loading
        // ICollection = قائمة من الأطعمة
        
        public virtual ICollection<Yemek> Yemekler { get; set; }   // أطعمة هذا المطعم
        
        public Restoran()
        {
            Yemekler = new List<Yemek>();  // تهيئة قائمة فارغة
        }
    }
}
