using Microsoft.EntityFrameworkCore;
using Shared.Core.Interfaces;

namespace Shared.Core
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        protected readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public Task BeginAsync() => _context.Database.BeginTransactionAsync();

        public Task CommitAsync() => _context.Database.CommitTransactionAsync();

        public Task RollbackAsync() => _context.Database.RollbackTransactionAsync();
    }
}
