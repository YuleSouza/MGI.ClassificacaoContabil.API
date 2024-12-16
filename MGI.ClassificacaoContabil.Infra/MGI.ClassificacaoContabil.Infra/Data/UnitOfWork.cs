using Infra.Interface;
using System.Data;

namespace Infra.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbSession _session;

        public UnitOfWork(DbSession session)
        {
            _session = session;
            AlterSession();
        }

        private void AlterSession()
        {
            var command = _session.Connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "alter session set nls_date_format = 'DD/MM/YYYY HH24:MI:SS'";
            command.ExecuteNonQuery();
        }

        public void BeginTransaction()
        {
            _session.Transaction = CreateTransaction();
        }

        public void Commit()
        {
            _session.Transaction.Commit();
        }

        public void Rollback()
        {
            _session.Transaction.Rollback();
        }

        private IDbTransaction CreateTransaction()
        {
            return _session.Connection.BeginTransaction();
        }

        public void Dispose() => _session.Transaction?.Dispose();
    }
}
