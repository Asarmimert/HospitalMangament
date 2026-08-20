using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class RandevuServisi : IRandevuServisi
    {
        private readonly IRandevuDeposu _randevuDeposu;
        private readonly IGenelDepo<Randevu> _genelRandevuDeposu;
        private readonly IGenelDepo<Doctor> _doktorDeposu;
        private readonly IGenelDepo<Hasta> _hastaDeposu;
        private readonly IGenelDepo<Sekreter> _sekreterDeposu;

        public RandevuServisi(
            IRandevuDeposu randevuDeposu,
            IGenelDepo<Randevu> genelRandevuDeposu,
            IGenelDepo<Doctor> doktorDeposu,
            IGenelDepo<Hasta> hastaDeposu,
            IGenelDepo<Sekreter> sekreterDeposu)
        {
            _randevuDeposu = randevuDeposu;
            _genelRandevuDeposu = genelRandevuDeposu;
            _doktorDeposu = doktorDeposu;
            _hastaDeposu = hastaDeposu;
            _sekreterDeposu = sekreterDeposu;
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
                bitisTarihi.Value <= baslangicTarihi.Value)
            {
                throw new ArgumentException(
                    "Filtre bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
            }

            return await _randevuDeposu.ListeleAsync(
                sayfaNo,
                sayfaBoyutu,
                doktorId,
                hastaId,
                durum,
                baslangicTarihi,
                bitisTarihi,
                cancellationToken);
        }
        public async Task<Randevu?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _randevuDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }
        public async Task<Randevu> EkleAsync(Randevu randevu)
        {
            RandevuBilgileriniDogrula(randevu);

            await IliskiliKayitlariDogrulaAsync(
                randevu.DoktorId,
                randevu.HastaId,
                randevu.OlusturanSekreterId);

            var cakismaVarMi =
                await _randevuDeposu
                    .DoktorRandevusuCakisiyorMuAsync(
                        randevu.DoktorId,
                        randevu.BaslangicZamani,
                        randevu.BitisZamani);

            if (cakismaVarMi)
            {
                throw new InvalidOperationException(
                    "Doktorun seçilen zaman aralığında başka bir randevusu var.");
            }

            randevu.Durum = RandevuDurumu.Planlandi;
            randevu.IptalNedeni = null;
            randevu.OlusturulmaTarihi = DateTime.UtcNow;
            randevu.GuncellenmeTarihi = null;

            await _genelRandevuDeposu.EkleAsync(randevu);
            await _genelRandevuDeposu.KaydetAsync();

            return randevu;
        }

        public async Task<bool> GuncelleAsync(Randevu randevu)
        {
            var mevcutRandevu =
                await _genelRandevuDeposu.IdIleGetirAsync(
                    randevu.Id);

            if (mevcutRandevu is null)
            {
                return false;
            }

            if (mevcutRandevu.Durum !=
                RandevuDurumu.Planlandi)
            {
                throw new InvalidOperationException(
                    "Yalnızca planlanan randevular güncellenebilir.");
            }

            randevu.OlusturanSekreterId =
                mevcutRandevu.OlusturanSekreterId;

            RandevuBilgileriniDogrula(randevu);

            await IliskiliKayitlariDogrulaAsync(
                randevu.DoktorId,
                randevu.HastaId,
                randevu.OlusturanSekreterId);

            var cakismaVarMi =
                await _randevuDeposu
                    .DoktorRandevusuCakisiyorMuAsync(
                        randevu.DoktorId,
                        randevu.BaslangicZamani,
                        randevu.BitisZamani,
                        mevcutRandevu.Id);

            if (cakismaVarMi)
            {
                throw new InvalidOperationException(
                    "Doktorun seçilen zaman aralığında başka bir randevusu var.");
            }

            mevcutRandevu.DoktorId = randevu.DoktorId;
            mevcutRandevu.HastaId = randevu.HastaId;

            mevcutRandevu.BaslangicZamani =
                randevu.BaslangicZamani;

            mevcutRandevu.BitisZamani =
                randevu.BitisZamani;

            mevcutRandevu.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelRandevuDeposu.Guncelle(mevcutRandevu);
            await _genelRandevuDeposu.KaydetAsync();

            return true;
        }

        public async Task<bool> DurumGuncelleAsync(
            int id,
            RandevuDurumu yeniDurum,
            string? iptalNedeni)
        {
            var randevu =
                await _genelRandevuDeposu.IdIleGetirAsync(id);

            if (randevu is null)
            {
                return false;
            }

            if (randevu.Durum != RandevuDurumu.Planlandi)
            {
                throw new InvalidOperationException(
                    "Sonuçlandırılmış bir randevunun durumu değiştirilemez.");
            }

            if (yeniDurum == RandevuDurumu.Planlandi)
            {
                throw new ArgumentException(
                    "Randevu zaten planlandı durumundadır.");
            }

            if (!Enum.IsDefined(typeof(RandevuDurumu), yeniDurum))
            {
                throw new ArgumentException(
                    "Geçersiz randevu durumu.");
            }

            if (yeniDurum == RandevuDurumu.IptalEdildi)
            {
                if (string.IsNullOrWhiteSpace(iptalNedeni))
                {
                    throw new ArgumentException(
                        "İptal edilen randevu için iptal nedeni zorunludur.");
                }

                randevu.IptalNedeni = iptalNedeni.Trim();
            }
            else
            {
                randevu.IptalNedeni = null;
            }

            randevu.Durum = yeniDurum;
            randevu.GuncellenmeTarihi = DateTime.UtcNow;

            _genelRandevuDeposu.Guncelle(randevu);
            await _genelRandevuDeposu.KaydetAsync();

            return true;
        }

        private static void RandevuBilgileriniDogrula(
            Randevu randevu)
        {
            if (randevu.DoktorId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir doktor seçilmelidir.");
            }

            if (randevu.HastaId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir hasta seçilmelidir.");
            }

            //if (randevu.OlusturanSekreterId < 1)
            //{
            //    throw new ArgumentException(
            //        "Geçerli bir sekreter seçilmelidir.");
            //}

            if (randevu.BaslangicZamani == default ||
       randevu.BitisZamani == default)
            {
                throw new ArgumentException(
                    "Randevu başlangıç ve bitiş zamanı zorunludur.");
            }

            if (randevu.BitisZamani <=
                randevu.BaslangicZamani)
            {
                throw new ArgumentException(
                    "Randevu bitiş zamanı başlangıç zamanından sonra olmalıdır.");
            }

            // Gelen UTC saatlerini İstanbul saatine çeviriyoruz.
            var yerelBaslangic =
                IstanbulSaatineCevir(
                    randevu.BaslangicZamani);

            var yerelBitis =
                IstanbulSaatineCevir(
                    randevu.BitisZamani);

            var yerelSimdi =
                IstanbulSaatineCevir(DateTime.UtcNow);

            if (yerelBaslangic <= yerelSimdi)
            {
                throw new ArgumentException(
                    "Geçmiş tarih veya saate randevu oluşturulamaz.");
            }

            // Randevu aynı gün içinde olmalı.
            // Başlangıç 17:00 olamaz, bitiş ise en fazla 17:00 olabilir.
            if (yerelBaslangic.Date != yerelBitis.Date ||
     yerelBaslangic.TimeOfDay <
         TimeSpan.FromHours(8) ||
     yerelBaslangic.TimeOfDay >=
         TimeSpan.FromHours(17) ||
     yerelBitis.TimeOfDay >
         TimeSpan.FromHours(17))
            {
                throw new ArgumentException(
                       "Randevu saatleri 08:00 ile 17:00 arasında olmalıdır.");
            }
        }

        private static DateTime IstanbulSaatineCevir(
            DateTime zaman)
        {
            // Zaman zaten yerel biçimde geldiyse değiştirme.
            if (zaman.Kind == DateTimeKind.Unspecified)
            {
                return zaman;
            }

            var istanbulSaatDilimi =
                TimeZoneInfo.FindSystemTimeZoneById(
                    "Europe/Istanbul");

            return TimeZoneInfo.ConvertTimeFromUtc(
                zaman.ToUniversalTime(),
                istanbulSaatDilimi);
        }

        private async Task IliskiliKayitlariDogrulaAsync(
            int doktorId,
            int hastaId,
            int? sekreterId)
        {
            var doktor =
                await _doktorDeposu.IdIleGetirAsync(doktorId);

            if (doktor is null || !doktor.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif doktor bulunamadı.");
            }

            var hasta =
                await _hastaDeposu.IdIleGetirAsync(hastaId);

            if (hasta is null || !hasta.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif hasta bulunamadı.");
            }

            if (sekreterId.HasValue)
            {
                var sekreter =
                    await _sekreterDeposu.IdIleGetirAsync(
                        sekreterId.Value);

                if (sekreter is null || !sekreter.AktifMi)
                {
                    throw new InvalidOperationException(
                        "Aktif sekreter bulunamadı.");
                }
            }
        }
    }
}