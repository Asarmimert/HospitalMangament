using HospitalManagement.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Randevular
{
    public class RandevuFiltrelemeDto
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

        public int? DoktorId { get; set; }

        public int? HastaId { get; set; }

        public RandevuDurumu? Durum { get; set; }

        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }
}