
namespace Shared.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveAsync();
    }
}
