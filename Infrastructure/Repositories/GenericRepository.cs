using Application.Interfaces.RepositoryInterfaces;
using Domain.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class, IEntity<TKey>
    {
        private readonly DatabaseContext _realDatabase;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DatabaseContext database)
        {
            _realDatabase = database;
            _dbSet = _realDatabase.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            // Get the key property name using reflection
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (keyProperty == null)
            {
                throw new InvalidOperationException("No key property found for entity type " + typeof(T).Name);
            }

            var keyPropertyName = keyProperty.Name;

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyPropertyName).Equals(id));
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _realDatabase.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _realDatabase.Set<T>().Update(entity);
            await _realDatabase.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _realDatabase.SaveChangesAsync();
            return true;
        }
    }
}
