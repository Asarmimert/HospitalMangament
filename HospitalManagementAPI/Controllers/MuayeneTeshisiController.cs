using HospitalManagement.Business.Somut;
using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.DTOs.MuayeneTeshisleri;
using Microsoft.AspNetCore.Authorization;
using HospitalManagementAPI.Uzantilar;
using Microsoft.AspNetCore.Mvc;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuayeneTeshisiController : ControllerBase
    {
        private readonly IMuayeneTeshisiServisi
    _muayeneTeshisiServisi;

        private readonly IMuayeneServisi
            _muayeneServisi;

        public MuayeneTeshisiController(
       IMuayeneTeshisiServisi muayeneTeshisiServisi,
       IMuayeneServisi muayeneServisi)
        {
            _muayeneTeshisiServisi =
                muayeneTeshisiServisi;

            _muayeneServisi =
                muayeneServisi;
        }

        [HttpGet("muayene/{muayeneId:int}")]
        public async Task<IActionResult> GetByMuayeneId(
     int muayeneId)
        {
            var muayene =
                await _muayeneServisi.IdIleGetirAsync(
                    muayeneId);

            if (muayene is null)
            {
                return NotFound("Muayene bulunamadı.");
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

            var kayitlar =
                await _muayeneTeshisiServisi
                    .MuayeneyeGoreListeleAsync(muayeneId);

            var cevaplar = kayitlar
                .Select(DtoyaDonustur)
                .ToList();

            return Ok(cevaplar);
        }
        

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var kayit =
                await _muayeneTeshisiServisi
                    .IdIleGetirAsync(id);

            if (kayit is null)
            {
                return NotFound(
                    "Muayene teşhis kaydı bulunamadı.");
            }

            var muayene =
                await _muayeneServisi.IdIleGetirAsync(
                    kayit.MuayeneId);

            if (muayene is null)
            {
                return NotFound("Muayene bulunamadı.");
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

            return Ok(DtoyaDonustur(kayit));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPost]
        public async Task<IActionResult> Create(
     MuayeneTeshisiOlusturmaDto dto)
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

            var yeniKayit = new MuayeneTeshisi
            {
                MuayeneId = dto.MuayeneId,
                TeshisId = dto.TeshisId,
                DoktorNotu = dto.DoktorNotu
            };

            await _muayeneTeshisiServisi
                .EkleAsync(yeniKayit);

            var detayliKayit =
                await _muayeneTeshisiServisi
                    .IdIleGetirAsync(yeniKayit.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = yeniKayit.Id },
                DtoyaDonustur(detayliKayit!));
        }

        private static MuayeneTeshisiYanitDto DtoyaDonustur(
            MuayeneTeshisi kayit)
        {
            return new MuayeneTeshisiYanitDto
            {
                Id = kayit.Id,
                MuayeneId = kayit.MuayeneId,
                TeshisId = kayit.TeshisId,
                TeshisKodu = kayit.Teshis.TeshisKodu,
                TeshisAdi = kayit.Teshis.TeshisAdi,
                DoktorNotu = kayit.DoktorNotu,
                OlusturulmaTarihi =
                    kayit.OlusturulmaTarihi,
                GuncellenmeTarihi =
                    kayit.GuncellenmeTarihi
            };
        }
    }
}