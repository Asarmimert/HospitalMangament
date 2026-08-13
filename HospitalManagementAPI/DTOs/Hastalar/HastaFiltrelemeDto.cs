using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Patients
{
    public class PatientFilterDto
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

        public bool? AktifMi { get; set; }

        [MaxLength(
            50,
            ErrorMessage = "Arama metni en fazla 50 karakter olabilir.")]
        public string? Arama { get; set; }
    }
}