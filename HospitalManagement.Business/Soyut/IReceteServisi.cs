using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IReceteServisi
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

        Task<Recete?> IdIleGetirAsync(int id);

        Task<Recete> EkleAsync(Recete recete);

        Task<bool> GuncelleAsync(Recete recete);
    }
}