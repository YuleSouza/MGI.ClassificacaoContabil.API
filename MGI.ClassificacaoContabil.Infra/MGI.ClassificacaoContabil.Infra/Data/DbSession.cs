using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infra.Data
{
    public sealed class DbSession : IDisposable
    {
        public OracleConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }

        public DbSession(IConfiguration config)
        {
            Connection = new OracleConnection(config.GetSection("connectionString").Value);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
