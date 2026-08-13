using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class ReceteIcerikDeposu : IReceteIcerikDeposu
    {
        private readonly HospitalDbContext _context;

        public ReceteIcerikDeposu(
            HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReceteIcerik>>
            ReceteyeGoreListeleAsync(int receteId)
        {
            return await _context.Set<ReceteIcerik>()
                .AsNoTracking()
                .Include(x => x.Ilac)
                .Where(x => x.ReceteId == receteId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<ReceteIcerik?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Set<ReceteIcerik>()
                .AsNoTracking()
                .Include(x => x.Ilac)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AyniIlacVarMiAsync(
            int receteId,
            int ilacId)
        {
            return await _context.Set<ReceteIcerik>()
                .AnyAsync(
                    x => x.ReceteId == receteId &&
                         x.IlacId == ilacId);
        }
    }
}