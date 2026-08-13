using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IIlacServisi
    {
        Task<List<Ilac>> TumunuGetirAsync();

        Task<Ilac?> IdIleGetirAsync(int id);

        Task<Ilac> EkleAsync(Ilac ilac);

        Task<bool> GuncelleAsync(Ilac ilac);

        Task<bool> PasiflestirAsync(int id);
    }
}