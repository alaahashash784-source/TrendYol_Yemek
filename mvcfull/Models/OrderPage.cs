/*
╔══════════════════════════════════════════════════════════════════════════════╗
║                         نموذج الطلب (OrderPage)                             ║
╠══════════════════════════════════════════════════════════════════════════════╣
║  الغرض: تمثيل جدول الطلبات في قاعدة البيانات                                ║
║  الجدول: OrderPages                                                         ║
║  الحقول الرئيسية:                                                           ║
║    - OrderId: المفتاح الأساسي (Primary Key)                                 ║
║    - MusteriId: رقم العميل (Foreign Key)                                    ║
║    - TotalPrice: السعر الإجمالي (مع الضريبة 8%)                             ║
║    - DeliveryAddress: عنوان التوصيل                                         ║
║    - PaymentMethod: طريقة الدفع                                             ║
║    - OrderDate: تاريخ الطلب                                                 ║
║  خصائص NotMapped (لا تُخزن في القاعدة):                                     ║
║    - OrderStatus: حالة الطلب (للعرض فقط)                                    ║
║    - CartItems: عناصر السلة (للمعالجة فقط)                                  ║
║    - UserId: اسم بديل لـ MusteriId للتوافق                                  ║
╚══════════════════════════════════════════════════════════════════════════════╝
*/
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
        
        [NotMapped]
        public List<CartItemViewModel> CartItems { get; set; }
        
        [NotMapped]
        public virtual Musteri Musteri { get; set; }
    }
}