namespace HospitalManagementAPI.DTOs.Teshisler
{
    public class TeshisYanitDto
    {
        public int Id { get; set; }

        public string? TeshisKodu { get; set; }

        public string TeshisAdi { get; set; } =
            string.Empty;

        public string? Aciklama { get; set; }

        public bool AktifMi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}