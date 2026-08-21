using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.DTOs.Sekreterler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SekreterController : ControllerBase
    {
        private readonly ISekreterServisi
            _sekreterServisi;

        private readonly IPasswordHasher<KullaniciHesabi>
            _parolaHasher;

        public SekreterController(
            ISekreterServisi sekreterServisi,
            IPasswordHasher<KullaniciHesabi>
                parolaHasher)
        {
            _sekreterServisi = sekreterServisi;
            _parolaHasher = parolaHasher;
        }

        [Authorize(
            Roles = nameof(KullaniciRolu.Sekreter))]
        [HttpPost("hesapli")]
        public async Task<IActionResult>
            HesabiylaBirlikteOlustur(
                SekreterHesapliOlusturmaDto dto)
        {
            var kullaniciHesabi =
                new KullaniciHesabi
                {
                    Eposta =
                        dto.Eposta?.Trim() ??
                        string.Empty,

                    Rol = KullaniciRolu.Sekreter
                };

            kullaniciHesabi.ParolaHash =
                _parolaHasher.HashPassword(
                    kullaniciHesabi,
                    dto.Parola ?? string.Empty);

            var yeniSekreter = new Sekreter
            {
                Ad =
                    dto.Ad?.Trim() ??
                    string.Empty,

                Soyad =
                    dto.Soyad?.Trim() ??
                    string.Empty,

                TelefonNumarasi =
                    dto.TelefonNumarasi?.Trim() ??
                    string.Empty,

                KullaniciHesabi =
                    kullaniciHesabi
            };

            var eklenenSekreter =
                await _sekreterServisi
                    .HesabiylaBirlikteEkleAsync(
                        yeniSekreter,
                        kullaniciHesabi);

            return Created(
                $"/api/Sekreter/{eklenenSekreter.Id}",
                new
                {
                    SekreterId =
                        eklenenSekreter.Id,

                    KullaniciHesabiId =
                        kullaniciHesabi.Id,

                    Eposta =
                        kullaniciHesabi.Eposta,

                    Ad =
                        eklenenSekreter.Ad,

                    Soyad =
                        eklenenSekreter.Soyad,

                    TelefonNumarasi =
                        eklenenSekreter.TelefonNumarasi,

                    Rol =
                        kullaniciHesabi.Rol.ToString()
                });
        }
    }
}