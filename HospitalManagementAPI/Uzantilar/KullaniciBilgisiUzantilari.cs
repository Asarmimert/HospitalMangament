using System.Security.Claims;

namespace HospitalManagementAPI.Uzantilar
{
    public static class KullaniciBilgisiUzantilari
    {
        public static int? KullaniciIdGetir(
            this ClaimsPrincipal kullanici)
        {
            var deger = kullanici
                .FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (int.TryParse(deger, out var kullaniciId))
            {
                return kullaniciId;
            }

            return null;
        }

        public static string? RolGetir(
            this ClaimsPrincipal kullanici)
        {
            return kullanici
                .FindFirst(ClaimTypes.Role)
                ?.Value;
        }
    }
}