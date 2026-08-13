using HospitalManagement.Entity.Entities;

namespace HospitalManagement.Business.Soyut
{
    public interface IDepartmanServisi
    {
        Task<List<Department>> TumunuGetirAsync();

        Task<Department?> IdIleGetirAsync(int id);

        Task<Department> EkleAsync(Department departman);
        //işlem tamamlanınca department nesnesi dönecek
        //Metoda ekleneek departmanı verir.Parametre türü bizim tanımladığımız ad.
        Task<bool> GuncelleAsync(Department departman);

        Task<bool> SilAsync(int id);



    }
}