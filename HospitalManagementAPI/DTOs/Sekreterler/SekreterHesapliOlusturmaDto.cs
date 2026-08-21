using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Sekreterler
{
    public class SekreterHesapliOlusturmaDto
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [RegularExpression(
    @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$",
    ErrorMessage =
        "E-posta adresinde Türkçe karakter kullanılamaz.")]
        [MaxLength(150)]
        public string Eposta { get; set; } = null!;

        [Required(ErrorMessage = "Parola zorunludur.")]
        [MinLength(
            8,
            ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        public string Parola { get; set; } = null!;

        [Required(ErrorMessage = "Ad zorunludur.")]
        [MaxLength(30)]
        public string Ad { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(30)]
        public string Soyad { get; set; } = null!;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage =
                "Telefon numarası 11 rakamdan oluşmalıdır.")]
        public string TelefonNumarasi { get; set; } = null!;
    }
}