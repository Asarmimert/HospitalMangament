namespace HospitalManagementAPI.Ayarlar
{
    public class JwtAyarlari
    {
        public const string BolumAdi = "Jwt";

        public string Anahtar { get; set; } = string.Empty;

        public string Veren { get; set; } = string.Empty;

        public string Hedef { get; set; } = string.Empty;

        public int GecerlilikDakikasi { get; set; }
    }
}