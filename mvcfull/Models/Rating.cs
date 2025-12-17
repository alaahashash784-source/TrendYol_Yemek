using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_full.Models
{
    /// <summary>
    /// نموذج التقييمات والتعليقات
    /// يسمح للمستخدمين بتقييم الأطعمة والمطاعم
    /// </summary>
    [Table("Ratings")]
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RatingId { get; set; }
        
        // معرف المستخدم الذي قام بالتقييم
        [Column("MusteriId")]
        [Required]
        public int MusteriId { get; set; }
        
        // معرف الطعام المُقيَّم (اختياري)
        [Column("YemekId")]
        public int? YemekId { get; set; }
        
        // معرف المطعم المُقيَّم (اختياري)
        [Column("RestoranId")]
        public int? RestoranId { get; set; }
        
        // معرف الطلب المرتبط (للتأكد من أن المستخدم طلب فعلاً)
        [Column("OrderId")]
        public int? OrderId { get; set; }
        
        // التقييم بالنجوم (1-5)
        [Column("Puan")]
        [Required]
        [Range(1, 5, ErrorMessage = "Puan 1-5 arasinda olmalidir")]
        public int Puan { get; set; }
        
        // التعليق النصي
        [Column("Yorum")]
        [StringLength(1000)]
        public string Yorum { get; set; }
        
        // تاريخ التقييم
        [Column("DegerlendirmeTarihi")]
        public DateTime DegerlendirmeTarihi { get; set; } = DateTime.Now;
        
        // هل التقييم موافق عليه من الإدارة (للفلترة)
        [Column("Onaylandi")]
        public bool Onaylandi { get; set; } = true;
        
        // العلاقات
        [ForeignKey("MusteriId")]
        public virtual Musteri Musteri { get; set; }
        
        [ForeignKey("YemekId")]
        public virtual Yemek Yemek { get; set; }
        
        [ForeignKey("RestoranId")]
        public virtual Restoran Restoran { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual OrderPage Order { get; set; }
    }
}
