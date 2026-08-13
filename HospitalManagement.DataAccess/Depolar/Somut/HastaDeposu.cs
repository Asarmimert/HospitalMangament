using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class HastaDeposu : IHastaDeposu
    {
        private readonly HospitalDbContext _context;

        public HastaDeposu(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<
            (List<Hasta> Hastalar, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                bool? aktifMi,
                string? arama)
        {
            var sorgu = _context.Hastalar
                .AsNoTracking()
                .Include(x => x.KullaniciHesabi)
                .AsQueryable();

            if (aktifMi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.AktifMi == aktifMi.Value);
            }

            if (!string.IsNullOrWhiteSpace(arama))
            {
                var temizArama = arama.Trim();

                sorgu = sorgu.Where(
                    x => EF.Functions.ILike(
                             x.Ad,
                             $"%{temizArama}%")
                         || EF.Functions.ILike(
                             x.Soyad,
                             $"%{temizArama}%"));
            }

            var toplamKayitSayisi =
                await sorgu.CountAsync();

            var hastalar = await sorgu
                .OrderBy(x => x.Ad)
                .ThenBy(x => x.Soyad)
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            return (hastalar, toplamKayitSayisi);
        }

        public async Task<Hasta?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Hastalar
                .AsNoTracking()
                .Include(x => x.KullaniciHesabi)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}