using System.ComponentModel.DataAnnotations;

namespace HospitalManagementAPI.DTOs.Receteler
{
    public class ReceteFiltrelemeDto
    {
        [Range(1, int.MaxValue)]
        public int SayfaNo { get; set; } = 1;

        [Range(1, 100)]
        public int SayfaBoyutu { get; set; } = 10;

        public int? HastaId { get; set; }

        public int? DoktorId { get; set; }

        public DateTime? BaslangicTarihi { get; set; }

        public DateTime? BitisTarihi { get; set; }
    }
}