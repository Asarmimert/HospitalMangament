using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class DoktorDeposu : IDoktorDeposu
    {
        private readonly HospitalDbContext _context;

        public DoktorDeposu(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<
            (List<Doctor> Doktorlar, int ToplamKayitSayisi)>
            ListeleAsync(
                int sayfaNo,
                int sayfaBoyutu,
                int? departmentId,
                bool? aktifMi,
                string? arama)
        {
            var sorgu = _context.Doctors
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.KullaniciHesabi)
                .AsQueryable();

            if (departmentId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.DepartmentId ==
                         departmentId.Value);
            }

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
                             x.DoktorAd,
                             $"%{temizArama}%")
                         || EF.Functions.ILike(
                             x.DoktorSoyad,
                             $"%{temizArama}%"));
            }

            var toplamKayitSayisi =
                await sorgu.CountAsync();

            var doktorlar = await sorgu
                .OrderBy(x => x.DoktorAd)
                .ThenBy(x => x.DoktorSoyad)
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();

            return (doktorlar, toplamKayitSayisi);
        }

        public async Task<Doctor?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.KullaniciHesabi)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}