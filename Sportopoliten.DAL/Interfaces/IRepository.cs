using System.Linq.Expressions;

namespace Sportopoliten.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IEnumerable<T>> GetWithQueryAsync(Func<IQueryable<T>, IQueryable<T>> queryOperation);
        Task<T?> GetSingleWithQueryAsync(Func<IQueryable<T>, IQueryable<T>> queryOperation);
    }
}