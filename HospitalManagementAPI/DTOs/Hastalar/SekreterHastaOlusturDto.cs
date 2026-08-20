//namespace HospitalManagementAPI.DTOs.Patients
//{
//    public class CreatePatientWithAccountDto
//    {
//        public string Eposta { get; set; } = string.Empty;
//        public string Parola { get; set; } = string.Empty;
//        public string Ad { get; set; } = string.Empty;
//        public string Soyad { get; set; } = string.Empty;
//        public string KimlikNumarasi { get; set; } = string.Empty;
//        public string? TelefonNumarasi { get; set; }
//        public string? Adres { get; set; }
//        public DateOnly DogumTarihi { get; set; }
//    }
//}

// Düzenli kurallara uyan yeni yapılan
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Patients
{
    public class CreatePatientWithAccountDto
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
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

        [Required(ErrorMessage = "Hasta adı zorunludur.")]
        [MaxLength(
            20,
            ErrorMessage = "Hasta adı en fazla 20 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasta soyadı zorunludur.")]
        [MaxLength(
            25,
            ErrorMessage = "Hasta soyadı en fazla 25 karakter olabilir.")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kimlik numarası zorunludur.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage = "Kimlik numarası 11 rakamdan oluşmalıdır.")]
        public string KimlikNumarasi { get; set; } =
            string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage = "Telefon numarası 11 rakamdan oluşmalıdır.")]
        public string TelefonNumarasi { get; set; } =
            string.Empty;

        [MaxLength(
            300,
            ErrorMessage = "Adres en fazla 300 karakter olabilir.")]
        public string? Adres { get; set; }

        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        public DateOnly DogumTarihi { get; set; }
    }
}