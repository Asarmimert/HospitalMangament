using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IHastaDeposu
    {
        Task<(List<Hasta> Hastalar, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                bool? aktifMi,
                string? arama);

        Task<Hasta?> IliskileriyleIdIleGetirAsync(int id);
    }
}