namespace HospitalManagementAPI.DTOs.Common
{
    public class SayfalanmisResponseDto<T>
    {
        public List<T> Kayitlar { get; set; } = new();

        public int SayfaNo { get; set; }

        public int SayfaBoyutu { get; set; }

        public int ToplamKayitSayisi { get; set; }

        public int ToplamSayfaSayisi { get; set; }

        public bool OncekiSayfaVarMi =>
            SayfaNo > 1;

        public bool SonrakiSayfaVarMi =>
            SayfaNo < ToplamSayfaSayisi;
    }
}