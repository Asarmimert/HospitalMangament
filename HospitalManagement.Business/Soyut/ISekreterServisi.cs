using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface ISekreterServisi
    {
        Task<Sekreter?> KullaniciHesabiIdIleGetirAsync(
            int kullaniciHesabiId);
    }
}