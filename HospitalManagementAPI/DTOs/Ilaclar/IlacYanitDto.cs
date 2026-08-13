namespace HospitalManagementAPI.DTOs.Ilaclar
{
    public class IlacYanitDto
    {
        public int Id { get; set; }

        public string Ad { get; set; } = string.Empty;

        public bool AktifMi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}