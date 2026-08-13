using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class MuayeneTeshisiDeposu
        : IMuayeneTeshisiDeposu
    {
        private readonly HospitalDbContext _context;

        public MuayeneTeshisiDeposu(
            HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<List<MuayeneTeshisi>>
            MuayeneyeGoreListeleAsync(int muayeneId)
        {
            return await _context.MuayeneTeshisleri
                .AsNoTracking()
                .Include(x => x.Teshis)
                .Where(x => x.MuayeneId == muayeneId)
                .OrderBy(x => x.Teshis.TeshisAdi)
                .ToListAsync();
        }

        public async Task<MuayeneTeshisi?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.MuayeneTeshisleri
                .AsNoTracking()
                .Include(x => x.Teshis)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AyniTeshisVarMiAsync(
            int muayeneId,
            int teshisId)
        {
            return await _context.MuayeneTeshisleri
                .AsNoTracking()
                .AnyAsync(
                    x => x.MuayeneId == muayeneId
                         && x.TeshisId == teshisId);
        }
    }
}