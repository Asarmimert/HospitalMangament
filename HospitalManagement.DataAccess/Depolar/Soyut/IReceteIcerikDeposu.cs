using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IReceteIcerikDeposu
    {
        Task<List<ReceteIcerik>>
            ReceteyeGoreListeleAsync(int receteId);

        Task<ReceteIcerik?>
            IliskileriyleIdIleGetirAsync(int id);

        Task<bool> AyniIlacVarMiAsync(
            int receteId,
            int ilacId);
    }
}