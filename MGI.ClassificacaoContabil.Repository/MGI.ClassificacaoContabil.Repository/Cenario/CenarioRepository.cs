using Infra.Data;
using Infra.Interface;
using Service.DTO.Filtros;
using Service.DTO.Cenario;
using Service.Repository.Cenario;

using Dapper;

namespace Repository.Cenario
{
    public class CenarioRepository : ICenarioRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSession _session;

        public CenarioRepository(IUnitOfWork unitOfWork, DbSession session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
        }
        public async Task<bool> InserirCenario(CenarioDTO cenario)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into cenario_classif_contabil (nome, status, uscriacao, dtcriacao) 
                                                                  values (:nome, :status, :uscriacao, sysdate)",
            new
            {
                nome = cenario.Nome,
                status = cenario.Status,
                uscriacao = cenario.Usuario?.UsuarioCriacao
            });

            return result == 1;
        }
        public async Task<bool> AlterarCenario(CenarioDTO cenario)
        {
            int result = await _session.Connection.ExecuteAsync(@"update cenario_classif_contabil 
                                                                     set nome = nvl(:nome,nome),  
                                                                         status = nvl(:status,status), 
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_cenario = :idcenario",
            new
            {
                idcenario = cenario.IdCenario,
                nome = cenario.Nome,
                status = cenario.Status,
                usalteracao = cenario.Usuario?.UsuarioModificacao
            });

            return result == 1;
        }
        public async Task<IEnumerable<CenarioDTO>> ConsultarCenario()
        {
            
            var resultado = await _session.Connection.QueryAsync<CenarioDTO>($@"
                                           select 
                                                id_cenario          as IdCenario,
                                                nome                as Nome,
                                                status              as Status,
                                                dtcriacao           as DataCriacao,
                                                uscriacao           as UsuarioCriacao,
                                                dtalteracao         as DataModificacao,
                                                usalteracao         as UsuarioModificacao
                                            from cenario_classif_contabil
                                            where 1 = 1
                                              and status = 'A'");
            return resultado;
        }
        public async Task<IEnumerable<CenarioDTO>> ConsultarCenario(CenarioFiltro filtro)
        {
            var parametros = string.Empty;
            if (filtro.IdCenario > 0)
            {
                parametros += $" and id_cenario = :idcenario";
            }
            if (!string.IsNullOrEmpty(filtro.Nome))
            {
                parametros += " and upper(nome) like upper(:nome)";
            }
            if (!string.IsNullOrEmpty(filtro.Status))
            {
                parametros += $" and status = :status";
            }

            var resultado = await _session.Connection.QueryAsync<CenarioDTO>($@"
                                           select 
                                                id_cenario          as IdCenario,
                                                nome                as Nome,
                                                status              as Status,
                                                dtcriacao           as DataCriacao,
                                                uscriacao           as UsuarioCriacao,
                                                dtalteracao         as DataModificacao,
                                                usalteracao         as UsuarioModificacao
                                            from cenario_classif_contabil
                                            where 1 = 1
                                           {parametros}
                                        ", new
            {
                idcenario = filtro.IdCenario,
                nome = $"%{filtro.Nome}%",
                status = filtro.Status
            });
            return resultado;
        }
    }
}
