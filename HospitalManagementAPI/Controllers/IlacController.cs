using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Ilaclar;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IlacController : ControllerBase
    {
        private readonly IIlacServisi _ilacServisi;

        public IlacController(
            IIlacServisi ilacServisi)
        {
            _ilacServisi = ilacServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ilaclar =
                await _ilacServisi.TumunuGetirAsync();

            var cevaplar = ilaclar
                .Select(DtoyaDonustur)
                .ToList();

            return Ok(cevaplar);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ilac =
                await _ilacServisi.IdIleGetirAsync(id);

            if (ilac is null)
            {
                return NotFound("İlaç bulunamadı.");
            }

            return Ok(DtoyaDonustur(ilac));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPost]
        public async Task<IActionResult> Create(
            IlacOlusturmaDto dto)
        {
            var yeniIlac = new Ilac
            {
                Ad = dto.Ad
            };

            await _ilacServisi.EkleAsync(yeniIlac);

            return CreatedAtAction(
                nameof(GetById),
                new { id = yeniIlac.Id },
                DtoyaDonustur(yeniIlac));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            IlacGuncellemeDto dto)
        {
            var ilac = new Ilac
            {
                Id = id,
                Ad = dto.Ad
            };

            var guncellendiMi =
                await _ilacServisi.GuncelleAsync(ilac);

            if (!guncellendiMi)
            {
                return NotFound("İlaç bulunamadı.");
            }

            return NoContent();
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pasiflestirildiMi =
                await _ilacServisi.PasiflestirAsync(id);

            if (!pasiflestirildiMi)
            {
                return NotFound("İlaç bulunamadı.");
            }

            return NoContent();
        }

        private static IlacYanitDto DtoyaDonustur(
            Ilac ilac)
        {
            return new IlacYanitDto
            {
                Id = ilac.Id,
                Ad = ilac.Ad,
                AktifMi = ilac.AktifMi,
                OlusturulmaTarihi =
                    ilac.OlusturulmaTarihi,
                GuncellenmeTarihi =
                    ilac.GuncellenmeTarihi
            };
        }
    }
}