using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface ITeshisServisi
    {
        Task<List<Teshis>> TumunuGetirAsync();

        Task<Teshis?> IdIleGetirAsync(int id);

        Task<Teshis> EkleAsync(Teshis teshis);

        Task<bool> GuncelleAsync(Teshis teshis);

        Task<bool> PasiflestirAsync(int id);
    }
}