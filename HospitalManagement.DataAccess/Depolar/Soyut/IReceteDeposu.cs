using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IReceteDeposu
    {
        Task<
            (List<Recete> Receteler, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? hastaId,
                int? doktorId,
                DateTime? baslangicTarihi,
                DateTime? bitisTarihi);

        Task<Recete?> IliskileriyleIdIleGetirAsync(
            int id);

        Task<bool> MuayeneninRecetesiVarMiAsync(
            int muayeneId);
    }
}