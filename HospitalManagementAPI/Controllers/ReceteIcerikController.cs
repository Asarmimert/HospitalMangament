using HospitalManagement.Business.Somut;
using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.DTOs.ReceteIcerikleri;
using HospitalManagementAPI.Uzantilar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceteIcerikController : ControllerBase
    {
        private readonly IReceteIcerikServisi
            _receteIcerikServisi;

        private readonly IReceteServisi
            _receteServisi;

        public ReceteIcerikController(
            IReceteIcerikServisi receteIcerikServisi,
            IReceteServisi receteServisi)
        {
            _receteIcerikServisi =
                receteIcerikServisi;

            _receteServisi =
                receteServisi;
        }

        [HttpGet("recete/{receteId:int}")]
        public async Task<IActionResult> GetByReceteId(
    int receteId)
        {
            var recete =
                await _receteServisi.IdIleGetirAsync(receteId);

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

            var icerikler =
                await _receteIcerikServisi
                    .ReceteyeGoreListeleAsync(receteId);

            var cevaplar = icerikler
                .Select(DtoyaDonustur)
                .ToList();

            return Ok(cevaplar);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var icerik =
                await _receteIcerikServisi.IdIleGetirAsync(id);

            if (icerik is null)
            {
                return NotFound(
                    "Reçete içeriği bulunamadı.");
            }

            var recete =
                await _receteServisi.IdIleGetirAsync(
                    icerik.ReceteId);

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

            return Ok(DtoyaDonustur(icerik));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPost]
        public async Task<IActionResult> Create(
    ReceteIcerikOlusturmaDto dto)
        {
            var kullaniciHesabiId =
                User.KullaniciIdGetir();

            if (!kullaniciHesabiId.HasValue)
            {
                return Unauthorized();
            }

            var recete =
                await _receteServisi.IdIleGetirAsync(
                    dto.ReceteId);

            if (recete is null)
            {
                return NotFound(
                    new { mesaj = "Reçete bulunamadı." });
            }

            if (recete.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var yeniIcerik = new ReceteIcerik
            {
                ReceteId = dto.ReceteId,
                IlacId = dto.IlacId,
                KullanimTalimatlari =
                    dto.KullanimTalimatlari,
                KullanimSuresi = dto.KullanimSuresi,
                Miktar = dto.Miktar
            };

            await _receteIcerikServisi
                .EkleAsync(yeniIcerik);

            var detayliIcerik =
                await _receteIcerikServisi
                    .IdIleGetirAsync(yeniIcerik.Id);

            return CreatedAtAction(
                nameof(GetById),
                new { id = yeniIcerik.Id },
                DtoyaDonustur(detayliIcerik!));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
     int id,
     ReceteIcerikGuncellemeDto dto)
        {
            var mevcutIcerik =
                await _receteIcerikServisi.IdIleGetirAsync(id);

            if (mevcutIcerik is null)
            {
                return NotFound(
                    "Reçete içeriği bulunamadı.");
            }

            var recete =
                await _receteServisi.IdIleGetirAsync(
                    mevcutIcerik.ReceteId);

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

            if (recete.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var guncellenecekIcerik = new ReceteIcerik
            {
                Id = id,
                KullanimTalimatlari =
                    dto.KullanimTalimatlari,
                KullanimSuresi = dto.KullanimSuresi,
                Miktar = dto.Miktar
            };

            var guncellendiMi =
                await _receteIcerikServisi.GuncelleAsync(
                    guncellenecekIcerik);

            if (!guncellendiMi)
            {
                return NotFound(
                    "Reçete içeriği bulunamadı.");
            }

            return NoContent();
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mevcutIcerik =
                await _receteIcerikServisi.IdIleGetirAsync(id);

            if (mevcutIcerik is null)
            {
                return NotFound(
                    "Reçete içeriği bulunamadı.");
            }

            var recete =
                await _receteServisi.IdIleGetirAsync(
                    mevcutIcerik.ReceteId);

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

            if (recete.Doktor.KullaniciHesabiId !=
                kullaniciHesabiId.Value)
            {
                return Forbid();
            }

            var silindiMi =
                await _receteIcerikServisi.SilAsync(id);

            if (!silindiMi)
            {
                return NotFound(
                    "Reçete içeriği bulunamadı.");
            }

            return NoContent();
        }

        private static ReceteIcerikYanitDto
            DtoyaDonustur(ReceteIcerik icerik)
        {
            return new ReceteIcerikYanitDto
            {
                Id = icerik.Id,
                ReceteId = icerik.ReceteId,
                IlacId = icerik.IlacId,
                IlacAdi = icerik.Ilac.Ad,
                KullanimTalimatlari =
                    icerik.KullanimTalimatlari,
                KullanimSuresi =
                    icerik.KullanimSuresi,
                Miktar = icerik.Miktar,
                OlusturulmaTarihi =
                    icerik.OlusturulmaTarihi,
                GuncellenmeTarihi =
                    icerik.GuncellenmeTarihi
            };
        }
    }
}