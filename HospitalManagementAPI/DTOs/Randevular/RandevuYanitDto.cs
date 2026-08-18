using HospitalManagement.Entity.Enums;

namespace HospitalManagementAPI.DTOs.Randevular
{
    public class RandevuYanitDto
    {
        public int Id { get; set; }

        public int DoktorId { get; set; }

        public string DoktorAdiSoyadi { get; set; } =
            string.Empty;

        public int HastaId { get; set; }

        public string HastaAdiSoyadi { get; set; } =
            string.Empty;
        public string? DoktorUzmanlikAlani { get; set; }
        public int? OlusturanSekreterId { get; set; }

        public string? OlusturanSekreterAdiSoyadi { get; set; } 
           

        public DateTime BaslangicZamani { get; set; }

        public DateTime BitisZamani { get; set; }

        public RandevuDurumu Durum { get; set; }

        public string DurumAdi { get; set; } = string.Empty;

        public string? IptalNedeni { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}