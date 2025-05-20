using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Shared.Core.DatabaseRetryPolicies.PosgretSQL
{
    public class PostgresConnectionFactory
    {
        private readonly string _connectionString;
        private readonly IRetryPolicy _retryPolicy;

        public PostgresConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;

            _retryPolicy = new DatabaseRetryPolicy(
                retryCount: 5,
                waitBetweenRetriesInMilliseconds: 500,
                transientErrorCodes: new HashSet<int> { 57, 40001, 0, 23505, 53300, 55000, 57014 }
            );
        }


        // Crea y devuelve una conexión con la lógica de reintentos aplicada
        public IDbConnection Create()
        {
            var connection = new NpgsqlConnection(_connectionString);
            var reliableConnection = new ReliableDbConnection(connection, _retryPolicy);
            return reliableConnection;
        }

        // Método adicional para crear el comando con la política de reintentos
        public ReliableDbCommand CreateCommand()
        {
            var connection = new NpgsqlConnection(_connectionString);
            var command = connection.CreateCommand();

            // Crear el ReliableDbCommand, pasando el comando y la política de reintentos
            return new ReliableDbCommand(command, _retryPolicy);
        }
    }
}
