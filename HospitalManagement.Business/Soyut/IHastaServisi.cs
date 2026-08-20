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
          string? arama,
          CancellationToken cancellationToken = default);
        Task<Hasta?> IdIleGetirAsync(int id);

        Task<Hasta> HesabiylaBirlikteEkleAsync(
    Hasta hasta,
    KullaniciHesabi kullaniciHesabi);
        Task<Hasta> EkleAsync(Hasta hasta);

        Task<bool> GuncelleAsync(Hasta hasta);

        Task<bool> PasiflestirAsync(int id);
        
    }
}