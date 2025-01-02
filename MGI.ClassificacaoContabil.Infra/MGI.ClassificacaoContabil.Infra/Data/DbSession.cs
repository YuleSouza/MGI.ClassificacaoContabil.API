using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infra.Data
{
    public sealed class DbSession : IDisposable
    {
        public OracleConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }
        public DbSession(string stringConexao)
        {
            if (string.IsNullOrEmpty(stringConexao))
                throw new Exception("Sem conexão com a base de dados");
            Connection = new OracleConnection(stringConexao);
            Connection.Open();
        }
        public void Dispose() => Connection?.Dispose();
    }
}
