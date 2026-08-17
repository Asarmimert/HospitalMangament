using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Randevular
{
    public class RandevuOlusturmaDto
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir doktor seçiniz.")]
        public int DoktorId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir hasta seçiniz.")]
        public int HastaId { get; set; }

        [Required(
            ErrorMessage = "Başlangıç zamanı zorunludur.")]
        public DateTime BaslangicZamani { get; set; }

        [Required(
            ErrorMessage = "Bitiş zamanı zorunludur.")]
        public DateTime BitisZamani { get; set; }
    }
}