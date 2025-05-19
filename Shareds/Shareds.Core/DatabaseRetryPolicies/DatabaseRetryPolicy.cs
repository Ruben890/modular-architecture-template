using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Data.Common;
using Serilog;

namespace Shareds.Core.DatabaseRetryPolicies
{
    public class DatabaseRetryPolicy : IRetryPolicy
    {
        private const int DefaultRetryCount = 3;
        private const int DefaultWaitBetweenRetriesInMilliseconds = 1000;
        private const int DefaultCircuitBreakerFailuresThreshold = 5; // Número de fallos antes de abrir el disyuntor
        private const int DefaultCircuitBreakerDurationInSeconds = 30; // Duración del estado abierto del disyuntor

        private readonly int _retryCount;
        private readonly int _waitBetweenRetriesInMilliseconds;
        private readonly HashSet<int> _transientErrorCodes;
        private readonly AsyncRetryPolicy _retryPolicyAsync;
        private readonly Policy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicyAsync;
        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;

        public DatabaseRetryPolicy(
            int retryCount = DefaultRetryCount,
            int waitBetweenRetriesInMilliseconds = DefaultWaitBetweenRetriesInMilliseconds,
            int circuitBreakerFailuresThreshold = DefaultCircuitBreakerFailuresThreshold,
            int circuitBreakerDurationInSeconds = DefaultCircuitBreakerDurationInSeconds,
            HashSet<int>? transientErrorCodes = null)
        {
            _retryCount = retryCount;
            _waitBetweenRetriesInMilliseconds = waitBetweenRetriesInMilliseconds;
            _transientErrorCodes = transientErrorCodes ?? new HashSet<int>();

            _circuitBreakerPolicyAsync = Policy
                .Handle<DbException>(exception => ShouldRetry(exception))
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: circuitBreakerFailuresThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(circuitBreakerDurationInSeconds),
                    onBreak: (exception, timespan) =>
                    {
                        Log.Error($"Circuit breaker opened due to {exception}. Waiting {timespan} before retrying.");
                    },
                    onReset: () =>
                    {
                        Log.Information("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        Log.Warning("Circuit breaker is half-open. Testing connection.");
                    });

            _circuitBreakerPolicy = Policy
                .Handle<DbException>(exception => ShouldRetry(exception))
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: circuitBreakerFailuresThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(circuitBreakerDurationInSeconds),
                    onBreak: (exception, timespan) =>
                    {
                        Log.Error($"Circuit breaker opened due to {exception}. Waiting {timespan} before retrying.");
                    },
                    onReset: () =>
                    {
                        Log.Information("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        Log.Warning("Circuit breaker is half-open. Testing connection.");
                    });

            _retryPolicyAsync = Policy
                .Handle<DbException>(exception => ShouldRetry(exception))
                .WaitAndRetryAsync(
                    retryCount: _retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * _waitBetweenRetriesInMilliseconds),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning($"Retry {retryCount} due to {exception}. Waiting {timeSpan}.");
                    }
                );

            _retryPolicy = Policy
                .Handle<DbException>(exception => ShouldRetry(exception))
                .WaitAndRetry(
                    retryCount: _retryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * _waitBetweenRetriesInMilliseconds),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Log.Warning($"Retry {retryCount} due to {exception}. Waiting {timeSpan}.");
                    }
                );
        }

        private bool ShouldRetry(DbException exception)
        {
            if (int.TryParse(exception.ErrorCode.ToString(), out int errorCode))
            {
                return _transientErrorCodes.Contains(errorCode);
            }

            return false;
        }

        public void Execute(Action operation)
        {
            _circuitBreakerPolicy.Execute(() =>
            {
                _retryPolicy.Execute(() =>
                {
                    operation.Invoke();
                });
            });
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _circuitBreakerPolicy.Execute(() =>
            {
                return _retryPolicy.Execute(() =>
                {
                    return operation.Invoke();
                });
            });
        }

        public async Task Execute(Func<Task> operation, CancellationToken cancellationToken)
        {
            await _circuitBreakerPolicyAsync.ExecuteAsync(async () =>
            {
                await _retryPolicyAsync.ExecuteAsync(async () =>
                {
                    await operation.Invoke();
                });
            });
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await _circuitBreakerPolicyAsync.ExecuteAsync(async () =>
            {
                return await _retryPolicyAsync.ExecuteAsync(async () =>
                {
                    return await operation.Invoke();
                });
            });
        }
    }
}
