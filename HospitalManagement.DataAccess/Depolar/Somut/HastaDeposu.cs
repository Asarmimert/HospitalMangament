using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
         string? arama,
         CancellationToken cancellationToken = default)
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
                var aramaDeseni = $"%{temizArama}%";

                var tarihFormatlari = new[]
                {
        "dd.MM.yyyy",
        "dd/MM/yyyy",
        "yyyy-MM-dd"
    };

                var tarihMi = DateOnly.TryParseExact(
                    temizArama,
                    tarihFormatlari,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var arananTarih);

                if (tarihMi)
                {
                    sorgu = sorgu.Where(x =>
                        EF.Functions.ILike(
                            x.Ad,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.Soyad,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.KimlikNumarasi,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.TelefonNumarasi ?? "",
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.KullaniciHesabi.Eposta,
                            aramaDeseni) ||

                        x.DogumTarihi == arananTarih);
                }
                else
                {
                    sorgu = sorgu.Where(x =>
                        EF.Functions.ILike(
                            x.Ad,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.Soyad,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.KimlikNumarasi,
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.TelefonNumarasi ?? "",
                            aramaDeseni) ||

                        EF.Functions.ILike(
                            x.KullaniciHesabi.Eposta,
                            aramaDeseni));
                }
            }
            var toplamKayitSayisi =
    await sorgu.CountAsync(cancellationToken);

            var hastalar = await sorgu
    .OrderBy(x => x.Ad)
    .ThenBy(x => x.Soyad)
    .Skip((sayfaNo - 1) * sayfaBoyutu)
    .Take(sayfaBoyutu)
    .ToListAsync(cancellationToken);

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
        public async Task<Hasta?>
    KullaniciHesabiIdIleGetirAsync(
        int kullaniciHesabiId)
        {
            return await _context.Hastalar
                .AsNoTracking()
                .Include(x => x.KullaniciHesabi)
                .FirstOrDefaultAsync(
                    x => x.KullaniciHesabiId ==
                         kullaniciHesabiId &&
                         x.AktifMi);
        }
    }
}