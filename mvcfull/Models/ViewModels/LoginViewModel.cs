using System;
using System.ComponentModel.DataAnnotations;

namespace mvc_full.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email gerekli")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Þifre gerekli")]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre")]
        public string Sifre { get; set; }

        [Display(Name = "Beni Hatýrla")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad gerekli")]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad gerekli")]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Email gerekli")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon gerekli")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarasý giriniz")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; }

        [Display(Name = "Adres")]
        [StringLength(500)]
        public string Adres { get; set; }

        [Required(ErrorMessage = "Þifre gerekli")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr")]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Þifre tekrarý gerekli")]
        [DataType(DataType.Password)]
        [Display(Name = "Þifre Tekrar")]
        [Compare("Sifre", ErrorMessage = "Þifreler eþleþmiyor")]
        public string SifreTekrar { get; set; }
    }
}
