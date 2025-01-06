using Infra.Data;
using Service.DTO.Empresa;
using Service.Repository.Empresa;

using Dapper;
using Service.DTO.Combos;

namespace Repository.Empresa
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly DbSession _session;
        public EmpresaRepository(DbSession session)
        {
            _session = session;
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarEmpresa()
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(@"
                               select distinct ltrim(rtrim(a.empnomfan)) as Descricao, 
                               a.empcod as Id
                               from corpora.empres a
                               where empsit = 'A'
                               order by 1
                               ");
        }
    }
}
