using System;
using System.ComponentModel.DataAnnotations;

namespace mvc_full.Models
{
    // Sepet urun modeli
    public class CartItemViewModel
    {
        public int YemekId { get; set; }
        
        [Display(Name = "Yemek Adi")]
        public string YemekAdi { get; set; }
        
        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }
        
        [Display(Name = "Miktar")]
        [Range(1, 100, ErrorMessage = "Miktar 1 ile 100 arasinda olmalidir")]
        public int Miktar { get; set; }
        
        [Display(Name = "Toplam Fiyat")]
        public decimal ToplamFiyat
        {
            get { return Fiyat * Miktar; }
        }
        
        [Display(Name = "Resim URL")]
        public string ResimUrl { get; set; }
        
        [Display(Name = "Restoran")]
        public string RestoranAdi { get; set; }
        
        public int RestoranId { get; set; }
    }
}