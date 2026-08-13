using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Doctors
{
    public class DoctorFilterDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Sayfa numarası en az 1 olmalıdır.")]
        public int SayfaNo { get; set; } = 1;

        [Range(
            1,
            100,
            ErrorMessage = "Sayfa boyutu 1 ile 100 arasında olmalıdır.")]
        public int SayfaBoyutu { get; set; } = 10;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir departman seçiniz.")]
        public int? DepartmentId { get; set; }

        public bool? AktifMi { get; set; }

        [MaxLength(
            65,
            ErrorMessage = "Arama metni en fazla 65 karakter olabilir.")]
        public string? Arama { get; set; }
    }
}