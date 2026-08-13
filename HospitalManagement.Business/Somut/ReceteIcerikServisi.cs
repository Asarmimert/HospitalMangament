using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Somut
{
    public class ReceteIcerikServisi
        : IReceteIcerikServisi
    {
        private readonly IReceteIcerikDeposu
            _receteIcerikDeposu;

        private readonly IGenelDepo<ReceteIcerik>
            _genelReceteIcerikDeposu;

        private readonly IGenelDepo<Recete>
            _receteDeposu;

        private readonly IGenelDepo<Ilac>
            _ilacDeposu;

        public ReceteIcerikServisi(
            IReceteIcerikDeposu receteIcerikDeposu,
            IGenelDepo<ReceteIcerik>
                genelReceteIcerikDeposu,
            IGenelDepo<Recete> receteDeposu,
            IGenelDepo<Ilac> ilacDeposu)
        {
            _receteIcerikDeposu = receteIcerikDeposu;
            _genelReceteIcerikDeposu =
                genelReceteIcerikDeposu;
            _receteDeposu = receteDeposu;
            _ilacDeposu = ilacDeposu;
        }

        public async Task<List<ReceteIcerik>>
            ReceteyeGoreListeleAsync(int receteId)
        {
            if (receteId < 1)
            {
                throw new ArgumentException(
                    "Geçerli bir reçete seçilmelidir.");
            }

            return await _receteIcerikDeposu
                .ReceteyeGoreListeleAsync(receteId);
        }

        public async Task<ReceteIcerik?>
            IdIleGetirAsync(int id)
        {
            if (id < 1)
            {
                return null;
            }

            return await _receteIcerikDeposu
                .IliskileriyleIdIleGetirAsync(id);
        }

        public async Task<ReceteIcerik> EkleAsync(
            ReceteIcerik receteIcerik)
        {
            IcerikBilgileriniDogrula(receteIcerik);

            var recete = await _receteDeposu
                .IdIleGetirAsync(receteIcerik.ReceteId);

            if (recete is null)
            {
                throw new InvalidOperationException(
                    "Reçete bulunamadı.");
            }

            var ilac = await _ilacDeposu
                .IdIleGetirAsync(receteIcerik.IlacId);

            if (ilac is null || !ilac.AktifMi)
            {
                throw new InvalidOperationException(
                    "Aktif bir ilaç bulunamadı.");
            }

            var ayniIlacVarMi =
                await _receteIcerikDeposu
                    .AyniIlacVarMiAsync(
                        receteIcerik.ReceteId,
                        receteIcerik.IlacId);

            if (ayniIlacVarMi)
            {
                throw new InvalidOperationException(
                    "Bu ilaç reçeteye daha önce eklenmiş.");
            }

            BilgileriTemizle(receteIcerik);

            receteIcerik.OlusturulmaTarihi =
                DateTime.UtcNow;

            receteIcerik.GuncellenmeTarihi = null;

            await _genelReceteIcerikDeposu
                .EkleAsync(receteIcerik);

            await _genelReceteIcerikDeposu
                .KaydetAsync();

            return receteIcerik;
        }

        public async Task<bool> GuncelleAsync(
            ReceteIcerik receteIcerik)
        {
            var mevcutIcerik =
                await _genelReceteIcerikDeposu
                    .IdIleGetirAsync(receteIcerik.Id);

            if (mevcutIcerik is null)
            {
                return false;
            }

            IcerikBilgileriniDogrula(receteIcerik);
            BilgileriTemizle(receteIcerik);

            mevcutIcerik.KullanimTalimatlari =
                receteIcerik.KullanimTalimatlari;

            mevcutIcerik.KullanimSuresi =
                receteIcerik.KullanimSuresi;

            mevcutIcerik.Miktar = receteIcerik.Miktar;

            mevcutIcerik.GuncellenmeTarihi =
                DateTime.UtcNow;

            _genelReceteIcerikDeposu
                .Guncelle(mevcutIcerik);

            await _genelReceteIcerikDeposu
                .KaydetAsync();

            return true;
        }

        public async Task<bool> SilAsync(int id)
        {
            var receteIcerik =
                await _genelReceteIcerikDeposu
                    .IdIleGetirAsync(id);

            if (receteIcerik is null)
            {
                return false;
            }

            _genelReceteIcerikDeposu.Sil(receteIcerik);

            await _genelReceteIcerikDeposu
                .KaydetAsync();

            return true;
        }

        private static void IcerikBilgileriniDogrula(
            ReceteIcerik receteIcerik)
        {
            if (receteIcerik.Miktar < 1)
            {
                throw new ArgumentException(
                    "Miktar en az 1 olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(
                    receteIcerik.KullanimTalimatlari))
            {
                throw new ArgumentException(
                    "Kullanım talimatı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    receteIcerik.KullanimSuresi))
            {
                throw new ArgumentException(
                    "Kullanım süresi boş olamaz.");
            }
        }

        private static void BilgileriTemizle(
            ReceteIcerik receteIcerik)
        {
            receteIcerik.KullanimTalimatlari =
                receteIcerik.KullanimTalimatlari.Trim();

            receteIcerik.KullanimSuresi =
                receteIcerik.KullanimSuresi.Trim();
        }
    }
}