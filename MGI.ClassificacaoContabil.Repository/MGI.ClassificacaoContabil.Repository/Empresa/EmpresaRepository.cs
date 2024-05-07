using Infra.Data;
using Service.DTO.Empresa;
using Service.Repository.Empresa;

using Dapper;

namespace Repository.Empresa
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly DbSession _session;
        public EmpresaRepository(DbSession session)
        {
            _session = session;
        }
        public async Task<IEnumerable<EmpresaDTO>> ConsultarEmpresa()
        {
            return await _session.Connection.QueryAsync<EmpresaDTO>(@"
                               select distinct ltrim(rtrim(a.empnomfan)) as Nome, 
                               a.empcod as IdEmpresa 
                               from corpora.empres a
                               where empsit = 'A'
                               order by 1
                               ");
        }
    }
}
