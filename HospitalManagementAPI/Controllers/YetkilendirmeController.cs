using HospitalManagement.Business.Soyut;
using HospitalManagement.Entity.Entities;
using HospitalManagement.Entity.Enums;
using HospitalManagementAPI.DTOs.Yetkiler;
using HospitalManagementAPI.Servisler;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagementAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class YetkilendirmeController : ControllerBase
    {
        private readonly IKullaniciHesabiServisi
            _kullaniciHesabiServisi;

        private readonly IPasswordHasher<KullaniciHesabi>
            _parolaHasher;

        private readonly IJwtTokenServisi
            _jwtTokenServisi;

        public YetkilendirmeController(
            IKullaniciHesabiServisi kullaniciHesabiServisi,
            IPasswordHasher<KullaniciHesabi> parolaHasher,
            IJwtTokenServisi jwtTokenServisi)
        {
            _kullaniciHesabiServisi =
                kullaniciHesabiServisi;
            _parolaHasher = parolaHasher;
            _jwtTokenServisi = jwtTokenServisi;
        }

        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit(
            KullaniciKayitDto dto)
        {
            var kullanici = new KullaniciHesabi
            {
                Eposta = dto.Eposta,
                Rol = KullaniciRolu.Hasta,
                AktifMi = true
            };

            kullanici.ParolaHash =
                _parolaHasher.HashPassword(
                    kullanici,
                    dto.Parola);

            await _kullaniciHesabiServisi
                .EkleAsync(kullanici);

            return Created(
                string.Empty,
                new
                {
                    kullanici.Id,
                    kullanici.Eposta,
                    Rol = kullanici.Rol.ToString()
                });
        }

        [HttpPost("giris")]
        public async Task<IActionResult> Giris(
            KullaniciGirisDto dto)
        {
            var kullanici =
                await _kullaniciHesabiServisi
                    .EpostaIleGetirAsync(dto.Eposta);

            if (kullanici is null || !kullanici.AktifMi)
            {
                return Unauthorized(
                    "E-posta veya parola hatalı.");
            }

            PasswordVerificationResult parolaSonucu;

            try
            {
                parolaSonucu =
                    _parolaHasher.VerifyHashedPassword(
                        kullanici,
                        kullanici.ParolaHash,
                        dto.Parola);
            }
            catch
            {
                return Unauthorized(
                    "E-posta veya parola hatalı.");
            }

            if (parolaSonucu ==
                PasswordVerificationResult.Failed)
            {
                return Unauthorized(
                    "E-posta veya parola hatalı.");
            }

            var cevap =
                _jwtTokenServisi.TokenOlustur(kullanici);

            return Ok(cevap);
        }
    }
}