using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public void Create(TEntity entity) => Context.Set<TEntity>().Add(entity);
        public void Update(TEntity entity) => Context.Set<TEntity>().Update(entity);
        public void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().RemoveRange(entities);
        public void UpdateRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().UpdateRange(entities);
        public void InsertRange(IEnumerable<TEntity> entities) => Context.Set<TEntity>().AddRange(entities);
    }
}
