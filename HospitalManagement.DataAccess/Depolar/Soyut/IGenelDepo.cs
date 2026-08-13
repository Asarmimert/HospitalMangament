using System.Linq.Expressions;

namespace HospitalManagement.DataAccess.Depolar.Soyut
{
    public interface IGenelDepo<T>
        where T : class
    {
        Task<List<T>> TumunuGetirAsync();

        Task<List<T>> KosulaGoreGetirAsync(
            Expression<Func<T, bool>> kosul);

        Task<T?> IdIleGetirAsync(int id);

        Task<bool> VarMiAsync(
            Expression<Func<T, bool>> kosul);

        Task EkleAsync(T varlik);

        void Guncelle(T varlik);

        void Sil(T varlik);

        Task<int> KaydetAsync();
    }
}