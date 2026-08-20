using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class DoktorServisi : IDoktorServisi
    {
        private readonly IDoktorDeposu _doktorDeposu;

        private readonly IGenelDepo<Doctor>
            _genelDoktorDeposu;

        private readonly IGenelDepo<Department>
            _departmanDeposu;

        private readonly IGenelDepo<KullaniciHesabi>
            _kullaniciHesabiDeposu;

        public DoktorServisi(
            IDoktorDeposu doktorDeposu,
            IGenelDepo<Doctor> genelDoktorDeposu,
            IGenelDepo<Department> departmanDeposu,
            IGenelDepo<KullaniciHesabi>
                kullaniciHesabiDeposu)
        {
            _doktorDeposu = doktorDeposu;
            _genelDoktorDeposu = genelDoktorDeposu;
            _departmanDeposu = departmanDeposu;
            _kullaniciHesabiDeposu =
                kullaniciHesabiDeposu;
        }

        public async Task<
      (List<Doctor> Doktorlar, int ToplamKayitSayisi)>
      ListeleAsync(
          int sayfaNo,
          int sayfaBoyutu,
          int? departmentId,
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

            return await _doktorDeposu.ListeleAsync(
                sayfaNo,
                sayfaBoyutu,
                departmentId,
                aktifMi,
                arama,
                cancellationToken);
        }

        public async Task<Doctor?> IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _doktorDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }
        public async Task<Doctor?>
    KullaniciHesabiIdIleGetirAsync(
        int kullaniciHesabiId)
        {
            if (kullaniciHesabiId < 1)
            {
                return null;
            }

            var doktorlar =
                await _genelDoktorDeposu
                    .KosulaGoreGetirAsync(
                        x => x.KullaniciHesabiId ==
                             kullaniciHesabiId &&
                             x.AktifMi);

            return doktorlar.FirstOrDefault();
        }
        public async Task<Doctor> EkleAsync(Doctor doktor)
        {
            DoktorBilgileriniDogrula(doktor);

            var departman =
                await _departmanDeposu.IdIleGetirAsync(
                    doktor.DepartmentId);

            if (departman is null || !departman.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir departman bulunamadı.");
            }

            var kullaniciHesabi =
                await _kullaniciHesabiDeposu
                    .IdIleGetirAsync(
                        doktor.KullaniciHesabiId);

            if (kullaniciHesabi is null ||
                !kullaniciHesabi.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir kullanıcı hesabı bulunamadı.");
            }

            if (kullaniciHesabi.Rol != KullaniciRolu.Doktor)
            {
                throw new InvalidOperationException(
                    "Seçilen kullanıcı hesabının rolü Doktor değildir.");
            }

            var hesapKullaniliyorMu =
                await _genelDoktorDeposu.VarMiAsync(
                    x => x.KullaniciHesabiId ==
                         doktor.KullaniciHesabiId);

            if (hesapKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu kullanıcı hesabına bağlı bir doktor zaten var.");
            }

            BilgileriTemizle(doktor);

            doktor.AktifMi = true;
            doktor.OlusturulmaTarihi = DateTime.UtcNow;
            doktor.GuncellenmeTarihi = null;

            await _genelDoktorDeposu.EkleAsync(doktor);
            await _genelDoktorDeposu.KaydetAsync();

            return doktor;
        }
        public async Task<Doctor> HesabiylaBirlikteEkleAsync(
    Doctor doktor,
    KullaniciHesabi kullaniciHesabi)
        {
            if (doktor.DepartmentId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir departman seçilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(doktor.DoktorAd))
            {
                throw new ArgumentException(
                    "Doktor adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(doktor.DoktorSoyad))
            {
                throw new ArgumentException(
                    "Doktor soyadı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(kullaniciHesabi.Eposta))
            {
                throw new ArgumentException(
                    "E-posta boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    kullaniciHesabi.ParolaHash))
            {
                throw new ArgumentException(
                    "Parola bilgisi boş olamaz.");
            }

            var departman =
                await _departmanDeposu.IdIleGetirAsync(
                    doktor.DepartmentId);

            if (departman is null || !departman.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir departman bulunamadı.");
            }

            var temizEposta =
                kullaniciHesabi.Eposta
                    .Trim()
                    .ToLowerInvariant();

            var epostaKullaniliyorMu =
                await _kullaniciHesabiDeposu.VarMiAsync(
                    x => x.Eposta.ToLower() == temizEposta);

            if (epostaKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu e-posta adresi zaten kullanılıyor.");
            }

            BilgileriTemizle(doktor);

            kullaniciHesabi.Eposta = temizEposta;
            kullaniciHesabi.Rol = KullaniciRolu.Doktor;
            kullaniciHesabi.AktifMi = true;

            kullaniciHesabi.OlusturulmaTarihi =
                DateTime.UtcNow;

            kullaniciHesabi.GuncellenmeTarihi = null;

            doktor.KullaniciHesabi = kullaniciHesabi;
            doktor.AktifMi = true;
            doktor.OlusturulmaTarihi = DateTime.UtcNow;
            doktor.GuncellenmeTarihi = null;

            await _kullaniciHesabiDeposu
                .EkleAsync(kullaniciHesabi);

            await _genelDoktorDeposu.EkleAsync(doktor);
            await _genelDoktorDeposu.KaydetAsync();

            return doktor;
        }

        public async Task<bool> GuncelleAsync(Doctor doktor)
        {
            var mevcutDoktor =
    await _genelDoktorDeposu.IdIleGetirAsync(
        doktor.Id);

            if (mevcutDoktor is null ||
                !mevcutDoktor.AktifMi)
            {
                return false;
            }

            // Güncelleme sırasında kullanıcı hesabı değiştirilemez.
            // Mevcut doktorun kullanıcı hesabı bağlantısını koruyoruz.
            doktor.KullaniciHesabiId =
                mevcutDoktor.KullaniciHesabiId;

            DoktorBilgileriniDogrula(doktor);


            var departman =
                await _departmanDeposu.IdIleGetirAsync(
                    doktor.DepartmentId);

            if (departman is null || !departman.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir departman bulunamadı.");
            }

            BilgileriTemizle(doktor);

            mevcutDoktor.DepartmentId = doktor.DepartmentId;
            mevcutDoktor.DoktorAd = doktor.DoktorAd;
            mevcutDoktor.DoktorSoyad = doktor.DoktorSoyad;

            mevcutDoktor.TelefonNumarasi =
                doktor.TelefonNumarasi;

            mevcutDoktor.UzmanlikAlani =
                doktor.UzmanlikAlani;

            mevcutDoktor.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelDoktorDeposu.Guncelle(mevcutDoktor);
            await _genelDoktorDeposu.KaydetAsync();

            return true;
        }

        public async Task<bool> PasiflestirAsync(int id)
        {
            var doktor =
                await _genelDoktorDeposu.IdIleGetirAsync(id);

            if (doktor is null || !doktor.AktifMi)
            {
                return false;
            }

            doktor.AktifMi = false;
            doktor.GuncellenmeTarihi = DateTime.UtcNow;

            _genelDoktorDeposu.Guncelle(doktor);
            await _genelDoktorDeposu.KaydetAsync();

            return true;
        }

        private static void DoktorBilgileriniDogrula(
            Doctor doktor)
        {
            if (doktor.DepartmentId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir departman seçilmelidir.");
            }

            if (doktor.KullaniciHesabiId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir kullanıcı hesabı seçilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(doktor.DoktorAd))
            {
                throw new ArgumentException(
                    "Doktor adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    doktor.DoktorSoyad))
            {
                throw new ArgumentException(
                    "Doktor soyadı boş olamaz.");
            }
        }

        private static void BilgileriTemizle(Doctor doktor)
        {

            doktor.DoktorAd = doktor.DoktorAd.Trim();
            doktor.DoktorSoyad = doktor.DoktorSoyad.Trim();

            doktor.TelefonNumarasi =
                string.IsNullOrWhiteSpace(
                    doktor.TelefonNumarasi)
                    ? null
                    : doktor.TelefonNumarasi.Trim();

            doktor.UzmanlikAlani =
                string.IsNullOrWhiteSpace(
                    doktor.UzmanlikAlani)
                    ? null
                    : doktor.UzmanlikAlani.Trim();
        }
    }
}