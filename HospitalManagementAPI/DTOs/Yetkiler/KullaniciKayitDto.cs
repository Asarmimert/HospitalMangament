using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Yetkiler
{
    public class KullaniciKayitDto
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [MaxLength(
            150,
            ErrorMessage = "E-posta en fazla 150 karakter olabilir.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola zorunludur.")]
        [MinLength(
            8,
            ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        [MaxLength(
            100,
            ErrorMessage = "Parola en fazla 100 karakter olabilir.")]
        public string Parola { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ad zorunludur.")]
        [MaxLength(20)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(25)]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "T.C. kimlik numarası zorunludur.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage = "T.C. kimlik numarası 11 rakam olmalıdır.")]
        public string KimlikNumarasi { get; set; } =
            string.Empty;

        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        public DateOnly DogumTarihi { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage = "Telefon numarası 11 rakam olmalıdır.")]
        public string TelefonNumarasi { get; set; } =
            string.Empty;

        [MaxLength(300)]
        public string? Adres { get; set; }
    }
}