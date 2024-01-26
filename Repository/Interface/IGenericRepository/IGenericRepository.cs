using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> RemoveAsync(T entity);
    }
}
