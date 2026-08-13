namespace HospitalManagementAPI.DTOs.Yetkiler
{
    public class GirisYanitDto
    {
        public int KullaniciId { get; set; }

        public string Eposta { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime TokenBitisTarihi { get; set; }
    }
}