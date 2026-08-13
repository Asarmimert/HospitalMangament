using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Yetkiler
{
    public class KullaniciGirisDto
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola zorunludur.")]
        public string Parola { get; set; } = string.Empty;
    }
}