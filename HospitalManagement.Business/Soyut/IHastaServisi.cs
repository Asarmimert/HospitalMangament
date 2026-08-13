using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IHastaServisi
    {
        Task<Hasta?> KullaniciHesabiIdIleGetirAsync(
        int kullaniciHesabiId);
        Task<(List<Hasta> Hastalar, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                bool? aktifMi,
                string? arama);

        Task<Hasta?> IdIleGetirAsync(int id);

        Task<Hasta> EkleAsync(Hasta hasta);

        Task<bool> GuncelleAsync(Hasta hasta);

        Task<bool> PasiflestirAsync(int id);
        
    }
}