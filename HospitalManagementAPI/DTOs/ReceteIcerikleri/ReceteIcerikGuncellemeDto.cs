using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.ReceteIcerikleri
{
    public class ReceteIcerikGuncellemeDto
    {
        [Required(ErrorMessage = "Kullanım talimatı zorunludur.")]
        [MaxLength(500)]
        public string KullanimTalimatlari { get; set; }
            = string.Empty;

        [Required(ErrorMessage = "Kullanım süresi zorunludur.")]
        [MaxLength(100)]
        public string KullanimSuresi { get; set; }
            = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Miktar { get; set; }
    }
}