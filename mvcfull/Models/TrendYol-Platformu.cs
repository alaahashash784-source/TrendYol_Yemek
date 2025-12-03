using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("TrendYol_Platformu")]
    public class TrendYol_Platformu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Key]
        public int PlatformId { get; set; }          //  رقم المنصة (Primary Key)

        [Required]
        [Display(Name = "Platform Adı")]
        public string PlatformAdi { get; set; } = "TrendYol Yemek"; //  اسم المنصة

        [Display(Name = "Kuruluş Tarihi")]
        public DateTime KurulusTarihi { get; set; } = DateTime.Now; //  تاريخ تأسيس المنصة
    }
}