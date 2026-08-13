using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class IlacServisi : IIlacServisi
    {
        private readonly IGenelDepo<Ilac> _ilacDeposu;

        public IlacServisi(
            IGenelDepo<Ilac> ilacDeposu)
        {
            _ilacDeposu = ilacDeposu;
        }

        public async Task<List<Ilac>> TumunuGetirAsync()
        {
            return await _ilacDeposu
                .KosulaGoreGetirAsync(x => x.AktifMi);
        }

        public async Task<Ilac?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            var ilac =
                await _ilacDeposu.IdIleGetirAsync(id);

            if (ilac is null || !ilac.AktifMi)
            {
                return null;
            }

            return ilac;
        }

        public async Task<Ilac> EkleAsync(Ilac ilac)
        {
            if (string.IsNullOrWhiteSpace(ilac.Ad))
            {
                throw new ArgumentException(
                    "İlaç adı boş olamaz.");
            }

            var temizAd = ilac.Ad.Trim();

            var ayniAdVarMi =
                await _ilacDeposu.VarMiAsync(
                    x => x.Ad.ToLower() ==
                         temizAd.ToLower());

            if (ayniAdVarMi)
            {
                throw new InvalidOperationException(
                    "Aynı isimde bir ilaç zaten var.");
            }

            ilac.Ad = temizAd;
            ilac.AktifMi = true;
            ilac.OlusturulmaTarihi = DateTime.UtcNow;
            ilac.GuncellenmeTarihi = null;

            await _ilacDeposu.EkleAsync(ilac);
            await _ilacDeposu.KaydetAsync();

            return ilac;
        }

        public async Task<bool> GuncelleAsync(Ilac ilac)
        {
            var mevcutIlac =
                await _ilacDeposu.IdIleGetirAsync(ilac.Id);

            if (mevcutIlac is null || !mevcutIlac.AktifMi)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(ilac.Ad))
            {
                throw new ArgumentException(
                    "İlaç adı boş olamaz.");
            }

            var temizAd = ilac.Ad.Trim();

            var ayniAdVarMi =
                await _ilacDeposu.VarMiAsync(
                    x => x.Ad.ToLower() ==
                         temizAd.ToLower() &&
                         x.Id != ilac.Id);

            if (ayniAdVarMi)
            {
                throw new InvalidOperationException(
                    "Aynı isimde bir ilaç zaten var.");
            }

            mevcutIlac.Ad = temizAd;
            mevcutIlac.GuncellenmeTarihi =
                DateTime.UtcNow;

            _ilacDeposu.Guncelle(mevcutIlac);
            await _ilacDeposu.KaydetAsync();

            return true;
        }

        public async Task<bool> PasiflestirAsync(int id)
        {
            var ilac =
                await _ilacDeposu.IdIleGetirAsync(id);

            if (ilac is null || !ilac.AktifMi)
            {
                return false;
            }

            ilac.AktifMi = false;
            ilac.GuncellenmeTarihi = DateTime.UtcNow;

            _ilacDeposu.Guncelle(ilac);
            await _ilacDeposu.KaydetAsync();

            return true;
        }
    }
}