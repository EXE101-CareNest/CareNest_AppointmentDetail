using CareNest_NewService.Domain.Commons.Base;
using CareNest_NewService.Domain.Repositories;
using CareNest_NewService.Infrastructure.Persistences.Database;
using Microsoft.EntityFrameworkCore;

namespace CareNest_NewService.Infrastructure.Persistences.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
    }
}
