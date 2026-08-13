using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.DTOs.Common;
using HospitalManagementAPI.DTOs.Patients;
using HospitalManagementAPI.Uzantilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HastaController : ControllerBase
    {
        private readonly IHastaServisi _hastaServisi;

        public HastaController(IHastaServisi hastaServisi)
        {
            _hastaServisi = hastaServisi;
        }
        [Authorize(
        Roles = nameof(KullaniciRolu.Doktor) +
            "," +
            nameof(KullaniciRolu.Sekreter))]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PatientFilterDto filtre)
        {
            var sonuc = await _hastaServisi.ListeleAsync(
                filtre.SayfaNo,
                filtre.SayfaBoyutu,
                filtre.AktifMi,
                filtre.Arama);

            var hastaDtoListesi = sonuc.Hastalar
                .Select(DtoyaDonustur)
                .ToList();

            var cevap =
                new SayfalanmisResponseDto<PatientResponseDto>
                {
                    Kayitlar = hastaDtoListesi,
                    SayfaNo = filtre.SayfaNo,
                    SayfaBoyutu = filtre.SayfaBoyutu,

                    ToplamKayitSayisi =
                        sonuc.ToplamKayitSayisi,

                    ToplamSayfaSayisi =
                        (int)Math.Ceiling(
                            sonuc.ToplamKayitSayisi /
                            (double)filtre.SayfaBoyutu)
                };

            return Ok(cevap);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hasta =
                await _hastaServisi.IdIleGetirAsync(id);

            if (hasta is null)
            {
                return NotFound(
                    new { mesaj = "Hasta bulunamadı." });
            }

            var kullaniciId = User.KullaniciIdGetir();
            var rol = User.RolGetir();

            if (rol == nameof(KullaniciRolu.Hasta) &&
                hasta.KullaniciHesabiId != kullaniciId)
            {
                return Forbid();
            }

            return Ok(DtoyaDonustur(hasta));
        }
        [Authorize(
       Roles = nameof(KullaniciRolu.Hasta) +
               "," +
               nameof(KullaniciRolu.Sekreter))]
        [HttpPost]
        public async Task<IActionResult> Create(
       CreatePatientDto dto)
        {
            var tokenKullaniciId =
                User.KullaniciIdGetir();

            if (!tokenKullaniciId.HasValue)
            {
                return Unauthorized();
            }

            var kullaniciHesabiId =
                User.IsInRole(nameof(KullaniciRolu.Hasta))
                    ? tokenKullaniciId.Value
                    : dto.KullaniciHesabiId;

            var yeniHasta = new Hasta
            {
                KullaniciHesabiId = kullaniciHesabiId,
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                KimlikNumarasi = dto.KimlikNumarasi,
                DogumTarihi = dto.DogumTarihi,
                TelefonNumarasi = dto.TelefonNumarasi,
                Adres = dto.Adres
            };

            var eklenenHasta =
                await _hastaServisi.EkleAsync(yeniHasta);

            var iliskiliHasta =
                await _hastaServisi.IdIleGetirAsync(
                    eklenenHasta.Id);

            if (iliskiliHasta is null)
            {
                throw new InvalidOperationException(
                    "Eklenen hasta bilgileri alınamadı.");
            }

            var cevap = DtoyaDonustur(iliskiliHasta);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.Id },
                cevap);
        }

       [Authorize(
    Roles = nameof(KullaniciRolu.Hasta) +
            "," +
            nameof(KullaniciRolu.Sekreter))]
[HttpPut("{id:int}")]
public async Task<IActionResult> Update(
    int id,
    UpdatePatientDto dto)
{
    var mevcutHasta =
        await _hastaServisi.IdIleGetirAsync(id);

    if (mevcutHasta is null)
    {
        return NotFound(
            new { mesaj = "Hasta bulunamadı." });
    }

    var tokenKullaniciId =
        User.KullaniciIdGetir();

    if (User.IsInRole(nameof(KullaniciRolu.Hasta)) &&
        mevcutHasta.KullaniciHesabiId != tokenKullaniciId)
    {
        return Forbid();
    }

    var guncellenecekHasta = new Hasta
    {
        Id = id,
        Ad = dto.Ad,
        Soyad = dto.Soyad,
        DogumTarihi = dto.DogumTarihi,
        TelefonNumarasi = dto.TelefonNumarasi,
        Adres = dto.Adres
    };

    var guncellendiMi =
        await _hastaServisi.GuncelleAsync(
            guncellenecekHasta);

    if (!guncellendiMi)
    {
        return NotFound(
            new { mesaj = "Hasta bulunamadı." });
    }

    return NoContent();
}
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pasiflestirildiMi =
                await _hastaServisi.PasiflestirAsync(id);

            if (!pasiflestirildiMi)
            {
                return NotFound(
                    new { mesaj = "Aktif hasta bulunamadı." });
            }

            return NoContent();
        }

        private static PatientResponseDto DtoyaDonustur(
            Hasta hasta)
        {
            return new PatientResponseDto
            {
                Id = hasta.Id,

                KullaniciHesabiId =
                    hasta.KullaniciHesabiId,

                Eposta =
                    hasta.KullaniciHesabi.Eposta,

                Ad = hasta.Ad,
                Soyad = hasta.Soyad,
                KimlikNumarasi = hasta.KimlikNumarasi,
                DogumTarihi = hasta.DogumTarihi,
                TelefonNumarasi = hasta.TelefonNumarasi,
                Adres = hasta.Adres,
                AktifMi = hasta.AktifMi,

                OlusturulmaTarihi =
                    hasta.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    hasta.GuncellenmeTarihi
            };
        }
    }
}