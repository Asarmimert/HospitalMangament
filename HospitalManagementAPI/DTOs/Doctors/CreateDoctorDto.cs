using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Doctors
{
    public class CreateDoctorDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir kullanıcı hesabı seçiniz.")]
        public int KullaniciHesabiId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir departman seçiniz.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Doktor adı zorunludur.")]
        [MaxLength(
            25,
            ErrorMessage = "Doktor adı en fazla 25 karakter olabilir.")]
        public string DoktorAd { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doktor soyadı zorunludur.")]
        [MaxLength(
            40,
            ErrorMessage = "Doktor soyadı en fazla 40 karakter olabilir.")]
        public string DoktorSoyad { get; set; } = string.Empty;

        [MaxLength(
            11,
            ErrorMessage = "Telefon numarası en fazla 11 karakter olabilir.")]
        [RegularExpression(
            @"^\d{11}$",
            ErrorMessage = "Telefon numarası 11 rakamdan oluşmalıdır.")]
        public string? TelefonNumarasi { get; set; }

        [MaxLength(
            35,
            ErrorMessage = "Uzmanlık alanı en fazla 35 karakter olabilir.")]
        public string? UzmanlikAlani { get; set; }
    }
}