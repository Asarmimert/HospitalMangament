namespace HospitalManagementAPI.DTOs.Muayeneler
{
    public class MuayeneYanitDto
    {
        public int Id { get; set; }

        public int RandevuId { get; set; }

        public int DoktorId { get; set; }

        public string DoktorAdiSoyadi { get; set; } =
            string.Empty;

        public int HastaId { get; set; }

        public string HastaAdiSoyadi { get; set; } =
            string.Empty;

        public string HastaSikayeti { get; set; } =
            string.Empty;

        public string DoktorDegerlendirmesi { get; set; } =
            string.Empty;

        public string? DoktorNotlari { get; set; }

        public DateTime MuayeneTarihi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}