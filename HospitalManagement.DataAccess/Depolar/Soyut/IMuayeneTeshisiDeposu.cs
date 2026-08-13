using HospitalManagement.Entity.Entities;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IMuayeneTeshisiDeposu
    {
        Task<List<MuayeneTeshisi>>
            MuayeneyeGoreListeleAsync(int muayeneId);

        Task<MuayeneTeshisi?>
            IliskileriyleIdIleGetirAsync(int id);

        Task<bool> AyniTeshisVarMiAsync(
            int muayeneId,
            int teshisId);
    }
}