using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.Ayarlar;
using HospitalManagementAPI.DTOs.Yetkiler;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalManagementAPI.Servisler
{
    public class JwtTokenServisi : IJwtTokenServisi
    {
        private readonly JwtAyarlari _jwtAyarlari;

        public JwtTokenServisi(
            IOptions<JwtAyarlari> jwtSecenekleri)
        {
            _jwtAyarlari = jwtSecenekleri.Value;
        }

        public GirisYanitDto TokenOlustur(
            KullaniciHesabi kullanici)
        {
            var simdi = DateTime.UtcNow;

            var bitisTarihi = simdi.AddMinutes(
                _jwtAyarlari.GecerlilikDakikasi);

            var talepler = new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    kullanici.Id.ToString()),

                new(
                    ClaimTypes.Email,
                    kullanici.Eposta),

                new(
                    ClaimTypes.Role,
                    kullanici.Rol.ToString()),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var anahtar = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _jwtAyarlari.Anahtar));

            var imzaBilgisi = new SigningCredentials(
                anahtar,
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtAyarlari.Veren,
                audience: _jwtAyarlari.Hedef,
                claims: talepler,
                notBefore: simdi,
                expires: bitisTarihi,
                signingCredentials: imzaBilgisi);

            var token = new JwtSecurityTokenHandler()
                .WriteToken(jwt);

            return new GirisYanitDto
            {
                KullaniciId = kullanici.Id,
                Eposta = kullanici.Eposta,
                Rol = kullanici.Rol.ToString(),
                Token = token,
                TokenBitisTarihi = bitisTarihi
            };
        }
    }
}