using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class MuayeneDeposu : IMuayeneDeposu
    {
        private readonly HospitalDbContext _context;

        public MuayeneDeposu(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<
            (List<Muayene> Muayeneler,
            int ToplamKayitSayisi)> ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? doktorId,
                int? hastaId,
                DateTime? baslangicTarihi,
                DateTime? bitisTarihi)
        {
            var sorgu = _context.Muayeneler
                .AsNoTracking()
                .Include(x => x.Randevu)
                    .ThenInclude(x => x.Doktor)
                .Include(x => x.Randevu)
                    .ThenInclude(x => x.Hasta)
                .AsQueryable();

            if (doktorId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.Randevu.DoktorId ==
                         doktorId.Value);
            }

            if (hastaId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.Randevu.HastaId ==
                         hastaId.Value);
            }

            if (baslangicTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.MuayeneTarihi >=
                         baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.MuayeneTarihi <=
                         bitisTarihi.Value);
            }

            var toplamKayitSayisi =
                await sorgu.CountAsync();

            var muayeneler = await sorgu
                .OrderByDescending(x => x.MuayeneTarihi)
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            return (muayeneler, toplamKayitSayisi);
        }

        public async Task<Muayene?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Muayeneler
                .AsNoTracking()
                .Include(x => x.Randevu)
                    .ThenInclude(x => x.Doktor)
                .Include(x => x.Randevu)
                    .ThenInclude(x => x.Hasta)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool>
            RandevuIcinMuayeneVarMiAsync(
                int randevuId)
        {
            return await _context.Muayeneler
                .AsNoTracking()
                .AnyAsync(
                    x => x.RandevuId == randevuId);
        }
    }
}