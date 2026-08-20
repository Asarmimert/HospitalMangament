using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class MuayeneTeshisiServisi
        : IMuayeneTeshisiServisi
    {
        private readonly IMuayeneTeshisiDeposu
            _muayeneTeshisiDeposu;

        private readonly IGenelDepo<MuayeneTeshisi>
            _genelMuayeneTeshisiDeposu;

        private readonly IGenelDepo<Muayene>
            _muayeneDeposu;

        private readonly IGenelDepo<Teshis>
            _teshisDeposu;

        public MuayeneTeshisiServisi(
            IMuayeneTeshisiDeposu muayeneTeshisiDeposu,
            IGenelDepo<MuayeneTeshisi>
                genelMuayeneTeshisiDeposu,
            IGenelDepo<Muayene> muayeneDeposu,
            IGenelDepo<Teshis> teshisDeposu)
        {
            _muayeneTeshisiDeposu =
                muayeneTeshisiDeposu;

            _genelMuayeneTeshisiDeposu =
                genelMuayeneTeshisiDeposu;

            _muayeneDeposu = muayeneDeposu;
            _teshisDeposu = teshisDeposu;
        }

        public async Task<List<MuayeneTeshisi>>
            MuayeneyeGoreListeleAsync(int muayeneId)
        {
            if (muayeneId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir muayene seçilmelidir.");
            }

            return await _muayeneTeshisiDeposu
                .MuayeneyeGoreListeleAsync(muayeneId);
        }

        public async Task<MuayeneTeshisi?>
            IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _muayeneTeshisiDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }

        public async Task<MuayeneTeshisi> EkleAsync(
            MuayeneTeshisi muayeneTeshisi)
        {
            if (muayeneTeshisi.MuayeneId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir muayene seçilmelidir.");
            }

            if (muayeneTeshisi.TeshisId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir teşhis seçilmelidir.");
            }

            var muayene =
                await _muayeneDeposu.IdIleGetirAsync(
                    muayeneTeshisi.MuayeneId);

            if (muayene is null)
            {
                throw new InvalidOperationException(
                    "Muayene bulunamadı.");
            }

            var teshis =
                await _teshisDeposu.IdIleGetirAsync(
                    muayeneTeshisi.TeshisId);

            if (teshis is null || !teshis.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif teşhis bulunamadı.");
            }

            var ayniTeshisVarMi =
                await _muayeneTeshisiDeposu
                    .AyniTeshisVarMiAsync(
                        muayeneTeshisi.MuayeneId,
                        muayeneTeshisi.TeshisId);

            if (ayniTeshisVarMi)
            {
                throw new InvalidOperationException(
                    "Bu teşhis muayeneye daha önce eklenmiş.");
            }

            muayeneTeshisi.DoktorNotu =
                string.IsNullOrWhiteSpace(
                    muayeneTeshisi.DoktorNotu)
                    ? null
                    : muayeneTeshisi.DoktorNotu.Trim();

            muayeneTeshisi.OlusturulmaTarihi =
                DateTime.UtcNow;

            muayeneTeshisi.GuncellenmeTarihi = null;

            await _genelMuayeneTeshisiDeposu
                .EkleAsync(muayeneTeshisi);

            await _genelMuayeneTeshisiDeposu
                .KaydetAsync();

            return muayeneTeshisi;
        }
        public async Task<bool> SilAsync(int id)
        {
            if (id < 1)
            {
                return false;
            }

            var mevcutKayit =
                await _genelMuayeneTeshisiDeposu
                    .IdIleGetirAsync(id);

            if (mevcutKayit is null)
            {
                return false;
            }

            _genelMuayeneTeshisiDeposu
                .Sil(mevcutKayit);

            await _genelMuayeneTeshisiDeposu
                .KaydetAsync();

            return true;
        }
    }

}