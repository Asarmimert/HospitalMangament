using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class ReceteServisi : IReceteServisi
    {
        private readonly IReceteDeposu _receteDeposu;
        private readonly IMuayeneDeposu _muayeneDeposu;

        private readonly IGenelDepo<Recete>
            _genelReceteDeposu;

        public ReceteServisi(
            IReceteDeposu receteDeposu,
            IMuayeneDeposu muayeneDeposu,
            IGenelDepo<Recete> genelReceteDeposu)
        {
            _receteDeposu = receteDeposu;
            _muayeneDeposu = muayeneDeposu;
            _genelReceteDeposu = genelReceteDeposu;
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
            if (sayfaNo < 1)
            {
                throw new ArgumentException(
                    "Sayfa numarası en az 1 olmalıdır.");
            }

            if (sayfaBoyutu < 1 || sayfaBoyutu > 100)
            {
                throw new ArgumentException(
                    "Sayfa boyutu 1 ile 100 arasında olmalıdır.");
            }

            if (baslangicTarihi.HasValue &&
                bitisTarihi.HasValue &&
                bitisTarihi.Value < baslangicTarihi.Value)
            {
                throw new ArgumentException(
                    "Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }

            return await _receteDeposu.ListeleAsync(
                sayfaNo,
                sayfaBoyutu,
                hastaId,
                doktorId,
                baslangicTarihi,
                bitisTarihi);
        }

        public async Task<Recete?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _receteDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }

        public async Task<Recete> EkleAsync(Recete recete)
        {
            if (recete.MuayeneId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir muayene seçilmelidir.");
            }

            var muayene =
                await _muayeneDeposu
                    .IliskileriyleIdIleGetirAsync(
                        recete.MuayeneId);

            if (muayene is null)
            {
                throw new InvalidOperationException(
                    "Muayene bulunamadı.");
            }

            if (muayene.Randevu.Durum !=
                RandevuDurumu.Tamamlandi)
            {
                throw new InvalidOperationException(
                    "Yalnızca tamamlanmış randevunun " +
                    "muayenesine reçete yazılabilir.");
            }

            var receteVarMi =
                await _receteDeposu
                    .MuayeneninRecetesiVarMiAsync(
                        recete.MuayeneId);

            if (receteVarMi)
            {
                throw new InvalidOperationException(
                    "Bu muayeneye daha önce reçete yazılmış.");
            }

            recete.HastaId =
                muayene.Randevu.HastaId;

            recete.DoktorId =
                muayene.Randevu.DoktorId;

            recete.GenelNotlar =
                string.IsNullOrWhiteSpace(recete.GenelNotlar)
                    ? null
                    : recete.GenelNotlar.Trim();

            recete.ReceteTarihi = DateTime.UtcNow;
            recete.OlusturulmaTarihi = DateTime.UtcNow;
            recete.GuncellenmeTarihi = null;

            await _genelReceteDeposu.EkleAsync(recete);
            await _genelReceteDeposu.KaydetAsync();

            return recete;
        }

        public async Task<bool> GuncelleAsync(Recete recete)
        {
            var mevcutRecete =
                await _genelReceteDeposu.IdIleGetirAsync(
                    recete.Id);

            if (mevcutRecete is null)
            {
                return false;
            }

            mevcutRecete.GenelNotlar =
                string.IsNullOrWhiteSpace(recete.GenelNotlar)
                    ? null
                    : recete.GenelNotlar.Trim();

            mevcutRecete.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelReceteDeposu.Guncelle(mevcutRecete);
            await _genelReceteDeposu.KaydetAsync();

            return true;
        }
    }
}