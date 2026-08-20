using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Common;
using HospitalManagementAPI.DTOs.Receteler;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementAPI.Uzantilar;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceteController : ControllerBase
    {
        private readonly IReceteServisi _receteServisi;
        private readonly IHastaServisi _hastaServisi;
        private readonly IDoktorServisi _doktorServisi;
        private readonly IMuayeneServisi _muayeneServisi;
        public ReceteController(
     IReceteServisi receteServisi,
     IHastaServisi hastaServisi,
     IDoktorServisi doktorServisi,
     IMuayeneServisi muayeneServisi)
        {
            _receteServisi = receteServisi;
            _hastaServisi = hastaServisi;
            _doktorServisi = doktorServisi;
            _muayeneServisi = muayeneServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ReceteFiltrelemeDto filtre)
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
            var sonuc = await _receteServisi.ListeleAsync(
                filtre.SayfaNo,
                filtre.SayfaBoyutu,
                filtre.HastaId,
                filtre.DoktorId,
                filtre.BaslangicTarihi,
                filtre.BitisTarihi);

            var kayitlar = sonuc.Receteler
                .Select(DtoyaDonustur)
                .ToList();

            var toplamSayfaSayisi = (int)Math.Ceiling(
                sonuc.ToplamKayitSayisi /
                (double)filtre.SayfaBoyutu);

            var cevap =
     new SayfalanmisResponseDto<ReceteYanitDto>
     {
         Kayitlar = kayitlar,
         SayfaNo = filtre.SayfaNo,
         SayfaBoyutu = filtre.SayfaBoyutu,
         ToplamKayitSayisi =
             sonuc.ToplamKayitSayisi,
         ToplamSayfaSayisi =
             toplamSayfaSayisi
     };

            return Ok(cevap);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recete =
                await _receteServisi.IdIleGetirAsync(id);

            if (recete is null)
            {
                return NotFound("Reçete bulunamadı.");
            }
            var kullaniciHesabiId =
    User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Hasta)) &&
                recete.Hasta.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            if (User.IsInRole(nameof(KullaniciRolu.Doktor)) &&
                recete.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }
            return Ok(DtoyaDonustur(recete));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]

        [HttpPost]
        public async Task<IActionResult> Create(
            ReceteOlusturmaDto dto)
        {
            var kullaniciHesabiId =
    User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            var muayene =
                await _muayeneServisi.IdIleGetirAsync(
                    dto.MuayeneId);

            if (muayene is null)
            {
                return NotFound(
                    new { mesaj = "Muayene bulunamadı." });
            }

            if (muayene.Randevu.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }
            var yeniRecete = new Recete
            {
                MuayeneId = dto.MuayeneId,
                GenelNotlar = dto.GenelNotlar
            };

            await _receteServisi.EkleAsync(yeniRecete);

            var detayliRecete =
                await _receteServisi.IdIleGetirAsync(
                    yeniRecete.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = yeniRecete.Id },
                DtoyaDonustur(detayliRecete!));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
       int id,
       ReceteGuncellemeDto dto)
        {
            var mevcutRecete =
                await _receteServisi.IdIleGetirAsync(id);

            if (mevcutRecete is null)
            {
                return NotFound("Reçete bulunamadı.");
            }

            var kullaniciHesabiId =
                User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            if (mevcutRecete.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var recete = new Recete
            {
                Id = id,
                GenelNotlar = dto.GenelNotlar
            };

            var guncellendiMi =
                await _receteServisi.GuncelleAsync(recete);

            if (!guncellendiMi)
            {
                return NotFound("Reçete bulunamadı.");
            }

            return NoContent();
        }
        private static ReceteYanitDto DtoyaDonustur(
    Recete recete)
        {
            return new ReceteYanitDto
            {
                Id = recete.Id,
                MuayeneId = recete.MuayeneId,
                HastaId = recete.HastaId,

                HastaAdiSoyadi =
                    $"{recete.Hasta.Ad} {recete.Hasta.Soyad}",

                HastaKimlikNumarasi =
                    recete.Hasta.KimlikNumarasi,

                DoktorId = recete.DoktorId,

                DoktorAdiSoyadi =
                    $"{recete.Doktor.DoktorAd} " +
                    $"{recete.Doktor.DoktorSoyad}",

                ReceteTarihi = recete.ReceteTarihi,
                GenelNotlar = recete.GenelNotlar,

                OlusturulmaTarihi =
                    recete.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    recete.GuncellenmeTarihi
            };
        }
    }
}