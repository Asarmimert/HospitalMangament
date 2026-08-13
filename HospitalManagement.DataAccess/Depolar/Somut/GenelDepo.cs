using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Soyut;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalManagement.DataAccess.Depolar.Somut
{
    public class GenelDepo<T> : IGenelDepo<T>
        where T : class
    {
        private readonly HospitalDbContext _context;
        private readonly DbSet<T> _tablo;

        public GenelDepo(HospitalDbContext context)
        {
            _context = context;
            _tablo = context.Set<T>();
        }

        public async Task<List<T>> TumunuGetirAsync()
        {
            return await _tablo
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<T>> KosulaGoreGetirAsync(
            Expression<Func<T, bool>> kosul)
        {
            return await _tablo
                .AsNoTracking()
                .Where(kosul)
                .ToListAsync();
        }

        public async Task<T?> IdIleGetirAsync(int id)
        {
            return await _tablo.FindAsync(id);
        }

        public async Task<bool> VarMiAsync(
            Expression<Func<T, bool>> kosul)
        {
            return await _tablo.AnyAsync(kosul);
        }

        public async Task EkleAsync(T varlik)
        {
            await _tablo.AddAsync(varlik);
        }

        public void Guncelle(T varlik)
        {
            _tablo.Update(varlik);
        }

        public void Sil(T varlik)
        {
            _tablo.Remove(varlik);
        }

        public async Task<int> KaydetAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}