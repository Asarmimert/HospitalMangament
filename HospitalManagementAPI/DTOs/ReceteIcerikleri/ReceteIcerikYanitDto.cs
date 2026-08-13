namespace HospitalManagementAPI.DTOs.ReceteIcerikleri
{
    public class ReceteIcerikYanitDto
    {
        public int Id { get; set; }

        public int ReceteId { get; set; }

        public int IlacId { get; set; }

        public string IlacAdi { get; set; } = string.Empty;

        public string KullanimTalimatlari { get; set; }
            = string.Empty;

        public string KullanimSuresi { get; set; }
            = string.Empty;

        public int Miktar { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}