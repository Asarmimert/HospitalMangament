using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class TeshisServisi : ITeshisServisi
    {
        private readonly IGenelDepo<Teshis> _teshisDeposu;

        public TeshisServisi(
            IGenelDepo<Teshis> teshisDeposu)
        {
            _teshisDeposu = teshisDeposu;
        }

        public async Task<List<Teshis>> TumunuGetirAsync()
        {
            return await _teshisDeposu
                .KosulaGoreGetirAsync(x => x.AktifMi);
        }

        public async Task<Teshis?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            var teshis =
                await _teshisDeposu.IdIleGetirAsync(id);

            if (teshis is null || !teshis.AktifMi)
            {
                return null;
            }

            return teshis;
        }

        public async Task<Teshis> EkleAsync(Teshis teshis)
        {
            BilgileriDogrulaVeTemizle(teshis);

            if (teshis.TeshisKodu is not null)
            {
                var kodKullaniliyorMu =
                    await _teshisDeposu.VarMiAsync(
                        x => x.TeshisKodu ==
                             teshis.TeshisKodu);

                if (kodKullaniliyorMu)
                {
                    throw new InvalidOperationException(
                        "Bu teşhis kodu zaten kullanılıyor.");
                }
            }

            teshis.AktifMi = true;
            teshis.OlusturulmaTarihi = DateTime.UtcNow;
            teshis.GuncellenmeTarihi = null;

            await _teshisDeposu.EkleAsync(teshis);
            await _teshisDeposu.KaydetAsync();

            return teshis;
        }

        public async Task<bool> GuncelleAsync(Teshis teshis)
        {
            var mevcutTeshis =
                await _teshisDeposu.IdIleGetirAsync(
                    teshis.Id);

            if (mevcutTeshis is null ||
                !mevcutTeshis.AktifMi)
            {
                return false;
            }

            BilgileriDogrulaVeTemizle(teshis);

            if (teshis.TeshisKodu is not null)
            {
                var kodKullaniliyorMu =
                    await _teshisDeposu.VarMiAsync(
                        x => x.TeshisKodu ==
                             teshis.TeshisKodu
                             && x.Id != teshis.Id);

                if (kodKullaniliyorMu)
                {
                    throw new InvalidOperationException(
                        "Bu teşhis kodu başka bir kayıtta kullanılıyor.");
                }
            }

            mevcutTeshis.TeshisKodu =
                teshis.TeshisKodu;

            mevcutTeshis.TeshisAdi =
                teshis.TeshisAdi;

            mevcutTeshis.Aciklama =
                teshis.Aciklama;

            mevcutTeshis.GuncellenmeTarihi =
                DateTime.UtcNow;

            _teshisDeposu.Guncelle(mevcutTeshis);
            await _teshisDeposu.KaydetAsync();

            return true;
        }

        public async Task<bool> PasiflestirAsync(int id)
        {
            var teshis =
                await _teshisDeposu.IdIleGetirAsync(id);

            if (teshis is null || !teshis.AktifMi)
            {
                return false;
            }

            teshis.AktifMi = false;
            teshis.GuncellenmeTarihi = DateTime.UtcNow;

            _teshisDeposu.Guncelle(teshis);
            await _teshisDeposu.KaydetAsync();

            return true;
        }

        private static void BilgileriDogrulaVeTemizle(
            Teshis teshis)
        {
            if (string.IsNullOrWhiteSpace(
                    teshis.TeshisAdi))
            {
                throw new ArgumentException(
                    "Teşhis adı boş olamaz.");
            }

            teshis.TeshisAdi =
                teshis.TeshisAdi.Trim();

            if (teshis.TeshisAdi.Length > 250)
            {
                throw new ArgumentException(
                    "Teşhis adı en fazla 250 karakter olabilir.");
            }

            teshis.TeshisKodu =
                string.IsNullOrWhiteSpace(
                    teshis.TeshisKodu)
                    ? null
                    : teshis.TeshisKodu
                        .Trim()
                        .ToUpperInvariant();

            if (teshis.TeshisKodu?.Length > 100)
            {
                throw new ArgumentException(
                    "Teşhis kodu en fazla 100 karakter olabilir.");
            }

            teshis.Aciklama =
                string.IsNullOrWhiteSpace(teshis.Aciklama)
                    ? null
                    : teshis.Aciklama.Trim();
        }
    }
}