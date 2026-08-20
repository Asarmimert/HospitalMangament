namespace HospitalManagementAPI.DTOs.Patients
{
    public class CreatePatientWithAccountDto
    {
        public string Eposta { get; set; } = string.Empty;
        public string Parola { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string KimlikNumarasi { get; set; } = string.Empty;
        public string? TelefonNumarasi { get; set; }
        public string? Adres { get; set; }
        public DateOnly DogumTarihi { get; set; }
    }
}