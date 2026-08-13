using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Patients
{
    public class CreatePatientDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kullanıcı hesabı seçiniz.")]
        public int KullaniciHesabiId { get; set; }

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

        [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
        public DateOnly DogumTarihi { get; set; }

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
    }
}