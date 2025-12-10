using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("Sepetler")]
    public class Sepet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SepetId { get; set; }       // ?? ??? ????? (Primary Key)

        [Required]
        public int MusteriId { get; set; }     // ?? ??? ?????? (??? ???? ?????????)

        [Required]
        public int YemekId { get; set; }       // ?? ??? ?????? (MenuItem)

        [Required]
        [Range(1, 100, ErrorMessage = "Miktar 1 ile 100 aras�nda olmal�")]
        public int Miktar { get; set; }        // ?? ?????? ????????

        [Required]
        public decimal Fiyat { get; set; }     // ?? ??? ?????? ??? ????

        [Display(Name = "Ekleme Tarihi")]
        public DateTime EklemeTarihi { get; set; } = DateTime.Now;

        // ?? ?????? - Navigation Properties
        [ForeignKey("MusteriId")]
        public virtual Musteri Musteri { get; set; }   // ????? One-to-Many ?? ??????
        
        [ForeignKey("YemekId")]
        public virtual Yemek Yemek { get; set; }    // ????? One-to-Many ?? ??????

        // Calculated property for total price
        [NotMapped]
        public decimal ToplamFiyat
        {
            get { return Fiyat * Miktar; }
        }
    }
}