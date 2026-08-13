using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Soyut
{
    public interface IRandevuServisi
    {
        Task<
    (List<Randevu> Randevular, int ToplamKayitSayisi)>
    ListeleAsync(
        int sayfaNo,
        int sayfaBoyutu,
        int? doktorId,
        int? hastaId,
        RandevuDurumu? durum,
        DateTime? baslangicTarihi,
        DateTime? bitisTarihi,
        CancellationToken cancellationToken = default);

        Task<Randevu?> IdIleGetirAsync(int id);

        Task<Randevu> EkleAsync(Randevu randevu);

        Task<bool> GuncelleAsync(Randevu randevu);

        Task<bool> DurumGuncelleAsync(
            int id,
            RandevuDurumu yeniDurum,
            string? iptalNedeni);
    }
}