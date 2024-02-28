using Dapper;

using Infra.Data;
using Infra.Interface;

using Service.DTO.Filtros;
using Service.DTO.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Repository.Parametrizacao
{
    public class ParametrizacaoRepository : IParametrizacaoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSession _session;

        public ParametrizacaoRepository(IUnitOfWork unitOfWork, DbSession session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
        }

        public async Task<bool> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_cenario (id_classificacao_contabil, id_classificacao_esg, id_cenario_classif_contabil, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaocontabil, :idclassificacaoesg, :idcenarioclassificacao, :uscriacao, sysdate)",
            new
            {
                idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
                idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
                idcenarioclassificacao = parametrizacao.IdCenarioClassificacaoContabil,
                uscriacao = parametrizacao.Usuario?.UsuarioCriacao
            });

            return result == 1;
        }
        public async Task<bool> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update parametrizacao_cenario 
                                                                     set id_classificacao_contabil = :idclassificacaocontabil,  
                                                                         id_classificacao_esg = :idclassificacaoesg, 
                                                                         id_cenario_classif_contabil = :idcenarioclassificacao,
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_parametrizacao_cenario = :idparametrizacaocenario",
           new
           {
               idparametrizacaocenario = parametrizacao.IdParametrizacaoCenario,
               idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               idcenarioclassificacao = parametrizacao.IdCenarioClassificacaoContabil,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoCenarioDTO>> ConsultarParametrizacaoCenario()
        {

            var resultado = await _session.Connection.QueryAsync<ParametrizacaoCenarioDTO>($@"
                                           select 
                                                id_parametrizacao_cenario       as IdParametrizacaoCenario,
                                                id_classificacao_contabil       as IdClassificacaoContabil,
                                                id_classificacao_esg            as IdClassificacaoEsg,
                                                id_cenario_classif_contabil     as IdCenarioClassificacaoContabil,
                                                dtcriacao                       as DataCriacao,
                                                uscriacao                       as UsuarioCriacao,
                                                dtalteracao                     as DataModificacao,
                                                usalteracao                     as UsuarioModificacao
                                            from parametrizacao_cenario
                                            where 1 = 1");
            return resultado;
        }
    }
}
