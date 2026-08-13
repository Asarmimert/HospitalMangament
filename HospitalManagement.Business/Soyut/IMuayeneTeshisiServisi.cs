using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IMuayeneTeshisiServisi
    {
        Task<List<MuayeneTeshisi>>
            MuayeneyeGoreListeleAsync(int muayeneId);

        Task<MuayeneTeshisi?> IdIleGetirAsync(int id);

        Task<MuayeneTeshisi> EkleAsync(
            MuayeneTeshisi muayeneTeshisi);
    }
}