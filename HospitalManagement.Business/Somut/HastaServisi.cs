using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class HastaServisi : IHastaServisi
    {
        private readonly IHastaDeposu _hastaDeposu;

        private readonly IGenelDepo<Hasta>
            _genelHastaDeposu;

        private readonly IGenelDepo<KullaniciHesabi>
            _kullaniciHesabiDeposu;

        public HastaServisi(
            IHastaDeposu hastaDeposu,
            IGenelDepo<Hasta> genelHastaDeposu,
            IGenelDepo<KullaniciHesabi>
                kullaniciHesabiDeposu)
        {
            _hastaDeposu = hastaDeposu;
            _genelHastaDeposu = genelHastaDeposu;
            _kullaniciHesabiDeposu =
                kullaniciHesabiDeposu;
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

            return await _hastaDeposu.ListeleAsync(
                sayfaNo,
                sayfaBoyutu,
                aktifMi,
                arama,
                cancellationToken);
        }

        public async Task<Hasta?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _hastaDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }
        public async Task<Hasta?>
            KullaniciHesabiIdIleGetirAsync(
                int kullaniciHesabiId)
        {
            if (kullaniciHesabiId < 1)
            {
                return null;
            }

            return await _hastaDeposu
                .KullaniciHesabiIdIleGetirAsync(
                    kullaniciHesabiId);
        }

        public async Task<Hasta> EkleAsync(Hasta hasta)
        {
            HastaBilgileriniDogrula(hasta);

            var kullaniciHesabi =
                await _kullaniciHesabiDeposu
                    .IdIleGetirAsync(
                        hasta.KullaniciHesabiId);

            if (kullaniciHesabi is null ||
                !kullaniciHesabi.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir kullanıcı hesabı bulunamadı.");
            }

            if (kullaniciHesabi.Rol != KullaniciRolu.Hasta)
            {
                throw new InvalidOperationException(
                    "Seçilen kullanıcı hesabının rolü Hasta değildir.");
            }

            var hesapKullaniliyorMu =
                await _genelHastaDeposu.VarMiAsync(
                    x => x.KullaniciHesabiId ==
                         hasta.KullaniciHesabiId);

            if (hesapKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu kullanıcı hesabına bağlı bir hasta zaten var.");
            }

            var kimlikNumarasiKullaniliyorMu =
                await _genelHastaDeposu.VarMiAsync(
                    x => x.KimlikNumarasi ==
                         hasta.KimlikNumarasi);

            if (kimlikNumarasiKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu kimlik numarası zaten kayıtlı.");
            }

            BilgileriTemizle(hasta);

            hasta.AktifMi = true;
            hasta.OlusturulmaTarihi = DateTime.UtcNow;
            hasta.GuncellenmeTarihi = null;

            await _genelHastaDeposu.EkleAsync(hasta);
            await _genelHastaDeposu.KaydetAsync();

            return hasta;
        }

        public async Task<bool> GuncelleAsync(Hasta hasta)
        {
            var mevcutHasta =
                await _genelHastaDeposu.IdIleGetirAsync(
                    hasta.Id);

            if (mevcutHasta is null ||
                !mevcutHasta.AktifMi)
            {
                return false;
            }

            // Bu alanlar normal güncellemede değiştirilemez.
            hasta.KullaniciHesabiId =
                mevcutHasta.KullaniciHesabiId;

            hasta.KimlikNumarasi =
                mevcutHasta.KimlikNumarasi;

            HastaBilgileriniDogrula(hasta);
            BilgileriTemizle(hasta);

            mevcutHasta.Ad = hasta.Ad;
            mevcutHasta.Soyad = hasta.Soyad;
            mevcutHasta.DogumTarihi = hasta.DogumTarihi;
            mevcutHasta.TelefonNumarasi =
                hasta.TelefonNumarasi;
            mevcutHasta.Adres = hasta.Adres;
            mevcutHasta.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelHastaDeposu.Guncelle(mevcutHasta);
            await _genelHastaDeposu.KaydetAsync();

            return true;
        }

        public async Task<bool> PasiflestirAsync(int id)
        {
            var hasta =
                await _genelHastaDeposu.IdIleGetirAsync(id);

            if (hasta is null || !hasta.AktifMi)
            {
                return false;
            }

            hasta.AktifMi = false;
            hasta.GuncellenmeTarihi = DateTime.UtcNow;

            _genelHastaDeposu.Guncelle(hasta);
            await _genelHastaDeposu.KaydetAsync();

            return true;
        }

        private static void HastaBilgileriniDogrula(
            Hasta hasta)
        {
            if (hasta.KullaniciHesabiId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir kullanıcı hesabı seçilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(hasta.Ad))
            {
                throw new ArgumentException(
                    "Hasta adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(hasta.Soyad))
            {
                throw new ArgumentException(
                    "Hasta soyadı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    hasta.KimlikNumarasi) ||
                hasta.KimlikNumarasi.Length != 11 ||
                !hasta.KimlikNumarasi.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "Kimlik numarası 11 rakamdan oluşmalıdır.");
            }

            if (hasta.DogumTarihi == default ||
                hasta.DogumTarihi >
                DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException(
                    "Geçerli bir doğum tarihi girilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(
                    hasta.TelefonNumarasi) ||
                hasta.TelefonNumarasi.Length != 11 ||
                !hasta.TelefonNumarasi.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "Telefon numarası 11 rakamdan oluşmalıdır.");
            }
        }

        private static void BilgileriTemizle(Hasta hasta)
        {
            hasta.Ad = hasta.Ad.Trim();
            hasta.Soyad = hasta.Soyad.Trim();
            hasta.KimlikNumarasi =
                hasta.KimlikNumarasi.Trim();
            hasta.TelefonNumarasi =
                hasta.TelefonNumarasi.Trim();

            hasta.Adres =
                string.IsNullOrWhiteSpace(hasta.Adres)
                    ? null
                    : hasta.Adres.Trim();
        }
    }
}