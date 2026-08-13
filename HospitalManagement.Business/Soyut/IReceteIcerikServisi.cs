using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IReceteIcerikServisi
    {
        Task<List<ReceteIcerik>>
            ReceteyeGoreListeleAsync(int receteId);

        Task<ReceteIcerik?> IdIleGetirAsync(int id);

        Task<ReceteIcerik> EkleAsync(
            ReceteIcerik receteIcerik);

        Task<bool> GuncelleAsync(
            ReceteIcerik receteIcerik);

        Task<bool> SilAsync(int id);
    }
}