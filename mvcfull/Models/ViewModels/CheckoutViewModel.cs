using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvc_full.Models
{
    public class CheckoutViewModel
    {
        public CheckoutViewModel()
        {
            SepetUrunleri = new List<CartItemViewModel>();
        }

        // Delivery Information
        [Required(ErrorMessage = "Ad Soyad gerekli")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100)]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Telefon gerekli")]
        [Phone(ErrorMessage = "Gecerli bir telefon numarasi giriniz")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Adres gerekli")]
        [Display(Name = "Adres")]
        [StringLength(500)]
        public string Adres { get; set; }

        [Display(Name = "Siparis Notu")]
        [StringLength(500)]
        public string SiparisNotu { get; set; }

        // Payment Information
        [Required(ErrorMessage = "Odeme yontemi secin")]
        [Display(Name = "Odeme Yontemi")]
        public string OdemeYontemi { get; set; }

        // Cart Items
        public List<CartItemViewModel> SepetUrunleri { get; set; }

        // Totals
        [Display(Name = "Ara Toplam")]
        public decimal AraToplam { get; set; }

        [Display(Name = "Teslimat Ucreti")]
        public decimal TeslimatUcreti { get; set; }

        [Display(Name = "KDV")]
        public decimal KDV { get; set; }

        [Display(Name = "Genel Toplam")]
        public decimal GenelToplam { get; set; }
    }
}
