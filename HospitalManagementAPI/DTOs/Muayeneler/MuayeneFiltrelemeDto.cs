using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Muayeneler
{
    public class MuayeneFiltrelemeDto
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
            ErrorMessage = "Geçerli bir doktor seçiniz.")]
        public int? DoktorId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Geçerli bir hasta seçiniz.")]
        public int? HastaId { get; set; }

        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }
}