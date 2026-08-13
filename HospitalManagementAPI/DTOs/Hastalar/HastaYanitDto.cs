namespace HospitalManagementAPI.DTOs.Patients
{
    public class PatientResponseDto
    {
        public int Id { get; set; }

        public int KullaniciHesabiId { get; set; }

        public string Eposta { get; set; } = string.Empty;

        public string Ad { get; set; } = string.Empty;

        public string Soyad { get; set; } = string.Empty;

        public string KimlikNumarasi { get; set; } =
            string.Empty;

        public DateOnly DogumTarihi { get; set; }

        public string TelefonNumarasi { get; set; } =
            string.Empty;

        public string? Adres { get; set; }

        public bool AktifMi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }
    }
}