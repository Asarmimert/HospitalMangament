using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class KullaniciHesabiServisi
        : IKullaniciHesabiServisi
    {
        private readonly IGenelDepo<KullaniciHesabi>
            _kullaniciHesabiDeposu;

        public KullaniciHesabiServisi(
            IGenelDepo<KullaniciHesabi>
                kullaniciHesabiDeposu)
        {
            _kullaniciHesabiDeposu =
                kullaniciHesabiDeposu;
        }

        public async Task<KullaniciHesabi?>
            EpostaIleGetirAsync(string eposta)
        {
            if (string.IsNullOrWhiteSpace(eposta))
            {
                return null;
            }

            var temizEposta =
                eposta.Trim().ToLower();

            var kullanicilar =
                await _kullaniciHesabiDeposu
                    .KosulaGoreGetirAsync(
                        x => x.Eposta.ToLower() ==
                             temizEposta);

            return kullanicilar.FirstOrDefault();
        }

        public async Task<KullaniciHesabi> EkleAsync(
            KullaniciHesabi kullaniciHesabi)
        {
            if (string.IsNullOrWhiteSpace(
                    kullaniciHesabi.Eposta))
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

            var temizEposta =
                kullaniciHesabi.Eposta.Trim().ToLower();

            var epostaKullaniliyorMu =
                await _kullaniciHesabiDeposu
                    .VarMiAsync(
                        x => x.Eposta.ToLower() ==
                             temizEposta);

            if (epostaKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu e-posta adresi zaten kullanılıyor.");
            }

            kullaniciHesabi.Eposta = temizEposta;
            kullaniciHesabi.AktifMi = true;
            kullaniciHesabi.OlusturulmaTarihi =
                DateTime.UtcNow;
            kullaniciHesabi.GuncellenmeTarihi = null;

            await _kullaniciHesabiDeposu
                .EkleAsync(kullaniciHesabi);

            await _kullaniciHesabiDeposu
                .KaydetAsync();

            return kullaniciHesabi;
        }
    }
}