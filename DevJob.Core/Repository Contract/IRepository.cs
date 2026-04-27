using System.Collections.Generic;
using System.Linq.Expressions;

namespace DevJob.Application.ServiceContract
{
    public interface IRepository<T> where T: class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllAsyncASQuarable();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> del);
        Task<T?> GetById(int id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        IQueryable<T> Where(Expression<Func<T,bool>> del);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> del);
    }
}
