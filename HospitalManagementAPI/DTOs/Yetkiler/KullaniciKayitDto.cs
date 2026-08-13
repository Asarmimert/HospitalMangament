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
    }
}