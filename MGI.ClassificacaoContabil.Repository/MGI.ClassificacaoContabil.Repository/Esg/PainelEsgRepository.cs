using Dapper;
using Infra.Data;
using Service.DTO.Combos;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.Enum;
using Service.Repository.Esg;
using System.Text;

namespace Repository.PainelEsg
{
    public class PainelEsgRepository : IPainelEsgRepository
    {
        private readonly DbSession _session;

        public PainelEsgRepository(DbSession session)
        {
            _session = session;
        }

        #region [ Consultas ]
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarCalssifInvestimento()
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(@$"select pgmtipcod as Id, pgmtipnom as Descricao from PGMTIP where pgmtipsit = 'A'");
        }
        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {
            #region [ Filtros ]
            StringBuilder parametros = new StringBuilder();
            parametros.Append(" and sub.DtLancamentoProjeto between :datainicial and :datafinal");
            if (filtro.IdEmpresa >= 0)
            {
                parametros.Append(" and sub.IdEmpresa = :IdEmpresa");
            }
            if (filtro.IdProjeto > 0)
            {
                parametros.Append(" and sub.IdProjeto = :idProjeto");
            }
            if (!string.IsNullOrEmpty(filtro.IdGrupoPrograma))
            {
                parametros.Append(" and sub.IdGrupoPrograma = :idgrupoprogrma ");
            }
            if (!string.IsNullOrEmpty(filtro.IdGerencia))
            {
                parametros.Append(" and sub.IdGerencia = :idgerencia ");
            }            
            if (!string.IsNullOrEmpty(filtro.IdDiretoria))
            {
                parametros.Append(" and TRIM(sub.IdDiretoria) = :iddiretoria ");
            }
            if (!string.IsNullOrEmpty(filtro.StatusProjeto))
            {
                parametros.Append(" and sub.IdStatusProjeto = :idstatusprojeto");
            }
            if (!string.IsNullOrEmpty(filtro.ClassificacaoInvestimento))
            {
                parametros.Append(" and sub.ClassifInvestimento = :classifinvestimento");
            }
            #endregion
            return await _session.Connection.QueryAsync<ProjetoEsgDTO>($@"
                  select sub.IdProjeto
                        ,sub.Nomeprojeto
                        ,sub.IdEmpresa
                        ,sub.NomeEmpresa
                        ,sub.IdGestor
                        ,sub.IdStatusProjeto
                        ,sub.DescricaoStatusProjeto
                        ,trim(sub.Patrocinador)          as NomePatrocinador
                        ,sub.ClassifInvestimento         as ClassifInvestimento
                        ,sum(nvl(sub.TotalLancamento,0)) as ValorOrcamento
                        ,'{filtro.StatusAprovacao}'        as StatusAprovacao
                   from (
                 select p.prjcod                                           as IdProjeto
                        , trim(p.prjnom)                                   as NomeProjeto
                        , e.empcod                                         as IdEmpresa
                        , trim(e.empnomfan)                                as NomeEmpresa        
                        , LTRIM(RTRIM(U.USUNOM))                           as IdGestor
                        , usu.usunom                                       as Patrocinador
                        , st.prjstacod                                     as IdStatusProjeto
                        , trim(st.prjstades)                               as DescricaoStatusProjeto
                        , p.prjpgmtip                                      as ClassifInvestimento
                        , orc.prjorctip                                    as TipoValor
                        , to_date('01' || '/' || orc.prjorcmes || '/' || orc.prjorcano) as DtLancamentoProjeto
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = :TipoValor
                              and orc2.prjorcmes > 0
                              and orc2.prjorcano > 0
                              and to_date('01' || '/' || orc2.prjorcmes || '/' || orc2.prjorcano) between :dataInicial and :datafinal) as TotalLancamento
                  from projeto p
                        inner join prjorc orc on (p.prjcod = orc.prjcod 
                                                and orc.prjorcfse = 0
                                                and orc.prjorcver = 0 
                                                and orc.prjorctip in ('O','J','R','2','1','P') 
                                                and orc.prjorcmes > 0 and orc.prjorcano > 0)
                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                        inner join corpora.usuari u on (u.USULOG = p.PRJGES)
                        inner join prjsta st on (st.prjstacod = p.prjsta and prjstasit = 'A')
                        inner join corpora.usuari usu on (p.prjreq = usu.usulog)
                where p.prjsit = 'A'
                   and orc.prjorcano > 2016
                   and p.prjesg = 'S'
                ) sub
                where 1 = 1 {parametros.ToString()}
               group by sub.IdProjeto
                     ,sub.Nomeprojeto
                     ,sub.IdEmpresa
                     ,sub.NomeEmpresa
                     ,sub.IdGestor
                     ,sub.IdStatusProjeto
                     ,sub.DescricaoStatusProjeto
                     ,trim(sub.Patrocinador)
                     ,sub.ClassifInvestimento 
               order by sub.IdProjeto
            ", new
            {
                idEmpresa = filtro.IdEmpresa,
                idGrupoPrograma = filtro.IdGrupoPrograma,
                iddiretoria = filtro.IdDiretoria,
                idGerencia = filtro.IdGerencia,
                datainicial = filtro.DataInicio.ToString("01/MM/yyyy"),
                datafinal = filtro.DataFim.ToString("01/MM/yyyy"),
                TipoValor = filtro.TipoValor,
                idProjeto = filtro.IdProjeto,
                idstatusprojeto = filtro.StatusProjeto,
                classifinvestimento = filtro.ClassificacaoInvestimento
            });
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarComboProjetosEsg(FiltroProjeto filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(filtro.IdEmpresa)) 
            {
                parametros.Append(" AND p.prjempcus IN :codEmpresa");
            }
            return await _session.Connection.QueryAsync<PayloadComboDTO>($@"SELECT concat(concat(lpad(prjcod, 5, '0'),' - '), trim(prjnom)) as Descricao,
                                                                              prjcod as Id
                                                                         FROM servdesk.projeto p, servdesk.pgmass a
                                                                        WHERE p.prjsit = 'A'
                                                                          AND a.pgmassver = 0
                                                                          AND a.pgmasscod = p.pgmasscod
                                                                          {parametros}
                                                                          AND (EXISTS (SELECT 1
                                                                                         FROM servdesk.geradm g
                                                                                        WHERE upper(g.geradmusu) = RPAD(upper(:usuario),20)
                                                                          AND g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                                          AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(upper(:usuario),20) OR upper(p.prjreq) = RPAD(upper(:usuario),20))
                                                                          AND p.prjesg = 'S'
                                                                          AND EXISTS (SELECT 1 FROM prjorc orc WHERE orc.prjcod = p.prjcod)",
            new
            {
                codEmpresa = filtro.IdEmpresa,
                usuario = filtro.Usuario?.ToUpper()
            });
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarStatusProjeto()
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(@$"select prjstacod as Id, trim(prjstades) as Descricao from PRJSTA where prjstasit = 'A' order by prjstaord");
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg()
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(@$"select clecod as Id, clenom as Descricao from CLAESG where clesit = 'A'");
        }
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarSubClassificacaoEsg(int idClassificacao)
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(@$"select clemetcod as Id
                                                                                 , clemetnom as Descricao 
                                                                              from CLAESGMET 
                                                                             where clemetsit = 'A' 
                                                                               and clecod = :clecod",
                new
                {
                    clecod = idClassificacao
                });
        }
        public async Task<JustificativaClassifEsgDTO> ConsultarJustificativaEsgPorId(int id)
        {
            return await _session.Connection.QueryFirstOrDefaultAsync<JustificativaClassifEsgDTO>(@$"select 
                                                                                                        id_justif_classif_esg as IdJustifClassifEsg
                                                                                                        , empcod              as IdEmpresa
                                                                                                        , dat_anomes          as DataClassif
                                                                                                        , j.prjcod              as IdProjeto
                                                                                                        , p.prjnom            as NomeProjeto
                                                                                                        , id_classif          as IdClassif
                                                                                                        , c.clenom            as DescricaoClassif
                                                                                                        , id_sub_classif      as IdSubClassif
                                                                                                        , m.clemetnom         as DescricaoSubClassif
                                                                                                        , justificativa
                                                                                                    from justif_classif_esg j 
                                                                                                            inner join claesg c on (j.id_classif = c.clecod)
                                                                                                            inner join claesgmet m on (c.clecod = m.clecod and m.clemetcod = j.id_sub_classif)
                                                                                                            inner join projeto p on (j.prjcod = p.prjcod)
                                                                                                    where j.id_justif_classif_esg = :id_justif_classif_esg ",
                                                                                                    new
                                                                                                    {
                                                                                                        id_justif_classif_esg = id
                                                                                                    });
        }
        public async Task<IEnumerable<AprovacaoClassifEsg>> ConsultarLogAprovacoesPorId(int id)
        {
            return await _session.Connection.QueryAsync<AprovacaoClassifEsg>(@"select aprovacao             as Aprovacao
                                                                                    , uscriacao             as UsCriacao
                                                                                    , dtcriacao             as DtCriacao
                                                                                    , id_aprovacao          as IdAprovacao 
                                                                                    , id_justif_classif_esg as IdJustifClassifEsg
                                                                                 from aprovacao_justif_classif_esg 
                                                                                where id_justif_classif_esg = :id_justf_classif_esg"
                , new
                {
                    id_justf_classif_esg = id
                });
        }
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            #region [ Filtros ]
            StringBuilder parametros = new StringBuilder();            
            if (!string.IsNullOrEmpty(filtro.StatusAprovacao))
            {
                parametros.Append(" and j.status_aprovacao = :statusAprovacao");
            }            
            if (filtro.IdClassif > 0)
            {
                parametros.Append(" and j.id_classis = :idclassif");
            }
            if (filtro.IdSubClassif > 0) 
            {
                parametros.Append(" and j.id_sub_classis = :idsubclassif");
            }
            #endregion
            return await _session.Connection.QueryAsync<JustificativaClassifEsgDTO>(@$"select 
                                                                                               j.id_justif_classif_esg as IdJustifClassifEsg
                                                                                             , empcod              as IdEmpresa
                                                                                             , dat_anomes          as DataClassif
                                                                                             , j.prjcod              as IdProjeto
                                                                                             , p.prjnom            as NomeProjeto
                                                                                             , id_classif          as IdClassif
                                                                                             , trim(c.clenom)      as DescricaoClassif
                                                                                             , id_sub_classif      as IdSubClassif
                                                                                             , trim(m.clemetnom)   as DescricaoSubClassif
                                                                                             , justificativa       as Justificativa
                                                                                             , j.status_aprovacao  as StatusAprovacao
                                                                                             , decode(j.status_aprovacao,'P','Pendente','A','Aprovado','R','Reprovado','Excluído')  as DescricaoStatusAprovacao
                                                                                             , decode(j.status_aprovacao,'E',1,0) as ClassificacaoBloqueada
                                                                                             , j.uscriacao         as Usuario
                                                                                             , j.perc_kpi          as PercentualKpi
                                                                                         from justif_classif_esg j 
                                                                                                inner join claesg c on (j.id_classif = c.clecod)
                                                                                                inner join claesgmet m on (c.clecod = m.clecod and m.clemetcod = j.id_sub_classif)
                                                                                                inner join projeto p on (j.prjcod = p.prjcod)
                                                                                        where j.prjcod     = :idprojeto 
                                                                                          and j.empcod     = :idempresa
                                                                                          {parametros}",
                                                                                          new
                                                                                          {
                                                                                              idprojeto = filtro.IdProjeto,
                                                                                              idempresa = filtro.IdEmpresa,
                                                                                              idclassif = filtro.IdClassif,
                                                                                              idsubclassif = filtro.IdSubClassif,
                                                                                              statusAprovacao = filtro.StatusAprovacao
                                                                                          });
        }

        #endregion
        public async Task<int> InserirJustificativaEsg(JustificativaClassifEsg justificativa)
        {
            int id = await _session.Connection.QueryFirstOrDefaultAsync<int>(@"select SERVDESK.SEQ_JUSTIF_CLASSIF_ESG.NEXTVAL from dual ");
            int result = await _session.Connection.ExecuteAsync(@"insert into justif_classif_esg (
                                                                        id_justif_classif_esg,
                                                                        empcod, 
                                                                        dat_anomes,
                                                                        prjcod, 
                                                                        id_classif, 
                                                                        id_sub_classif, 
                                                                        justificativa, 
                                                                        uscriacao,
                                                                        status_aprovacao,
                                                                        perc_kpi) 
                                                                        values ( 
                                                                        :id_justif_classif_esg,
                                                                        :empcod, 
                                                                        :datanomes, 
                                                                        :prjcod, 
                                                                        :idclassif, 
                                                                        :idsubclassif, 
                                                                        :justificativa, 
                                                                        :uscriacao,
                                                                        :status_aprovacao,
                                                                        :perc_kpi)",
            new
            {
                id_justif_classif_esg = id,
                empcod = justificativa.IdEmpresa,
                datanomes = justificativa.DataClassif,
                prjcod = justificativa.IdProjeto,
                idclassif = justificativa.IdClassif,
                idsubclassif = justificativa.IdSubClassif,
                justificativa = justificativa.Justificativa,
                uscriacao = justificativa.UsCriacao,
                status_aprovacao = justificativa.StatusAprovacao,
                perc_kpi = justificativa.PercentualKpi
            });
            return id;
        }
        public async Task<bool> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            int result = await _session.Connection.ExecuteAsync(@"update justif_classif_esg 
                                                                     set justificativa = :justificativa, 
                                                                         usalteracao = :usalteracao,
                                                                         dtalteracao = sysdate,
                                                                         perc_kpi = :percentualKpi
                                                                   where id_justif_classif_esg = :id_justif_classif_esg",
            new
            {
    
                justificativa = justificativa.Justificativa,
                usalteracao = justificativa.UsAlteracao,
                id_justif_classif_esg = justificativa.IdJustifClassifEsg,
                percentualKpi = justificativa.PercentualKpi,
            });
            return result == 1;
        }
        public async Task<bool> AlterarStatusJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            int result = await _session.Connection.ExecuteAsync(@"update justif_classif_esg
                                                                     set status_aprovacao = :status_aprovacao
                                                                   where id_justif_classif_esg = :id_justif_classif_esg",
            new
            {
                status_aprovacao = justificativa.StatusAprovacao,
                id_justif_classif_esg = justificativa.IdJustifClassifEsg
            });
            return result == 1;
        }
        public async Task<bool> InserirAprovacao(AprovacaoClassifEsg aprovacaoClassifEsg)
        {
            int result = await _session.Connection.ExecuteAsync(@"
                                insert into aprovacao_justif_classif_esg (
                                    id_justif_classif_esg,
                                    aprovacao,
                                    uscriacao
                                ) values (
                                    :id_justif_classif_esg,
                                    :aprovacao,                                        
                                    :uscriacao
                                )
                            ", new
            {
                id_justif_classif_esg = aprovacaoClassifEsg.IdJustifClassifEsg,
                aprovacao = aprovacaoClassifEsg.Aprovacao,
                uscriacao = aprovacaoClassifEsg.UsCriacao
            });
            return result > 0;
        }
        public async Task<bool> RemoverClassificacao(int id)
        {
            int result = await _session.Connection.ExecuteAsync(@"update justif_classif_esg
                                                                     set status_aprovacao = 'E'
                                                                   where id_justif_classif_esg = :id_justif_classif_esg",
            new
            {
                id_justif_classif_esg = id
            });
            return result == 1;
        }
    }
}
