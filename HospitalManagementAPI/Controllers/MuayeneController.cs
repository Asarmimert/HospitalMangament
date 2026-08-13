using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Common;
using HospitalManagementAPI.DTOs.Muayeneler;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.Uzantilar;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class MuayeneController : ControllerBase
    {
        private readonly IMuayeneServisi _muayeneServisi;
        private readonly IRandevuServisi _randevuServisi;
        private readonly IHastaServisi _hastaServisi;
        private readonly IDoktorServisi _doktorServisi;

        public MuayeneController(
      IMuayeneServisi muayeneServisi,
      IRandevuServisi randevuServisi,
      IHastaServisi hastaServisi,
      IDoktorServisi doktorServisi)
        {
            _muayeneServisi = muayeneServisi;
            _randevuServisi = randevuServisi;
            _hastaServisi = hastaServisi;
            _doktorServisi = doktorServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
      [FromQuery] MuayeneFiltrelemeDto filtre,
      CancellationToken cancellationToken)
        {
            var kullaniciHesabiId =
    User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Hasta)))
            {
                var hasta =
                    await _hastaServisi
                        .KullaniciHesabiIdIleGetirAsync(
                            kullaniciHesabiId.Value);

                if (hasta is null)
                {
                    return Forbid();
                }

                filtre.HastaId = hasta.Id;
            }
            else if (User.IsInRole(nameof(KullaniciRolu.Doktor)))
            {
                var doktor =
                    await _doktorServisi
                        .KullaniciHesabiIdIleGetirAsync(
                            kullaniciHesabiId.Value);

                if (doktor is null)
                {
                    return Forbid();
                }

                filtre.DoktorId = doktor.Id;
            }
            var sonuc =
    await _muayeneServisi.ListeleAsync(
        filtre.SayfaNo,
        filtre.SayfaBoyutu,
        filtre.DoktorId,
        filtre.HastaId,
        filtre.BaslangicTarihi,
        filtre.BitisTarihi,
        cancellationToken);

            var muayeneDtoListesi = sonuc.Muayeneler
                .Select(DtoyaDonustur)
                .ToList();

            var cevap =
                new SayfalanmisResponseDto<MuayeneYanitDto>
                {
                    Kayitlar = muayeneDtoListesi,
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
            var muayene =
                await _muayeneServisi.IdIleGetirAsync(id);

            if (muayene is null)
            {
                return NotFound(
                    new { mesaj = "Muayene bulunamadı." });
            }
            var kullaniciHesabiId =
             User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Hasta)) &&
                muayene.Randevu.Hasta.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Doktor)) &&
                muayene.Randevu.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }
            return Ok(DtoyaDonustur(muayene));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPost]
        public async Task<IActionResult> Create(
     MuayeneOlusturmaDto dto)
        {
            var kullaniciHesabiId =
                User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            var randevu =
                await _randevuServisi.IdIleGetirAsync(
                    dto.RandevuId);

            if (randevu is null)
            {
                return NotFound(
                    new { mesaj = "Randevu bulunamadı." });
            }

            if (randevu.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var yeniMuayene = new Muayene
            {
                RandevuId = dto.RandevuId,
                HastaSikayeti = dto.HastaSikayeti,
                DoktorDegerlendirmesi =
                    dto.DoktorDegerlendirmesi,
                DoktorNotlari = dto.DoktorNotlari,
                MuayeneTarihi = dto.MuayeneTarihi
            };

            var eklenenMuayene =
                await _muayeneServisi.EkleAsync(
                    yeniMuayene);

            var iliskiliMuayene =
                await _muayeneServisi.IdIleGetirAsync(
                    eklenenMuayene.Id);

            if (iliskiliMuayene is null)
            {
                throw new InvalidOperationException(
                    "Eklenen muayene bilgileri alınamadı.");
            }

            var cevap = DtoyaDonustur(iliskiliMuayene);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.Id },
                cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
      int id,
      MuayeneGuncellemeDto dto)
        {
            var mevcutMuayene =
                await _muayeneServisi.IdIleGetirAsync(id);

            if (mevcutMuayene is null)
            {
                return NotFound(
                    new { mesaj = "Muayene bulunamadı." });
            }

            var kullaniciHesabiId =
                User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (mevcutMuayene.Randevu
                    .Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var guncellenecekMuayene = new Muayene
            {
                Id = id,
                HastaSikayeti = dto.HastaSikayeti,
                DoktorDegerlendirmesi =
                    dto.DoktorDegerlendirmesi,
                DoktorNotlari = dto.DoktorNotlari,
                MuayeneTarihi = dto.MuayeneTarihi
            };

            var guncellendiMi =
                await _muayeneServisi.GuncelleAsync(
                    guncellenecekMuayene);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Muayene bulunamadı." });
            }

            return NoContent();
        }

        private static MuayeneYanitDto DtoyaDonustur(
            Muayene muayene)
        {
            return new MuayeneYanitDto
            {
                Id = muayene.Id,
                RandevuId = muayene.RandevuId,

                DoktorId =
                    muayene.Randevu.DoktorId,

                DoktorAdiSoyadi =
                    $"{muayene.Randevu.Doktor.DoktorAd} " +
                    $"{muayene.Randevu.Doktor.DoktorSoyad}",

                HastaId =
                    muayene.Randevu.HastaId,

                HastaAdiSoyadi =
                    $"{muayene.Randevu.Hasta.Ad} " +
                    $"{muayene.Randevu.Hasta.Soyad}",

                HastaSikayeti =
                    muayene.HastaSikayeti,

                DoktorDegerlendirmesi =
                    muayene.DoktorDegerlendirmesi,

                DoktorNotlari =
                    muayene.DoktorNotlari,

                MuayeneTarihi =
                    muayene.MuayeneTarihi,

                OlusturulmaTarihi =
                    muayene.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    muayene.GuncellenmeTarihi
            };
        }
    }
}