using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Teshisler;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeshisController : ControllerBase
    {
        private readonly ITeshisServisi _teshisServisi;

        public TeshisController(
            ITeshisServisi teshisServisi)
        {
            _teshisServisi = teshisServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teshisler =
                await _teshisServisi.TumunuGetirAsync();

            var cevap = teshisler
                .Select(DtoyaDonustur)
                .ToList();

            return Ok(cevap);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teshis =
                await _teshisServisi.IdIleGetirAsync(id);

            if (teshis is null)
            {
                return NotFound(
                    new { mesaj = "Teşhis bulunamadı." });
            }

            return Ok(DtoyaDonustur(teshis));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPost]
        public async Task<IActionResult> Create(
            TeshisOlusturmaDto dto)
        {
            var yeniTeshis = new Teshis
            {
                TeshisKodu = dto.TeshisKodu,
                TeshisAdi = dto.TeshisAdi,
                Aciklama = dto.Aciklama
            };

            var eklenenTeshis =
                await _teshisServisi.EkleAsync(
                    yeniTeshis);

            var cevap = DtoyaDonustur(eklenenTeshis);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.Id },
                cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            TeshisGuncellemeDto dto)
        {
            var guncellenecekTeshis = new Teshis
            {
                Id = id,
                TeshisKodu = dto.TeshisKodu,
                TeshisAdi = dto.TeshisAdi,
                Aciklama = dto.Aciklama
            };

            var guncellendiMi =
                await _teshisServisi.GuncelleAsync(
                    guncellenecekTeshis);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Aktif teşhis bulunamadı." });
            }

            return NoContent();
        }
        [Authorize(Roles = nameof(KullaniciRolu.Doktor))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pasiflestirildiMi =
                await _teshisServisi.PasiflestirAsync(id);

            if (!pasiflestirildiMi)
            {
                return NotFound(
                    new { mesaj = "Aktif teşhis bulunamadı." });
            }

            return NoContent();
        }

        private static TeshisYanitDto DtoyaDonustur(
            Teshis teshis)
        {
            return new TeshisYanitDto
            {
                Id = teshis.Id,
                TeshisKodu = teshis.TeshisKodu,
                TeshisAdi = teshis.TeshisAdi,
                Aciklama = teshis.Aciklama,
                AktifMi = teshis.AktifMi,

                OlusturulmaTarihi =
                    teshis.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    teshis.GuncellenmeTarihi
            };
        }
    }
}