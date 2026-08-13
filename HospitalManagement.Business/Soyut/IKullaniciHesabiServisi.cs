using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IKullaniciHesabiServisi
    {
        Task<KullaniciHesabi?> EpostaIleGetirAsync(
            string eposta);

        Task<KullaniciHesabi> EkleAsync(
            KullaniciHesabi kullaniciHesabi);
    }
}