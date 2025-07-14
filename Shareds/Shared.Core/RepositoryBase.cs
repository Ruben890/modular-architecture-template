using Microsoft.EntityFrameworkCore;
using Shared.Core.Interfaces;
using System.Linq.Expressions;

namespace Shared.Core
{
    public class RepositoryBase<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext Context;
        protected readonly IUnitOfWork UnitOfWork;

        public RepositoryBase(TContext context, IUnitOfWork unitOfWork)
        {
            Context = context;
            UnitOfWork = unitOfWork;
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

        public Task BeginTransactionAsync() => UnitOfWork.BeginAsync();

        public Task CommitTransactionAsync() => UnitOfWork.CommitAsync();

        public Task RollbackTransactionAsync() => UnitOfWork.RollbackAsync();
        public Task SaveChangesAsync() => UnitOfWork.SaveAsync();
    }
}
