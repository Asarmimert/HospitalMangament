using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Common;
using HospitalManagementAPI.DTOs.Randevular;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using HospitalManagementAPI.Uzantilar;

namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandevuController : ControllerBase
    {
        private readonly IRandevuServisi _randevuServisi;
        private readonly IHastaServisi _hastaServisi;
        private readonly IDoktorServisi _doktorServisi;

        public RandevuController(
     IRandevuServisi randevuServisi,
     IHastaServisi hastaServisi,
     IDoktorServisi doktorServisi)
        {
            _randevuServisi = randevuServisi;
            _hastaServisi = hastaServisi;
            _doktorServisi = doktorServisi;
        }
        [HttpGet]
        [ProducesResponseType(
            typeof(SayfalanmisResponseDto<RandevuYanitDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
            [FromQuery] RandevuFiltrelemeDto filtre,
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
                await _randevuServisi.ListeleAsync(
                    filtre.SayfaNo,
                    filtre.SayfaBoyutu,
                    filtre.DoktorId,
                    filtre.HastaId,
                    filtre.Durum,
                    filtre.BaslangicTarihi,
                    filtre.BitisTarihi,
                    cancellationToken);

            var randevuDtoListesi = sonuc.Randevular
                .Select(DtoyaDonustur)
                .ToList();

            var cevap =
                new SayfalanmisResponseDto<RandevuYanitDto>
                {
                    Kayitlar = randevuDtoListesi,
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
        [ProducesResponseType(
     typeof(RandevuYanitDto),
     StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var randevu =
                await _randevuServisi.IdIleGetirAsync(id);

            if (randevu is null)
            {
                return NotFound(
                    new { mesaj = "Randevu bulunamadı." });
            }
            var kullaniciHesabiId =
    User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Hasta)) &&
                randevu.Hasta.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Doktor)) &&
                randevu.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            return Ok(DtoyaDonustur(randevu));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPost]
        [ProducesResponseType(
    typeof(RandevuYanitDto),
    StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
    RandevuOlusturmaDto dto)
        {
            var yeniRandevu = new Randevu
            {
                DoktorId = dto.DoktorId,
                HastaId = dto.HastaId,

                OlusturanSekreterId =
                    dto.OlusturanSekreterId,

                BaslangicZamani =
                    dto.BaslangicZamani,

                BitisZamani =
                    dto.BitisZamani
            };

            var eklenenRandevu =
                await _randevuServisi.EkleAsync(
                    yeniRandevu);

            var iliskiliRandevu =
                await _randevuServisi.IdIleGetirAsync(
                    eklenenRandevu.Id);

            if (iliskiliRandevu is null)
            {
                throw new InvalidOperationException(
                    "Eklenen randevu bilgileri alınamadı.");
            }

            var cevap = DtoyaDonustur(iliskiliRandevu);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.Id },
                cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
     int id,
     RandevuGuncellemeDto dto)
        {
            var guncellenecekRandevu = new Randevu
            {
                Id = id,
                DoktorId = dto.DoktorId,
                HastaId = dto.HastaId,

                BaslangicZamani =
                    dto.BaslangicZamani,

                BitisZamani =
                    dto.BitisZamani
            };

            var guncellendiMi =
                await _randevuServisi.GuncelleAsync(
                    guncellenecekRandevu);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Randevu bulunamadı." });
            }

            return NoContent();
        }
        [Authorize(
        Roles = nameof(KullaniciRolu.Sekreter) +
            "," +
            nameof(KullaniciRolu.Doktor))]
        [HttpPatch("{id:int}/durum")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DurumGuncelle(
        int id,
        RandevuDurumGuncellemeDto dto)
        {
            var randevu =
                await _randevuServisi.IdIleGetirAsync(id);

            if (randevu is null)
            {
                return NotFound(
                    new { mesaj = "Randevu bulunamadı." });
            }

            if (User.IsInRole(nameof(KullaniciRolu.Doktor)))
            {
                var kullaniciHesabiId =
                    User.KullaniciIdGetir();

                if (!kullaniciHesabiId.HasValue)
                {
                    return Unauthorized();
                }

                if (randevu.Doktor.KullaniciHesabiId !=
                    kullaniciHesabiId.Value)
                {
                    return Forbid();
                }
            }

            var guncellendiMi =
                await _randevuServisi.DurumGuncelleAsync(
                    id,
                    dto.Durum,
                    dto.IptalNedeni);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Randevu bulunamadı." });
            }

            return NoContent();
        }

        private static RandevuYanitDto DtoyaDonustur(
            Randevu randevu)
        {
            return new RandevuYanitDto
            {
                Id = randevu.Id,

                DoktorId = randevu.DoktorId,

                DoktorAdiSoyadi =
                    $"{randevu.Doktor.DoktorAd} " +
                    $"{randevu.Doktor.DoktorSoyad}",

                HastaId = randevu.HastaId,

                HastaAdiSoyadi =
                    $"{randevu.Hasta.Ad} " +
                    $"{randevu.Hasta.Soyad}",

                OlusturanSekreterId =
                    randevu.OlusturanSekreterId,

                OlusturanSekreterAdiSoyadi =
                    $"{randevu.OlusturanSekreter.Ad} " +
                    $"{randevu.OlusturanSekreter.Soyad}",

                BaslangicZamani =
                    randevu.BaslangicZamani,

                BitisZamani =
                    randevu.BitisZamani,

                Durum = randevu.Durum,
                DurumAdi = randevu.Durum.ToString(),
                IptalNedeni = randevu.IptalNedeni,

                OlusturulmaTarihi =
                    randevu.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    randevu.GuncellenmeTarihi
            };
        }
    }
}