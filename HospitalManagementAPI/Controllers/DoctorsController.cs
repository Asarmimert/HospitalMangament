using HospitalManagement.Business.Soyut;
using Microsoft.AspNetCore.Identity;
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

        private readonly IPasswordHasher<KullaniciHesabi>
    _parolaHasher;
        public DoctorsController(
     IDoktorServisi doktorServisi,
     IPasswordHasher<KullaniciHesabi> parolaHasher)
        {
            _doktorServisi = doktorServisi;
            _parolaHasher = parolaHasher;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DoctorFilterDto filtre,
            CancellationToken cancellationToken)
        {
            var sonuc =
     await _doktorServisi.ListeleAsync(
         filtre.SayfaNo,
         filtre.SayfaBoyutu,
         filtre.DepartmentId,
         filtre.AktifMi,
         filtre.Arama,
         cancellationToken);

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
        [HttpPost("hesapli")]
        public async Task<IActionResult> CreateWithAccount(
    CreateDoctorWithAccountDto dto)
        {
            var kullaniciHesabi = new KullaniciHesabi
            {
                Eposta = dto.Eposta,
                Rol = KullaniciRolu.Doktor
            };

            kullaniciHesabi.ParolaHash =
                _parolaHasher.HashPassword(
                    kullaniciHesabi,
                    dto.Parola);

            var yeniDoktor = new Doctor
            {
                DepartmentId = dto.DepartmentId,
                DoktorAd = dto.DoktorAd,
                DoktorSoyad = dto.DoktorSoyad,
                TelefonNumarasi = dto.TelefonNumarasi,
                UzmanlikAlani = dto.UzmanlikAlani,
                KullaniciHesabi = kullaniciHesabi
            };

            var eklenenDoktor =
                await _doktorServisi
                    .HesabiylaBirlikteEkleAsync(
                        yeniDoktor,
                        kullaniciHesabi);

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