using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Muayeneler
{
    public class MuayeneGuncellemeDto
    {
        [Required(ErrorMessage = "Hasta şikâyeti zorunludur.")]
        public string HastaSikayeti { get; set; } =
            string.Empty;

        [Required(
            ErrorMessage = "Doktor değerlendirmesi zorunludur.")]
        public string DoktorDegerlendirmesi { get; set; } =
            string.Empty;

        public string? DoktorNotlari { get; set; }

        [Required(ErrorMessage = "Muayene tarihi zorunludur.")]
        public DateTime MuayeneTarihi { get; set; }
    }
}