
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
   
    [Table("Musteriler")]
    public class Musteri
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int MusteriId { get; set; }    

       
        // بيانات المستخدم الأساسية
        // [Required] = حقل إجباري
        // [StringLength] = الحد الأقصى للأحرف
       
        [Required(ErrorMessage = "Ad gerekli")]           // الاسم إجباري
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Ad { get; set; }   

        [Required(ErrorMessage = "Soyad gerekli")]        // اللقب إجباري
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string Soyad { get; set; }   

        // ═══════════════════════════════════════════════════════════════════
        // بيانات تسجيل الدخول
        // ═══════════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "Email gerekli")]
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Gecerli bir email adresi giriniz")]  // التحقق من صيغة الإيميل
        [StringLength(100)]
        public string Email { get; set; }     

        [Required(ErrorMessage = "Sifre gerekli")]
        [Display(Name = "Sifre")]
        [DataType(DataType.Password)]                     // يظهر كنجوم ***
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
        public string Sifre { get; set; }     

        // ═══════════════════════════════════════════════════════════════════
        // بيانات التواصل والتوصيل
        // ═══════════════════════════════════════════════════════════════════
        [Display(Name = "Telefon Numarasi")]
        [Phone(ErrorMessage = "Gecerli bir telefon numarasi giriniz")]
        [StringLength(20)]
        public string Telefon { get; set; }   

        [Display(Name = "Adres")]
        [StringLength(500)]
        public string Adres { get; set; }     
        
        [Display(Name = "Kayit Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;  // تاريخ التسجيل

        [Display(Name = "Admin Mi?")]
        public bool IsAdmin { get; set; } = false;  // هل المستخدم أدمن؟

        // ═══════════════════════════════════════════════════════════════════
        // Restaurant Admin - مدير مطعم
        // ═══════════════════════════════════════════════════════════════════
        [Display(Name = "Restoran Yöneticisi Mi?")]
        public bool IsRestoranAdmin { get; set; } = false;  // هل مدير مطعم؟

        [Display(Name = "Yönetilen Restoran")]
        public int? RestoranId { get; set; }  // رقم المطعم المدار (null للمستخدم العادي)

        [ForeignKey("RestoranId")]
        public virtual Restoran YonetilenRestoran { get; set; }  // المطعم المدار

        public virtual ICollection<Sepet> Sepetler { get; set; }      // سلات الزبون
        public virtual ICollection<OrderPage> Siparisler { get; set; } // طلبات الزبون
        public virtual ICollection<Fatura> Faturalar { get; set; }    // فواتير الزبون

        [NotMapped]
        [Display(Name = "Tam Ad")]
        public string TamAd
        {
            get { return Ad + " " + Soyad; }  // دمج الاسم واللقب
        }
    }
}