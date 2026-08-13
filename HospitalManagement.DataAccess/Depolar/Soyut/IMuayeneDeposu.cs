using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IMuayeneDeposu
    {
        Task<(List<Muayene> Muayeneler,
            int ToplamKayitSayisi)> ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? doktorId,
                int? hastaId,
                DateTime? baslangicTarihi,
                DateTime? bitisTarihi);

        Task<Muayene?> IliskileriyleIdIleGetirAsync(
            int id);

        Task<bool> RandevuIcinMuayeneVarMiAsync(
            int randevuId);
    }
}