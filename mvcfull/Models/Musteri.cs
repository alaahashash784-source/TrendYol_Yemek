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

        [Required(ErrorMessage = "Ad gerekli")]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Ad { get; set; }   
        [Required(ErrorMessage = "Soyad gerekli")]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string Soyad { get; set; }   

        [Required(ErrorMessage = "Email gerekli")]
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Ge�erli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; }     
        [Required(ErrorMessage = "�ifre gerekli")]
        [Display(Name = "�ifre")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "�ifre en az 6 karakter olmal�d�r")]
        public string Sifre { get; set; }     

        [Display(Name = "Telefon Numaras�")]
        [Phone(ErrorMessage = "Ge�erli bir telefon numaras� giriniz")]
        [StringLength(20)]
        public string Telefon { get; set; }   

        [Display(Name = "Adres")]
        [StringLength(500)]
        public string Adres { get; set; }     
        
        [Display(Name = "Kay�t Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now; 

        [Display(Name = "Admin Mi?")]
        public bool IsAdmin { get; set; } = false; // Admin kullanıcı mı?

        // Navigation Properties
        public virtual ICollection<Sepet> Sepetler { get; set; }
        public virtual ICollection<OrderPage> Siparisler { get; set; }
        public virtual ICollection<Fatura> Faturalar { get; set; }

        [NotMapped]
        [Display(Name = "Tam Ad")]
        public string TamAd
        {
            get { return Ad + " " + Soyad; }
        }
    }
}