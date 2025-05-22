using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shared.Core
{
    public class RepositoryBase<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public RepositoryBase(TContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> GetAll(bool trackChanges) =>
            trackChanges
                ? Context.Set<TEntity>()
                : Context.Set<TEntity>().AsNoTracking();
        public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression, bool trackChanges) =>
            trackChanges
                ? Context.Set<TEntity>().Where(expression)
                : Context.Set<TEntity>().Where(expression).AsNoTracking();
        public async Task Create(TEntity entity) => await Context.Set<TEntity>().AddAsync(entity);
        public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);
        public void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().RemoveRange(entities);
        public void UpdateRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().UpdateRange(entities);
        public async Task CreateRange(IEnumerable<TEntity> entities) => await Context.Set<TEntity>().AddRangeAsync(entities);
    }
}
