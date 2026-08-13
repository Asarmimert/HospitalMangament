using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class ReceteDeposu : IReceteDeposu
    {
        private readonly HospitalDbContext _context;

        public ReceteDeposu(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<
            (List<Recete> Receteler, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? hastaId,
                int? doktorId,
                DateTime? baslangicTarihi,
                DateTime? bitisTarihi)
        {
            var sorgu = _context.Set<Recete>()
                .AsNoTracking()
                .Include(x => x.Muayene)
                .Include(x => x.Hasta)
                .Include(x => x.Doktor)
                .AsQueryable();

            if (hastaId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.HastaId == hastaId.Value);
            }

            if (doktorId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.DoktorId == doktorId.Value);
            }

            if (baslangicTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.ReceteTarihi >=
                         baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.ReceteTarihi <=
                         bitisTarihi.Value);
            }

            var toplamKayitSayisi =
                await sorgu.CountAsync();

            var receteler = await sorgu
                .OrderByDescending(x => x.ReceteTarihi)
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            return (receteler, toplamKayitSayisi);
        }

        public async Task<Recete?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Set<Recete>()
                .AsNoTracking()
                .Include(x => x.Muayene)
                .Include(x => x.Hasta)
                .Include(x => x.Doktor)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool>
            MuayeneninRecetesiVarMiAsync(int muayeneId)
        {
            return await _context.Set<Recete>()
                .AnyAsync(x => x.MuayeneId == muayeneId);
        }
    }
}