using Microsoft.EntityFrameworkCore;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Interfaces;
using System.Linq.Expressions;

namespace Sportopoliten.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ShopDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ShopDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate != null)
            {
                return await _dbSet.CountAsync(predicate);
            }
            return await _dbSet.CountAsync();
        }
        public async Task<IEnumerable<T>> GetWithQueryAsync(Func<IQueryable<T>, IQueryable<T>> queryOperation)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            query = queryOperation(query);

            return await query.ToListAsync();
        }

        public async Task<T?> GetSingleWithQueryAsync(Func<IQueryable<T>, IQueryable<T>> queryOperation)
        {
            IQueryable<T> query = _dbSet;

            query = queryOperation(query);

            return await query.FirstOrDefaultAsync();
        }
    }
}