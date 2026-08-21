using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Entities;
namespace HospitalManagement.Business.Soyut
{
    public interface ISekreterServisi
    {
        Task<Sekreter?> KullaniciHesabiIdIleGetirAsync(
            int kullaniciHesabiId);
        Task<Sekreter> HesabiylaBirlikteEkleAsync(
    Sekreter sekreter,
    KullaniciHesabi kullaniciHesabi);
    }
}