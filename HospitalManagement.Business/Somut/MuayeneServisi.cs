using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class MuayeneServisi : IMuayeneServisi
    {
        private readonly IMuayeneDeposu _muayeneDeposu;

        private readonly IGenelDepo<Muayene>
            _genelMuayeneDeposu;

        private readonly IGenelDepo<Randevu>
            _randevuDeposu;

        public MuayeneServisi(
            IMuayeneDeposu muayeneDeposu,
            IGenelDepo<Muayene> genelMuayeneDeposu,
            IGenelDepo<Randevu> randevuDeposu)
        {
            _muayeneDeposu = muayeneDeposu;
            _genelMuayeneDeposu = genelMuayeneDeposu;
            _randevuDeposu = randevuDeposu;
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

            if (doktorId.HasValue && doktorId.Value < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir doktor seçilmelidir.");
            }

            if (hastaId.HasValue && hastaId.Value < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir hasta seçilmelidir.");
            }

            if (baslangicTarihi.HasValue &&
                bitisTarihi.HasValue &&
                bitisTarihi.Value < baslangicTarihi.Value)
            {
                throw new ArgumentException(
                    "Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }

            return await _muayeneDeposu.ListeleAsync(
                sayfaNo,
                sayfaBoyutu,
                doktorId,
                hastaId,
                baslangicTarihi,
                bitisTarihi);
        }

        public async Task<Muayene?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _muayeneDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }

        public async Task<Muayene> EkleAsync(
            Muayene muayene)
        {
            MuayeneBilgileriniDogrula(muayene);

            var randevu =
                await _randevuDeposu.IdIleGetirAsync(
                    muayene.RandevuId);

            if (randevu is null)
            {
                throw new InvalidOperationException(
                    "Randevu bulunamadı.");
            }

            if (randevu.Durum != RandevuDurumu.Tamamlandi)
            {
                throw new InvalidOperationException(
                    "Yalnızca tamamlanmış bir randevu için " +
                    "muayene kaydı oluşturulabilir.");
            }

            var muayeneVarMi =
                await _muayeneDeposu
                    .RandevuIcinMuayeneVarMiAsync(
                        muayene.RandevuId);

            if (muayeneVarMi)
            {
                throw new InvalidOperationException(
                    "Bu randevu için daha önce muayene " +
                    "kaydı oluşturulmuş.");
            }

            if (muayene.MuayeneTarihi <
                randevu.BaslangicZamani)
            {
                throw new ArgumentException(
                    "Muayene tarihi randevu başlangıcından " +
                    "önce olamaz.");
            }

            BilgileriTemizle(muayene);

            muayene.OlusturulmaTarihi = DateTime.UtcNow;
            muayene.GuncellenmeTarihi = null;

            await _genelMuayeneDeposu.EkleAsync(muayene);
            await _genelMuayeneDeposu.KaydetAsync();

            return muayene;
        }

        public async Task<bool> GuncelleAsync(
            Muayene muayene)
        {
            var mevcutMuayene =
                await _genelMuayeneDeposu.IdIleGetirAsync(
                    muayene.Id);

            if (mevcutMuayene is null)
            {
                return false;
            }

            // Muayenenin bağlı olduğu randevu değiştirilemez.
            muayene.RandevuId =
                mevcutMuayene.RandevuId;

            MuayeneBilgileriniDogrula(muayene);

            var randevu =
                await _randevuDeposu.IdIleGetirAsync(
                    mevcutMuayene.RandevuId);

            if (randevu is null)
            {
                throw new InvalidOperationException(
                    "Muayenenin bağlı olduğu randevu bulunamadı.");
            }

            if (muayene.MuayeneTarihi <
                randevu.BaslangicZamani)
            {
                throw new ArgumentException(
                    "Muayene tarihi randevu başlangıcından " +
                    "önce olamaz.");
            }

            BilgileriTemizle(muayene);

            mevcutMuayene.HastaSikayeti =
                muayene.HastaSikayeti;

            mevcutMuayene.DoktorDegerlendirmesi =
                muayene.DoktorDegerlendirmesi;

            mevcutMuayene.DoktorNotlari =
                muayene.DoktorNotlari;

            mevcutMuayene.MuayeneTarihi =
                muayene.MuayeneTarihi;

            mevcutMuayene.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelMuayeneDeposu.Guncelle(mevcutMuayene);
            await _genelMuayeneDeposu.KaydetAsync();

            return true;
        }

        private static void MuayeneBilgileriniDogrula(
            Muayene muayene)
        {
            if (muayene.RandevuId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir randevu seçilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(
                    muayene.HastaSikayeti))
            {
                throw new ArgumentException(
                    "Hasta şikâyeti boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    muayene.DoktorDegerlendirmesi))
            {
                throw new ArgumentException(
                    "Doktor değerlendirmesi boş olamaz.");
            }

            if (muayene.MuayeneTarihi == default)
            {
                throw new ArgumentException(
                    "Geçerli bir muayene tarihi girilmelidir.");
            }
        }

        private static void BilgileriTemizle(
            Muayene muayene)
        {
            muayene.HastaSikayeti =
                muayene.HastaSikayeti.Trim();

            muayene.DoktorDegerlendirmesi =
                muayene.DoktorDegerlendirmesi.Trim();

            muayene.DoktorNotlari =
                string.IsNullOrWhiteSpace(
                    muayene.DoktorNotlari)
                    ? null
                    : muayene.DoktorNotlari.Trim();
        }
    }
}