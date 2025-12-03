using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("Restoranlar")]
    public class Restoran
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Key]
        public int RestoranId { get; set; }        // رقم المطعم (Primary Key)

        [Required(ErrorMessage = "Restoran adı gerekli")]
        [Display(Name = "Restoran Adı")]
        public string Ad { get; set; }            
        [Display(Name = "Adres")]
        public string Adres { get; set; }          

        [Display(Name = "Telefon")]
        public string Telefon { get; set; }       

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }          
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }       
        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; }       //  صورة أو شعار المطعم

        // 🔹 Navigation Properties - using virtual for lazy loading
        public virtual ICollection<Yemek> Yemekler { get; set; }   // قائمة الوجبات التي يقدمها المطعم
        
        // Constructor to initialize collections
        public Restoran()
        {
            Yemekler = new List<Yemek>();
        }
    }
}
