using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IRandevuDeposu
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

        Task<Randevu?> IliskileriyleIdIleGetirAsync(int id);

        Task<bool> DoktorRandevusuCakisiyorMuAsync(
            int doktorId,
            DateTime baslangicZamani,
            DateTime bitisZamani,
            int? haricTutulacakRandevuId = null);
    }
}