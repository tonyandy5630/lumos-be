using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repository.Interface.IGenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly LumosDBContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(LumosDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await Task.FromResult(entity);
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> RemoveAsync(T entity)
        {
                
            return  await Task.FromResult(_dbSet.Remove(entity) != null);
        }

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            //Then set the state of the Entity as Modified
            _context.Entry(entity).State = EntityState.Modified;
            return await Task.FromResult(await _context.SaveChangesAsync() == 1);
        }
    }
}
