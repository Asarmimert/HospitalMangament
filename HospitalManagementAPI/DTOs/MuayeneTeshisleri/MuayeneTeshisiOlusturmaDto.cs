using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.MuayeneTeshisleri
{
    public class MuayeneTeshisiOlusturmaDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir muayene seçiniz.")]
        public int MuayeneId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir teşhis seçiniz.")]
        public int TeshisId { get; set; }

        public string? DoktorNotu { get; set; }
    }
}