using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IMuayeneServisi
    {
        Task<(List<Muayene> Muayeneler, int ToplamKayitSayisi)>
    ListeleAsync(
        int sayfaNo,
        int sayfaBoyutu,
        int? doktorId,
        int? hastaId,
        DateTime? baslangicTarihi,
        DateTime? bitisTarihi,
        CancellationToken cancellationToken = default);

        Task<Muayene?> IdIleGetirAsync(int id);

        Task<Muayene> EkleAsync(Muayene muayene);

        Task<bool> GuncelleAsync(Muayene muayene);
    }
}