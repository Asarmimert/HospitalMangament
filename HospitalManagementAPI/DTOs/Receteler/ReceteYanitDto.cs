namespace HospitalManagementAPI.DTOs.Receteler
{
    public class ReceteYanitDto
    {
        public int Id { get; set; }

        public int MuayeneId { get; set; }

        public int HastaId { get; set; }

        public string HastaAdiSoyadi { get; set; }
            = string.Empty;

        public int DoktorId { get; set; }

        public string DoktorAdiSoyadi { get; set; }
            = string.Empty;

        public DateTime ReceteTarihi { get; set; }

        public string? GenelNotlar { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}