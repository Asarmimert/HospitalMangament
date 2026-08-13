namespace HospitalManagementAPI.DTOs.MuayeneTeshisleri
{
    public class MuayeneTeshisiYanitDto
    {
        public int Id { get; set; }

        public int MuayeneId { get; set; }

        public int TeshisId { get; set; }

        public string? TeshisKodu { get; set; }

        public string TeshisAdi { get; set; } =
            string.Empty;

        public string? DoktorNotu { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}