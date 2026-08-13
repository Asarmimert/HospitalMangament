using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.DTOs.Yetkiler;

namespace HospitalManagementAPI.Servisler
{
    public interface IJwtTokenServisi
    {
        GirisYanitDto TokenOlustur(
            KullaniciHesabi kullanici);
    }
}