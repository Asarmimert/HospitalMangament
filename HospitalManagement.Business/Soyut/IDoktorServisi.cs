using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IDoktorServisi
    {
        Task<(List<Doctor> Doktorlar, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? departmentId,
                bool? aktifMi,
                string? arama,
                CancellationToken cancellationToken = default);

        Task<Doctor?> IdIleGetirAsync(int id);

        Task<Doctor> EkleAsync(Doctor doktor);
        Task<Doctor> HesabiylaBirlikteEkleAsync(
    Doctor doktor,
    KullaniciHesabi kullaniciHesabi);

        Task<bool> GuncelleAsync(Doctor doktor);

        Task<bool> PasiflestirAsync(int id);
        Task<Doctor?> KullaniciHesabiIdIleGetirAsync(
        int kullaniciHesabiId);
    }
}