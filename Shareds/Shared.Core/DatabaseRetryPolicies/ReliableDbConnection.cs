using System.Data;
using System.Data.Common;

namespace Shared.Core.DatabaseRetryPolicies
{
    public class ReliableDbConnection : DbConnection
    {
        private readonly DbConnection _underlyingConnection;
        private readonly IRetryPolicy _retryPolicy;

        private string _connectionString;

        public ReliableDbConnection(
            DbConnection underlyingConnection,
            IRetryPolicy retryPolicy)
        {
            _underlyingConnection = underlyingConnection ?? throw new ArgumentNullException(nameof(underlyingConnection));
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
            _connectionString = _underlyingConnection.ConnectionString;
        }

        public override string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
                _underlyingConnection.ConnectionString = value;
            }
        }

        public override string Database => _underlyingConnection.Database;

        public override string DataSource => _underlyingConnection.DataSource;

        public override string ServerVersion => _underlyingConnection.ServerVersion;

        public override ConnectionState State => _underlyingConnection.State;

        public override void ChangeDatabase(string databaseName)
        {
            _underlyingConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            _underlyingConnection.Close();
        }

        public override void Open()
        {
            if (_underlyingConnection.State == ConnectionState.Open)
            {
                return; // Evita intentar abrir una conexión ya abierta.
            }

            _retryPolicy.Execute(() =>
            {
                _underlyingConnection.Open();
            });
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _underlyingConnection.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return _underlyingConnection.CreateCommand();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_underlyingConnection.State == ConnectionState.Open)
                {
                    _underlyingConnection.Close();
                }

                _underlyingConnection.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
