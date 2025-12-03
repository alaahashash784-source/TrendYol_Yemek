/*
  الغرض: تمثيل جدول الأطعمة/الوجبات في قاعدة البيانات  العلاقات: ║
║    - Restoran: المطعم الذي يقدم هذا الطعام (Many-to-One)    ║
║  ملاحظة: RestaurantId هي خاصية بديلة (NotMapped) للتوافق    
*/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("Yemekler")]
    public class Yemek
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int YemekId { get; set; }           

        [Required(ErrorMessage = "Yemek ismi gerekli")]
        [Display(Name = "Yemek Ad�")]
        public string Ad { get; set; }           

        [Display(Name = "A��klama")]
        public string Aciklama { get; set; }      

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }        

        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; }     
        
        [Required]
        public int RestoranId { get; set; }    
        // Alternative property for compatibility
        [NotMapped]
        public int RestaurantId 
        { 
            get { return RestoranId; } 
            set { RestoranId = value; } 
        }

        // Navigation Properties
        [ForeignKey("RestoranId")]
        public virtual Restoran Restoran { get; set; }

        public virtual ICollection<Sepet> Sepetler { get; set; }
    }
}