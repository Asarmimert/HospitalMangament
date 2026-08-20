using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Doctors
{
    public class CreateDoctorWithAccountDto
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(
            ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [MaxLength(
            150,
            ErrorMessage =
                "E-posta en fazla 150 karakter olabilir.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola zorunludur.")]
        [MinLength(
            8,
            ErrorMessage =
                "Parola en az 8 karakter olmalıdır.")]
        [MaxLength(
            100,
            ErrorMessage =
                "Parola en fazla 100 karakter olabilir.")]
        public string Parola { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Geçerli bir departman seçiniz.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Doktor adı zorunludur.")]
        [MaxLength(
            25,
            ErrorMessage =
                "Doktor adı en fazla 25 karakter olabilir.")]
        public string DoktorAd { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doktor soyadı zorunludur.")]
        [MaxLength(
            40,
            ErrorMessage =
                "Doktor soyadı en fazla 40 karakter olabilir.")]
        public string DoktorSoyad { get; set; } =
            string.Empty;

        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage =
                "Telefon numarası 11 rakamdan oluşmalıdır.")]
        public string? TelefonNumarasi { get; set; }

        [MaxLength(
            35,
            ErrorMessage =
                "Uzmanlık alanı en fazla 35 karakter olabilir.")]
        public string? UzmanlikAlani { get; set; }
    }
}