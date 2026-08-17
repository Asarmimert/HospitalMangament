using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class SekreterServisi : ISekreterServisi
    {
        private readonly IGenelDepo<Sekreter>
            _sekreterDeposu;

        public SekreterServisi(
            IGenelDepo<Sekreter> sekreterDeposu)
        {
            _sekreterDeposu = sekreterDeposu;
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
    }
}