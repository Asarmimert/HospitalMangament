using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Common;
using HospitalManagementAPI.DTOs.Doctors;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoktorServisi _doktorServisi;

        public DoctorsController(
            IDoktorServisi doktorServisi)
        {
            _doktorServisi = doktorServisi;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DoctorFilterDto filtre)
        {
            var sonuc =
                await _doktorServisi.ListeleAsync(
                    filtre.SayfaNo,
                    filtre.SayfaBoyutu,
                    filtre.DepartmentId,
                    filtre.AktifMi,
                    filtre.Arama);

            var doktorDtoListesi = sonuc.Doktorlar
                .Select(DtoyaDonustur)
                .ToList();

            var cevap =
                new SayfalanmisResponseDto<DoctorResponseDto>
                {
                    Kayitlar = doktorDtoListesi,
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
            var doktor =
                await _doktorServisi.IdIleGetirAsync(id);

            if (doktor is null)
            {
                return NotFound(
                    new { mesaj = "Doktor bulunamadı." });
            }

            return Ok(DtoyaDonustur(doktor));
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateDoctorDto dto)
        {
            var yeniDoktor = new Doctor
            {
                KullaniciHesabiId =
                    dto.KullaniciHesabiId,

                DepartmentId = dto.DepartmentId,
                DoktorAd = dto.DoktorAd,
                DoktorSoyad = dto.DoktorSoyad,
                TelefonNumarasi = dto.TelefonNumarasi,
                UzmanlikAlani = dto.UzmanlikAlani
            };

            var eklenenDoktor =
                await _doktorServisi.EkleAsync(yeniDoktor);

            // Departman ve kullanıcı hesabı bilgilerini de yükler.
            var iliskiliDoktor =
                await _doktorServisi.IdIleGetirAsync(
                    eklenenDoktor.Id);

            if (iliskiliDoktor is null)
            {
                throw new InvalidOperationException(
                    "Eklenen doktor bilgileri alınamadı.");
            }

            var cevap = DtoyaDonustur(iliskiliDoktor);

            return CreatedAtAction(
                nameof(GetById),
                new { id = cevap.Id },
                cevap);
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDoctorDto dto)
        {
            var guncellenecekDoktor = new Doctor
            {
                Id = id,
                DepartmentId = dto.DepartmentId,
                DoktorAd = dto.DoktorAd,
                DoktorSoyad = dto.DoktorSoyad,
                TelefonNumarasi = dto.TelefonNumarasi,
                UzmanlikAlani = dto.UzmanlikAlani
            };

            var guncellendiMi =
                await _doktorServisi.GuncelleAsync(
                    guncellenecekDoktor);

            if (!guncellendiMi)
            {
                return NotFound(
                    new { mesaj = "Aktif doktor bulunamadı." });
            }

            return NoContent();
        }
        [Authorize(Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pasiflestirildiMi =
                await _doktorServisi.PasiflestirAsync(id);

            if (!pasiflestirildiMi)
            {
                return NotFound(
                    new { mesaj = "Aktif doktor bulunamadı." });
            }

            return NoContent();
        }

        private static DoctorResponseDto DtoyaDonustur(
            Doctor doktor)
        {
            return new DoctorResponseDto
            {
                Id = doktor.Id,

                KullaniciHesabiId =
                    doktor.KullaniciHesabiId,

                Eposta =
                    doktor.KullaniciHesabi.Eposta,

                DepartmentId = doktor.DepartmentId,

                DepartmanAdi =
                    doktor.Department.Name,

                DoktorAd = doktor.DoktorAd,
                DoktorSoyad = doktor.DoktorSoyad,

                TelefonNumarasi =
                    doktor.TelefonNumarasi,

                UzmanlikAlani =
                    doktor.UzmanlikAlani,

                AktifMi = doktor.AktifMi,

                OlusturulmaTarihi =
                    doktor.OlusturulmaTarihi,

                GuncellenmeTarihi =
                    doktor.GuncellenmeTarihi
            };
        }
    }
}