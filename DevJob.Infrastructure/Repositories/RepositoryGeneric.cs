using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace DevJob.Infrastructure.Repositories
{
    public class RepositoryGeneric<T> : IRepository<T> where T :class
    {
        protected readonly AppDbContext context;
        protected readonly DbSet<T> dbSet;
        public RepositoryGeneric (AppDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }
        public async Task AddAsync(T entity) =>
            await  dbSet.AddAsync(entity);
        public async Task AddRangeAsync(IEnumerable<T> entities) =>
        await dbSet.AddRangeAsync(entities);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> del) =>
            await dbSet.AnyAsync(del);
       
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> del) =>
            await dbSet.FirstOrDefaultAsync(del);
     
        public async Task<IEnumerable<T>> GetAllAsync() =>
          await dbSet.ToListAsync();

        public  IQueryable<T> GetAllAsyncASQuarable() =>
             dbSet.AsQueryable();
        
        public async Task<T?> GetById(int id) =>
         await dbSet.FindAsync(id);
        public void Remove(T entity) =>
            dbSet.Remove(entity);
        public void Update(T entity) =>
            dbSet.Update(entity);
        public IQueryable<T> Where(Expression<Func<T, bool>> del) =>
        dbSet.Where(del);
    }
}
