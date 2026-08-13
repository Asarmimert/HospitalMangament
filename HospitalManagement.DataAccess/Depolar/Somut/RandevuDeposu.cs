using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class RandevuDeposu : IRandevuDeposu
    {
        private readonly HospitalDbContext _context;

        public RandevuDeposu(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<
     (List<Randevu> Randevular, int ToplamKayitSayisi)>
     ListeleAsync(
         int sayfaNo,
         int sayfaBoyutu,
         int? doktorId,
         int? hastaId,
         RandevuDurumu? durum,
         DateTime? baslangicTarihi,
         DateTime? bitisTarihi,
         CancellationToken cancellationToken = default)
        {
            var sorgu = _context.Randevular
                .AsNoTracking()
                .Include(x => x.Doktor)
                .Include(x => x.Hasta)
                .Include(x => x.OlusturanSekreter)
                .AsQueryable();

            if (doktorId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.DoktorId == doktorId.Value);
            }

            if (hastaId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.HastaId == hastaId.Value);
            }

            if (durum.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.Durum == durum.Value);
            }

            if (baslangicTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.BaslangicZamani >=
                         baslangicTarihi.Value);
            }

            if (bitisTarihi.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.BitisZamani <=
                         bitisTarihi.Value);
            }

            var toplamKayitSayisi =
                await sorgu.CountAsync(cancellationToken);

            var randevular = await sorgu
                .OrderBy(x => x.BaslangicZamani)
                .Skip((sayfaNo - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync(cancellationToken);

            return (randevular, toplamKayitSayisi);
        }
        public async Task<Randevu?>
            IliskileriyleIdIleGetirAsync(int id)
        {
            return await _context.Randevular
                .AsNoTracking()
                .Include(x => x.Doktor)
                .Include(x => x.Hasta)
                .Include(x => x.OlusturanSekreter)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool>
            DoktorRandevusuCakisiyorMuAsync(
                int doktorId,
                DateTime baslangicZamani,
                DateTime bitisZamani,
                int? haricTutulacakRandevuId = null)
        {
            var sorgu = _context.Randevular
                .AsNoTracking()
                .Where(x =>
                    x.DoktorId == doktorId &&
                    x.Durum == RandevuDurumu.Planlandi &&
                    x.BaslangicZamani < bitisZamani &&
                    x.BitisZamani > baslangicZamani);

            if (haricTutulacakRandevuId.HasValue)
            {
                sorgu = sorgu.Where(
                    x => x.Id !=
                         haricTutulacakRandevuId.Value);
            }

            return await sorgu.AnyAsync();
        }
    }
}