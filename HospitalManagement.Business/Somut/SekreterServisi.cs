using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;

namespace HospitalManagement.Business.Somut
{
    public class SekreterServisi : ISekreterServisi
    {
        private readonly IGenelDepo<Sekreter>
            _sekreterDeposu;

        private readonly IGenelDepo<KullaniciHesabi>
            _kullaniciHesabiDeposu;

        public SekreterServisi(
            IGenelDepo<Sekreter> sekreterDeposu,
            IGenelDepo<KullaniciHesabi>
                kullaniciHesabiDeposu)
        {
            _sekreterDeposu = sekreterDeposu;
            _kullaniciHesabiDeposu =
                kullaniciHesabiDeposu;
        }

        public async Task<Sekreter?>
            KullaniciHesabiIdIleGetirAsync(
                int kullaniciHesabiId)
        {
            if (kullaniciHesabiId < 1)
            {
                return null;
            }

            var sekreterler =
                await _sekreterDeposu
                    .KosulaGoreGetirAsync(
                        x => x.KullaniciHesabiId ==
                             kullaniciHesabiId &&
                             x.AktifMi);

            return sekreterler.FirstOrDefault();
        }

        public async Task<Sekreter>
            HesabiylaBirlikteEkleAsync(
                Sekreter sekreter,
                KullaniciHesabi kullaniciHesabi)
        {
            if (string.IsNullOrWhiteSpace(
                    kullaniciHesabi.Eposta))
            {
                throw new ArgumentException(
                    "E-posta adresi boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(sekreter.Ad))
            {
                throw new ArgumentException(
                    "Sekreter adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(sekreter.Soyad))
            {
                throw new ArgumentException(
                    "Sekreter soyadı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    sekreter.TelefonNumarasi))
            {
                throw new ArgumentException(
                    "Telefon numarası boş olamaz.");
            }

            var temizEposta =
                kullaniciHesabi.Eposta
                    .Trim()
                    .ToLowerInvariant();

            var epostaKullaniliyorMu =
                await _kullaniciHesabiDeposu.VarMiAsync(
                    x => x.Eposta.ToLower() ==
                         temizEposta);

            if (epostaKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu e-posta adresi zaten kullanılıyor.");
            }

            var simdi = DateTime.UtcNow;

            kullaniciHesabi.Eposta = temizEposta;
            kullaniciHesabi.Rol = KullaniciRolu.Sekreter;
            kullaniciHesabi.AktifMi = true;
            kullaniciHesabi.OlusturulmaTarihi = simdi;
            kullaniciHesabi.GuncellenmeTarihi = null;

            sekreter.Ad = sekreter.Ad.Trim();
            sekreter.Soyad = sekreter.Soyad.Trim();

            sekreter.TelefonNumarasi =
                sekreter.TelefonNumarasi.Trim();

            sekreter.AktifMi = true;
            sekreter.OlusturulmaTarihi = simdi;
            sekreter.GuncellenmeTarihi = null;

            sekreter.KullaniciHesabi =
                kullaniciHesabi;

            await _kullaniciHesabiDeposu
                .EkleAsync(kullaniciHesabi);

            await _sekreterDeposu
                .EkleAsync(sekreter);

            await _sekreterDeposu.KaydetAsync();

            return sekreter;
        }
    }
}