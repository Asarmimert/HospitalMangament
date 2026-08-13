using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IDoktorDeposu
    {
       Task<(List<Doctor> Doktorlar, int ToplamKayitSayisi)>
    ListeleAsync(
        int sayfaNo,
        int sayfaBoyutu,
        int? departmentId,
        bool? aktifMi,
        string? arama,
        CancellationToken cancellationToken = default);
        Task<Doctor?> IliskileriyleIdIleGetirAsync(int id);
    }
}