using Infra.Data;
using Infra.Interface;
using Service.DTO.Parametrizacao;
using Service.Repository.Parametrizacao;

using Dapper;

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

        #region [Parametrização Cenario]
        public async Task<bool> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_cenario (id_classificacao_contabil, id_classificacao_esg, id_cenario_classif_contabil, status, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaocontabil, :idclassificacaoesg, :idcenarioclassificacao, :status, :uscriacao, sysdate)",
            new
            {
                idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
                idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
                idcenarioclassificacao = parametrizacao.IdCenarioClassificacaoContabil,
                status = parametrizacao.Status,
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
                                                                         status  = :status,
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_parametrizacao_cenario = :idparametrizacaocenario",
           new
           {
               idparametrizacaocenario = parametrizacao.IdParametrizacaoCenario,
               idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               idcenarioclassificacao = parametrizacao.IdCenarioClassificacaoContabil,
               status = parametrizacao.Status,
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
        #endregion

        #region [Classificacao Geral]
        public async Task<bool> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_esg_geral (id_classificacao_esg, id_grupo_programa, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaoesg, :idgrupoprograma, :uscriacao, sysdate)",
            new
            {
                idgrupoprograma = parametrizacao.IdGrupoPrograma,
                idclassificacaoesg = parametrizacao.IdParametrizacaoEsgGeral,
                uscriacao = parametrizacao.Usuario?.UsuarioCriacao
            });

            return result == 1;
        }
        public async Task<bool> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update parametrizacao_esg_geral 
                                                                     set id_grupo_programa = :idgrupoprograma,  
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_param_esg_geral = :idparamesggeral",
           new
           {
               idparamesggeral = parametrizacao.IdParametrizacaoEsgGeral,
               idgrupoprograma = parametrizacao.IdGrupoPrograma,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoClassificacaoGeralDTO>> ConsultarParametrizacaoClassificacaoGeral()
        {

            var resultado = await _session.Connection.QueryAsync<ParametrizacaoClassificacaoGeralDTO>($@"
                                           select 
                                                id_param_esg_geral              as IdParametrizacaoEsgGeral,
                                                id_grupo_programa               as IdGrupoPrograma,
                                                dtcriacao                       as DataCriacao,
                                                uscriacao                       as UsuarioCriacao,
                                                dtalteracao                     as DataModificacao,
                                                usalteracao                     as UsuarioModificacao
                                            from parametrizacao_esg_geral
                                            where 1 = 1");
            return resultado;
        }
        #endregion

        #region [Classificacao ESG]
        public async Task<bool> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_esg_exc (id_cenario_classif_contabil, id_empresa, id_grupo_programa, id_programa, id_projeto, id_classificacao_esg, uscriacao, dtcriacao) 
                                                                  values (:idparametrizacaocenario, :idempresa, :idgrupoprograma, :idprograma, :idprojeto, :idclassificacaoesg, :uscriacao, sysdate)",
            new
            {
                idparametrizacaocenario = parametrizacao.IdCenarioClassificacaoContabil,
                idempresa = parametrizacao.IdEmpresa,
                idgrupoprograma = parametrizacao.IdGrupoPrograma,
                idprograma = parametrizacao.IdPrograma,
                idprojeto = parametrizacao.IdProjeto,
                idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
                uscriacao = parametrizacao.Usuario?.UsuarioCriacao
            });

            return result == 1;
        }
        public async Task<bool> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update parametrizacao_esg_exc 
                                                                     set id_cenario_classif_contabil   = :idparametrizacaocenario,  
                                                                         id_empresa                    = :idempresa,
                                                                         id_grupo_programa             = :idgrupoprograma,
                                                                         id_programa                   = :idprograma,
                                                                         id_projeto                    = :idprojeto,
                                                                         id_classificacao_esg          = :idclassificacaoesg,                                                               
                                                                         usalteracao                   = :usalteracao, 
                                                                         dtalteracao                   = sysdate
                                                                   where id_param_esg_exc              = :idparamesgexc",
           new
           {
               idparamesgexc = parametrizacao.IdParametrizacaoEsgExc,
               idparametrizacaocenario = parametrizacao.IdCenarioClassificacaoContabil,
               idempresa = parametrizacao.IdEmpresa,
               idgrupoprograma = parametrizacao.IdGrupoPrograma,
               idprograma = parametrizacao.IdPrograma,
               idprojeto = parametrizacao.IdProjeto,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoClassificacaoEsgDTO>> ConsultarParametrizacaoClassificacaoExcecao()
        {

            var resultado = await _session.Connection.QueryAsync<ParametrizacaoClassificacaoEsgDTO>($@"
                                           select 
                                                id_param_esg_exc                as IdParametrizacaoEsgExc,
                                                id_cenario_classif_contabil     as IdCenarioClassificacaoContabil,
                                                id_grupo_programa               as IdGrupoPrograma,
                                                id_programa                     as IdPrograma,
                                                id_projeto                      as IdProjeto,
                                                id_classificacao_esg            as IdClassificacaoEsg,                                     
                                                dtcriacao                       as DataCriacao,
                                                uscriacao                       as UsuarioCriacao,
                                                dtalteracao                     as DataModificacao,
                                                usalteracao                     as UsuarioModificacao
                                            from parametrizacao_esg_exc
                                            where 1 = 1");
            return resultado;
        }
        #endregion
    }
}
