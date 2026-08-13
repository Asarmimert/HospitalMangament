namespace HospitalManagementAPI.DTOs.Doctors
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }

        public int KullaniciHesabiId { get; set; }

        public string Eposta { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string DepartmanAdi { get; set; } =
            string.Empty;

        public string DoktorAd { get; set; } =
            string.Empty;

        public string DoktorSoyad { get; set; } =
            string.Empty;

        public string? TelefonNumarasi { get; set; }

        public string? UzmanlikAlani { get; set; }

        public bool AktifMi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}