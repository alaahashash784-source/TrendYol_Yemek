using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("Faturalar")]
    public class Fatura
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Key]
        public int FaturaId { get; set; }             //  رقم الفاتورة (Primary Key)

        [Required]
        public int SiparisId { get; set; }           //  رقم الطلب المرتبط بالفاتورة

        [Required]
        public int MusteriId { get; set; }           //  رقم العميل

        [Display(Name = "Toplam Tutar")]
        public decimal ToplamTutar { get; set; }     //  المجموع الكلي

        [Display(Name = "Ödeme Yöntemi")]
        public string OdemeYontemi { get; set; }     //  طريقة الدفع (نقدي، بطاقة، ...)

        [Display(Name = "Fatura Tarihi")]
        public DateTime FaturaTarihi { get; set; } = DateTime.Now;

        //  علاقات - Navigation Properties
        [ForeignKey("MusteriId")]
        public virtual Musteri Musteri { get; set; }         // علاقة مع العميل
        
        [ForeignKey("SiparisId")]
        public virtual OrderPage Siparis { get; set; }       // علاقة مع الطلب
       
    }
}