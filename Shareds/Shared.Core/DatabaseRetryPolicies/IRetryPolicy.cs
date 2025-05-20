namespace Shared.Core.DatabaseRetryPolicies
{
    public interface IRetryPolicy
    {
        void Execute(Action operation);

        TResult Execute<TResult>(Func<TResult> operation);

        Task Execute(Func<Task> operation, CancellationToken cancellationToken);

        Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);
    }
}
