using Dapper;
using Infra.Data;
using Infra.Interface;
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

        #region [Parametrização Cenario]
        public async Task<bool> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_cenario (id_classificacao_contabil, id_classificacao_esg, id_cenario, status, uscriacao, dtcriacao) 
                                                                  values (:idclassificacaocontabil, :idclassificacaoesg, :idcenarioclassificacao, :status, :uscriacao, sysdate)",
            new
            {
                idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
                idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
                idcenarioclassificacao = parametrizacao.IdCenario,
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
                                                                         id_cenario = :idcenarioclassificacao,
                                                                         status  = :status,
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_parametrizacao_cenario = :idparametrizacaocenario",
           new
           {
               idparametrizacaocenario = parametrizacao.IdParametrizacaoCenario,
               idclassificacaocontabil = parametrizacao.IdClassificacaoContabil,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               idcenarioclassificacao = parametrizacao.IdCenario,
               status = parametrizacao.Status,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoCenarioDTO>> ConsultarParametrizacaoCenario()
        {

            var resultado = await _session.Connection.QueryAsync<ParametrizacaoCenarioDTO>($@"
                                           select 
                                                p.id_parametrizacao_cenario       as IdParametrizacaoCenario,
                                                p.id_classificacao_contabil       as IdClassificacaoContabil,
                                                p.id_classificacao_esg            as IdClassificacaoEsg,
                                                p.id_cenario                      as IdCenario,
                                                p.dtcriacao                       as DataCriacao,
                                                p.uscriacao                       as UsuarioCriacao,
                                                p.dtalteracao                     as DataModificacao,
                                                p.usalteracao                     as UsuarioModificacao,
                                                c.nome                            as NomeCenario,
                                                e.nome                            as NomeClassifEsg,
                                                c.cconom                          as NomeClassifContabil,
                                                p.Status                          as Status
                                            from parametrizacao_cenario p
                                                    inner join cenario_classif_contabil c on p.id_cenario = c.id_cenario
                                                    inner join classificacao_esg e on (p.id_classificacao_esg = e.id_classificacao_esg)
                                                    left join clacon c on (p.id_classificacao_contabil = c.ccocod)
                                            where 1 = 1
                                            order by p.status, c.nome");
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
                idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
                uscriacao = parametrizacao.Usuario?.UsuarioCriacao
            });

            return result == 1;
        }
        public async Task<bool> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"update parametrizacao_esg_geral 
                                                                     set id_classificacao_esg = :idclassificacaoesg,
                                                                         id_grupo_programa = :idgrupoprograma,  
                                                                         usalteracao = :usalteracao, 
                                                                         dtalteracao = sysdate
                                                                   where id_param_esg_geral = :idparamesggeral",
           new
           {
               idparamesggeral = parametrizacao.IdParametrizacaoEsgGeral,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               idgrupoprograma = parametrizacao.IdGrupoPrograma,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoClassificacaoGeralDTO>> ConsultarParametrizacaoClassificacaoGeral()
        {

            var resultado = await _session.Connection.QueryAsync<ParametrizacaoClassificacaoGeralDTO>($@"
                                           select pg.id_param_esg_geral          as IdParametrizacaoEsgGeral,
                                                  pg.id_classificacao_esg        as IdClassificacaoEsg,
                                                  ces.nome                       as NomeClassificacaoEsg,
                                                  pg.id_grupo_programa           as IdGrupoPrograma,
                                                  gp.pgmgrunom                   as NomeGrupoPrograma,
                                                  pg.dtcriacao                   as DataCriacao,
                                                  pg.uscriacao                   as UsuarioCriacao,
                                                  pg.dtalteracao                 as DataModificacao,
                                                  pg.usalteracao                 as UsuarioModificacao
                                             from parametrizacao_esg_geral pg
                                                    join servdesk.pgmgru gp on pg.id_grupo_programa = gp.pgmgrucod
                                                    join servdesk.classificacao_esg ces on pg.id_classificacao_esg = ces.id_classificacao_esg
                                            where 1 = 1
                                              and gp.pgmgrusit = 'A'
                                            order by ces.nome, gp.pgmgrunom");
            return resultado;
        }
        #endregion

        #region [Classificacao ESG]
        public async Task<bool> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into parametrizacao_esg_exc (id_cenario, id_empresa, id_grupo_programa, id_programa, id_projeto, id_classificacao_esg, uscriacao, dtcriacao) 
                                                                  values (:idparametrizacaocenario, :idempresa, :idgrupoprograma, :idprograma, :idprojeto, :idclassificacaoesg, :uscriacao, sysdate)",
            new
            {
                idparametrizacaocenario = parametrizacao.IdCenario,
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
                                                                     set id_cenario                    = :idparametrizacaocenario,  
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
               idparametrizacaocenario = parametrizacao.IdCenario,
               idempresa = parametrizacao.IdEmpresa,
               idgrupoprograma = parametrizacao.IdGrupoPrograma,
               idprograma = parametrizacao.IdPrograma,
               idprojeto = parametrizacao.IdProjeto,
               idclassificacaoesg = parametrizacao.IdClassificacaoEsg,
               usalteracao = parametrizacao.Usuario?.UsuarioModificacao
           });

            return result == 1;
        }
        public async Task<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>> ConsultarParametrizacaoClassificacaoExcecao()
        {
            var resultado = await _session.Connection.QueryAsync<ParametrizacaoClassificacaoEsgFiltroDTO>($@"
                                            select distinct
                                                 pe.id_param_esg_exc                as IdParametrizacaoEsgExc,
                                                 pe.id_cenario                      as IdCenario,
                                                 c.nome                             as NomeCenario, 
                                                 pe.id_empresa                      as IdEmpresa,
                                                 ltrim(rtrim(e.empnomfan))          as NomeEmpresa,         
                                                 pe.id_grupo_programa               as IdGrupoPrograma,
                                                 gp.pgmgrunom                       as NomeGrupoPrograma,  
                                                 pe.id_programa                     as IdPrograma,
                                                 p.pgmnom                           as NomePrograma,
                                                 pe.id_projeto                      as IdProjeto,
                                                 to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prj.prjnom)) nomeprojeto,
                                                 pe.id_classificacao_esg            as IdClassificacaoEsg,    
                                                 ces.nome                           as NomeClassificacaoEsg,
                                                 pe.dtcriacao                       as DataCriacao,
                                                 pe.uscriacao                       as UsuarioCriacao,
                                                 pe.dtalteracao                     as DataModificacao,
                                                 pe.usalteracao                     as UsuarioModificacao
                                            from parametrizacao_esg_exc pe
                                                    join corpora.empres e on pe.id_empresa = e.empcod            
                                                    join servdesk.pgmgru gp on pe.id_grupo_programa = gp.pgmgrucod
                                                    join servdesk.pgmass pgp on pgp.pgmgrucod = gp.pgmgrucod 
                                                    join servdesk.pgmpro p on p.pgmcod = pe.id_programa
                                                    join servdesk.cenario_classif_contabil c on pe.id_cenario = c.id_cenario
                                                    join servdesk.classificacao_esg ces on pe.id_classificacao_esg = ces.id_classificacao_esg
                                                    join servdesk.projeto prj on pe.id_programa = prj.prjcod
                                            where 1 = 1
                                            order by ltrim(rtrim(e.empnomfan)), gp.pgmgrunom, p.pgmnom ");
            return resultado;
        }
        
        #endregion
    }
}
