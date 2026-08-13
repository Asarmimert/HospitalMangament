using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Receteler
{
    public class ReceteOlusturmaDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir muayene seçiniz.")]
        public int MuayeneId { get; set; }

        public string? GenelNotlar { get; set; }
    }
}