
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvc_full.Models
{
    [Table("OrderPages")]
    public class OrderPage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Column("MusteriId")]
        public int MusteriId { get; set; }
        
        // Alias property for backward compatibility with existing code
        [NotMapped]
        public int UserId 
        { 
            get { return MusteriId; } 
            set { MusteriId = value; } 
        }

        [Column("ToplamTutar")]
        public decimal TotalPrice { get; set; }
        
        [Column("TeslimatAdresi")]
        [StringLength(500)]
        public string DeliveryAddress { get; set; }
        
        [Column("OdemeYontemi")]
        [StringLength(50)]
        public string PaymentMethod { get; set; }
        
        [Column("SiparisTarihi")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Order status - stored in database
        [Column("SiparisDurumu")]
        [StringLength(50)]
        public string OrderStatus { get; set; } = "Onaylandi";
        
        // وقت التجهيز المتوقع بالدقائق
        [Column("TahminiHazirlanmaSuresi")]
        public int TahminiHazirlanmaSuresi { get; set; } = 30;
        
        // الأوقات الثابتة بالدقائق
        public const int OnaylamaSuresi = 10;    // مرحلة التأكيد: 10 دقائق
        public const int TeslimatSuresi = 35;    // مرحلة التوصيل: 35 دقيقة
        
        // وقت انتهاء التأكيد (تاريخ ووقت)
        [NotMapped]
        public DateTime OnayBitisZamani
        {
            get { return OrderDate.AddMinutes(OnaylamaSuresi); }
        }
        
        // وقت انتهاء التحضير (تاريخ ووقت)
        [NotMapped]
        public DateTime HazirlanmaZamani
        {
            get { return OrderDate.AddMinutes(OnaylamaSuresi + TahminiHazirlanmaSuresi); }
        }
        
        // وقت التسليم النهائي (تأكيد + تحضير + توصيل)
        [NotMapped]
        public DateTime TahminiTeslimZamani
        {
            get { return OrderDate.AddMinutes(OnaylamaSuresi + TahminiHazirlanmaSuresi + TeslimatSuresi); }
        }
        
        // الوقت المتبقي للتحضير بالدقائق
        [NotMapped]
        public int KalanHazirlanmaSuresi
        {
            get 
            { 
                var remaining = (HazirlanmaZamani - DateTime.Now).TotalMinutes;
                return remaining > 0 ? (int)remaining : 0;
            }
        }
        
        // الوقت المتبقي للتسليم الكلي بالدقائق
        [NotMapped]
        public int KalanSure
        {
            get 
            { 
                var remaining = (TahminiTeslimZamani - DateTime.Now).TotalMinutes;
                return remaining > 0 ? (int)remaining : 0;
            }
        }
        
        // نسبة التقدم (0-100)
        [NotMapped]
        public int IlerlemeYuzdesi
        {
            get
            {
                var toplamSure = OnaylamaSuresi + TahminiHazirlanmaSuresi + TeslimatSuresi;
                var gecenSure = (DateTime.Now - OrderDate).TotalMinutes;
                var yuzde = (gecenSure / toplamSure) * 100;
                return yuzde > 100 ? 100 : (int)yuzde;
            }
        }
        
        [NotMapped]
        public List<CartItemViewModel> CartItems { get; set; }
        
        [NotMapped]
        public virtual Musteri Musteri { get; set; }
    }
}