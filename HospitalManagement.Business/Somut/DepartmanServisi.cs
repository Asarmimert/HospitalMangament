using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace HospitalManagement.Business.Somut
{
    public class DepartmanServisi : IDepartmanServisi
    {
        private const string AktifDepartmanlarCacheKey =
            "aktif-departmanlar";

        private readonly IGenelDepo<Department> _departmanDeposu;
        private readonly IMemoryCache _cache;

        public DepartmanServisi(
            IGenelDepo<Department> departmanDeposu,
            IMemoryCache cache)
        {
            _departmanDeposu = departmanDeposu;
            _cache = cache;
        }

        public async Task<List<Department>> TumunuGetirAsync()
        {
            var departmanlar =
                await _cache.GetOrCreateAsync(
                    AktifDepartmanlarCacheKey,
                    async cacheAyari =>
                    {
                        cacheAyari
                            .AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromMinutes(10);

                        return await _departmanDeposu
                            .KosulaGoreGetirAsync(
                                x => x.AktifMi);
                    });

            return departmanlar ?? new List<Department>();
        }

        public async Task<Department?> IdIleGetirAsync(int id)
        {
            var departman =
                await _departmanDeposu.IdIleGetirAsync(id);

            if (departman is null || !departman.AktifMi)
            {
                return null;
            }

            return departman;
        }

        public async Task<Department> EkleAsync(
            Department departman)
        {
            if (string.IsNullOrWhiteSpace(departman.Name))
            {
                throw new ArgumentException(
                    "Departman adı boş olamaz.");
            }

            var temizAd = departman.Name.Trim();

            var ayniIsimVarMi =
                await _departmanDeposu.VarMiAsync(
                    x => x.Name.ToLower() ==
                         temizAd.ToLower());

            if (ayniIsimVarMi)
            {
                throw new InvalidOperationException(
                    "Aynı isimde bir departman zaten var.");
            }

            departman.Name = temizAd;

            departman.Description =
                string.IsNullOrWhiteSpace(
                    departman.Description)
                    ? null
                    : departman.Description.Trim();

            departman.AktifMi = true;
            departman.OlusturulmaTarihi = DateTime.UtcNow;

            await _departmanDeposu.EkleAsync(departman);
            await _departmanDeposu.KaydetAsync();

            CacheTemizle();

            return departman;
        }

        public async Task<bool> GuncelleAsync(
            Department departman)
        {
            var mevcutDepartman =
                await _departmanDeposu.IdIleGetirAsync(
                    departman.DepartmentId);

            if (mevcutDepartman is null ||
                !mevcutDepartman.AktifMi)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(departman.Name))
            {
                throw new ArgumentException(
                    "Departman adı boş olamaz.");
            }

            var temizAd = departman.Name.Trim();

            var ayniIsimVarMi =
                await _departmanDeposu.VarMiAsync(
                    x => x.Name.ToLower() ==
                         temizAd.ToLower()
                         && x.DepartmentId !=
                         departman.DepartmentId);

            if (ayniIsimVarMi)
            {
                throw new InvalidOperationException(
                    "Aynı isimde bir departman zaten var.");
            }

            mevcutDepartman.Name = temizAd;

            mevcutDepartman.Description =
                string.IsNullOrWhiteSpace(
                    departman.Description)
                    ? null
                    : departman.Description.Trim();

            _departmanDeposu.Guncelle(mevcutDepartman);
            await _departmanDeposu.KaydetAsync();

            CacheTemizle();

            return true;
        }

        public async Task<bool> SilAsync(int id)
        {
            var departman =
                await _departmanDeposu.IdIleGetirAsync(id);

            if (departman is null || !departman.AktifMi)
            {
                return false;
            }

            departman.AktifMi = false;

            _departmanDeposu.Guncelle(departman);
            await _departmanDeposu.KaydetAsync();

            CacheTemizle();

            return true;
        }

        private void CacheTemizle()
        {
            _cache.Remove(AktifDepartmanlarCacheKey);
        }
    }
}