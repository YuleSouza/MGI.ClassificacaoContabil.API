using Dapper;
using Infra.Data;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;
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

        public async Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento()
        {
            return await _session.Connection.QueryAsync<CLassifInvestimentoDTO>(@$"select pgmtipcod as IdClassifInvestimento, pgmtipnom as Descricao from PGMTIP where pgmtipsit = 'A'");
        }
        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro)
        {
            #region [ Filtros ]
            StringBuilder parametros = new StringBuilder();
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
                // TO-DO - definir regra
                parametros.Append(" and 2 = 2");
            }
            if (!string.IsNullOrEmpty(filtro.StatusAprovacao))
            {
                // TO-DO - definir regra
                parametros.Append(" and 2 = 2");
            }
            if (!string.IsNullOrEmpty(filtro.ClassificacaoInvestimento))
            {
                // TO-DO - não sei o que é
                parametros.Append(" AND 2 = 2");
            }
            #endregion
            return await _session.Connection.QueryAsync<ProjetoEsgDTO>($@"
                select  sub.IdProjeto
                        , sub.Nomeprojeto
                        , sub.IdEmpresa
                        , sub.NomeEmpresa
                        , sub.IdGestor
                        , sub.IdStatusProjeto
                        , sub.DescricaoStatusProjeto
                        , sub.Patrocinador as NomePatrocinador
                        , case when :BaseOrcamento = 'O' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.OrcadoPartirAnoAtual,0))
                            when :BaseOrcamento = 'P' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.PrevistoPartirAnoAtual,0))
                            when :BaseOrcamento = '2' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.ReplanPartirAnoAtual,0)) 
                            else 0 end as ValorBaseOrcamento
                        , case when :FormatoAcompanhamento = '1' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.TedenciaMesAtualAteAnoVigente,0)) + sum(nvl(sub.CicloPartirAnoSeguinte,0))
                               when :FormatoAcompanhamento = 'J' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.TendenciaPartirMesAtual,0))
                               else 0 end as ValorFormatoAcompanhamento
                        , (sub.Aprovados / sub.classificacoes) * 100 as PercentualAprovados
                        , (sub.Pendentes / sub.classificacoes) * 100 as PercentualPendentes
                        , (sub.Reprovados / sub.classificacoes) * 100 as PercentualReprovados
                from (
                select p.prjcod                                            as IdProjeto
                        , trim(p.prjnom)                                   as NomeProjeto
                        , e.empcod                                         as IdEmpresa
                        , trim(e.empnomfan)                                as NomeEmpresa        
                        , LTRIM(RTRIM(U.USUNOM))                           as IdGestor
                        , usu.usunom                                       as Patrocinador
                        , st.prjstacod                                     as IdStatusProjeto
                        , trim(st.prjstades)                               as DescricaoStatusProjeto
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = 'R' 
                              and orc2.prjorcmes > 0
                              and orc2.prjorcano < to_char(sysdate,'YYYY')) as RealizadoAnoAnterior
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = 'O' 
                              and orc2.prjorcmes > 0
                              and orc2.prjorcano >= to_char(sysdate,'YYYY')) as OrcadoPartirAnoAtual
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = 'P' 
                              and orc2.prjorcmes > 0
                              and orc2.prjorcano >= to_char(sysdate,'YYYY')) as PrevistoPartirAnoAtual
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = '2' 
                              and orc2.prjorcmes > 0
                              and orc2.prjorcano >= to_char(sysdate,'YYYY')) as ReplanPartirAnoAtual
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = 'J' and orc2.prjorcmes > 0
                              and orc2.prjorcmes between to_char(sysdate,'MM') and 12 
                              and orc2.prjorcano = to_char(sysdate,'YYYY')) as TedenciaMesAtualAteAnoVigente
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = '1' and orc2.prjorcmes > 0
                              and orc2.prjorcano > to_char(sysdate,'YYYY')) as CicloPartirAnoSeguinte
                        , (select sum(prjorcval) 
                             from prjorc orc2 
                            where orc2.prjcod = orc.prjcod 
                              and orc2.prjorcfse = 0 
                              and orc2.prjorcver = 0 
                              and orc2.prjorctip = '1' and orc2.prjorcmes > 0
                              and orc2.prjorcano > 0 
                              and orc2.prjorcmes > to_char(sysdate,'MM')) as TendenciaPartirMesAtual
                        , (select count(*) from justif_classif_esg j where j.prjcod = p.prjcod and j.empcod = e.empcod) as classificacoes
                        , (select count(*) from justif_classif_esg j where j.prjcod = p.prjcod and j.empcod = e.empcod and j.status_aprovacao = 'A') as aprovados
                        , (select count(*) from justif_classif_esg j where j.prjcod = p.prjcod and j.empcod = e.empcod and j.status_aprovacao = 'P') as pendentes
                        , (select count(*) from justif_classif_esg j where j.prjcod = p.prjcod and j.empcod = e.empcod and j.status_aprovacao = 'R') as reprovados
                  from projeto p
                        inner join prjorc orc on (p.prjcod = orc.prjcod 
                                                and orc.prjorcfse = 0
                                                and orc.prjorcver = 0 
                                                and orc.prjorctip in ('O','J','R','2','1') 
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
               group by  sub.IdProjeto
                         , sub.IdEmpresa
                         , sub.IdProjeto
                         , sub.nomeprojeto
                         , sub.NomeEmpresa
                         , sub.IdGestor
                         , sub.IdStatusProjeto
                         , sub.DescricaoStatusProjeto
                         , sub.Aprovados
                         , sub.classificacoes
                         , sub.Pendentes
                         , sub.Reprovados
                         , sub.Patrocinador
               order by sub.IdProjeto
            ", new
            {
                idEmpresa = filtro.IdEmpresa,
                idGrupoPrograma = filtro.IdGrupoPrograma,
                iddiretoria = filtro.IdDiretoria,
                idGerencia = filtro.IdGerencia,
                datainicial = filtro.DataInicio.ToString("01/MM/yyyy"),
                datafinal = filtro.DataFim.ToString("01/MM/yyyy"),
                BaseOrcamento = filtro.BaseOrcamento,
                FormatoAcompanhamento = filtro.FormatoAcompanhamento,
                idProjeto = filtro.IdProjeto,
            });
        }
        public async Task<IEnumerable<ProjetoEsg>> ConsultarComboProjetosEsg(FiltroProjeto filtro)
        {
            return await _session.Connection.QueryAsync<ProjetoEsg>($@"SELECT to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prjnom)) Nome,
                                                                              prjcod Id
                                                                         FROM servdesk.projeto p, servdesk.pgmass a
                                                                        WHERE p.prjsit = 'A'
                                                                          AND a.pgmassver = 0
                                                                          AND a.pgmasscod = p.pgmasscod
                                                                          AND p.prjempcus IN :codEmpresa
                                                                          AND (EXISTS (SELECT 1
                                                                                         FROM servdesk.geradm g
                                                                                        WHERE upper(g.geradmusu) = RPAD(upper(:usuario),20)
                                                                          AND g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                                          AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(upper(:usuario),20) OR upper(p.prjreq) = RPAD(upper(:usuario),20))
                                                                          AND p.prjesg = 'S'",
            new
            {
                codEmpresa = filtro.IdEmpresa,
                usuario = filtro.Usuario?.ToUpper()
            });
        }
        public async Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto()
        {
            return await _session.Connection.QueryAsync<StatusProjetoDTO>(@$"select prjstacod as Id, trim(prjstades) as Descricao from PRJSTA where prjstasit = 'A' order by prjstaord");
        }
        public async Task<IEnumerable<CategoriaEsgDTO>> ConsultarClassificacaoEsg()
        {
            return await _session.Connection.QueryAsync<CategoriaEsgDTO>(@$"select clecod as IdCategoria, clenom as Descricao from CLAESG where clesit = 'A'");
        }
        public async Task<IEnumerable<SubCategoriaEsgDTO>> ConsultarSubClassificacaoEsg(int idCategoria)
        {
            return await _session.Connection.QueryAsync<SubCategoriaEsgDTO>(@$"select clemetcod as IdSubCategoria
                                                                                      , clemetnom as Descricao 
                                                                                 from CLAESGMET 
                                                                                where clemetsit = 'A' 
                                                                                  and clecod = :clecod",
                new
                {
                    clecod = idCategoria
                });
        }
        public async Task<int> InserirJustificativaEsg(JustificativaClassifEsg justificativa)
        {
            int id = await _session.Connection.QueryFirstOrDefaultAsync<int>(@"select SERVDESK.SEQ_JUSTIF_CLASSIF_ESG.NEXTVAL from dual ");
            int result = await _session.Connection.ExecuteAsync(@"insert into justif_classif_esg (
                                                                        id_justif_classif_esg,
                                                                        empcod, 
                                                                        dat_anomes,
                                                                        prjcod, 
                                                                        id_cat_classif, 
                                                                        id_sub_cat_classif, 
                                                                        justificativa, 
                                                                        uscriacao,
                                                                        status_aprovacao) 
                                                                        values ( 
                                                                        :id_justif_classif_esg,
                                                                        :empcod, 
                                                                        :datanomes, 
                                                                        :prjcod, 
                                                                        :idcatclassif, 
                                                                        :idsubcatclassif, 
                                                                        :justificativa, 
                                                                        :uscriacao,
                                                                        :status_aprovacao)",
            new
            {
                id_justif_classif_esg = id,
                empcod = justificativa.IdEmpresa,
                datanomes = justificativa.DataClassif.ToString("01/MM/yyyy"),
                prjcod = justificativa.IdProjeto,
                idcatclassif = justificativa.IdCatClassif,
                idsubcatclassif = justificativa.IdSubCatClassif,
                justificativa = justificativa.Justificativa,
                uscriacao = justificativa.UsCriacao,
                status_aprovacao = justificativa.StatusAprovacao
            });
            return id;
        }
        public async Task<bool> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            int result = await _session.Connection.ExecuteAsync(@"update justif_classif_esg 
                                                                     set justificativa = :justificativa, 
                                                                         usalteracao = :usalteracao,
                                                                         dtalteracao = sysdate
                                                                   where id_justif_classif_esg = :id_justif_classif_esg",
            new
            {
    
                justificativa = justificativa.Justificativa,
                usalteracao = justificativa.UsAlteracao,
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
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            #region [ Filtros ]
            StringBuilder parametros = new StringBuilder();
            if (filtro.ExibirClassificaoExcluida)
            {
                parametros.Append(" and j.status_aprovacao = 'E'");
            }
            else
            {
                parametros.Append(" and j.status_aprovacao != 'E'");
            }
            #endregion
            return await _session.Connection.QueryAsync<JustificativaClassifEsgDTO>(@$"select 
                                                                                        id_justif_classif_esg as IdJustifClassifEsg
                                                                                        , empcod              as IdEmpresa
                                                                                        , dat_anomes          as DataClassif
                                                                                        , prjcod              as IdProjeto
                                                                                        , id_cat_classif      as IdCatClassif
                                                                                        , trim(c.clenom)      as DescricaoCategoria
                                                                                        , id_sub_cat_classif  as IdSubCatClassif
                                                                                        , trim(m.clemetnom)   as DescricaoSubCategoria
                                                                                        , justificativa       as Justificativa
                                                                                        , j.status_aprovacao  as StatusAprovacao
                                                                                        , decode(j.status_aprovacao,'P','Pendente','A','Aprovado','R','Reprovado','Excluído')  as DescricaoStatusAprovacao
                                                                                        , decode(j.status_aprovacao,'E',1,0) as ClassificacaoBloqueada
                                                                                        from justif_classif_esg j 
                                                                                                inner join claesg c on (j.id_cat_classif = c.clecod)
                                                                                                inner join claesgmet m on (c.clecod = m.clecod and m.clemetcod = j.id_sub_cat_classif)
                                                                                        where j.prjcod     = :idprojeto 
                                                                                          and j.empcod     = :idempresa
                                                                                          and j.dat_anomes = :datClassif {parametros}
                                                                                    union
                                                                                    select 0 as idjustifclassifesg
                                                                                          , p.prjempcus as IdEmpresa
                                                                                          , sysdate as DataClassif
                                                                                          , p.prjcod as IdProjeto
                                                                                          , c1.clecod as IdCatClassif
                                                                                          , trim(c1.clenom)      as DescricaoCategoria
                                                                                          , c2.clemetcod  as IdSubCatClassif
                                                                                          , trim(c2.clemetnom)         as DescricaoSubCategoria
                                                                                          , ''       as Justificativa
                                                                                          , 'P'  as StatusAprovacao
                                                                                          , 'Pendente' as DescricaoStatusAprovacao
                                                                                          , 0 as ClassificacaoBloqueada
                                                                                    from projeto p, prjmet m, claesg c1, claesgmet c2
                                                                                    where c1.clecod = m.clecod 
                                                                                      and c2.clecod = m.clecod 
                                                                                      and c2.clemetcod = m.clemetcod 
                                                                                      and m.prjcod = p.prjcod
                                                                                      and p.prjcod = :idprojeto
                                                                                      and p.prjempcus = :idempresa
                                                                                      and prjmetesg  = 'S'
                                                                                      and not exists (select 1 
                                                                                                        from justif_classif_esg j
                                                                                                       where j.prjcod     = p.prjcod 
                                                                                                         and j.empcod     = p.prjempcus 
                                                                                                         and c2.clemetcod = j.id_sub_cat_classif
                                                                                                         and j.prjcod     = :idprojeto
                                                                                                         and j.empcod     = :idempresa
                                                                                                         and j.dat_anomes = :datClassif)",
                                                                                          new
                                                                                          {
                                                                                              idprojeto = filtro.IdProjeto,
                                                                                              idempresa = filtro.IdEmpresa,
                                                                                              datClassif = filtro.DataClassif.ToString("dd/MM/yyyy")
                                                                                          });
        }
        public async Task<bool> InserirAprovacao(AprovacaoClassifEsg aprovacaoClassifEsg)
        {
            try
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
            catch (Exception ex) 
            {

                throw ex;
            }

        }
        public async Task<JustificativaClassifEsgDTO> ConsultarJustificativaEsgPorId(int id)
        {
            return await _session.Connection.QueryFirstOrDefaultAsync<JustificativaClassifEsgDTO>(@$"select 
                                                                                                        id_justif_classif_esg as IdJustifClassifEsg
                                                                                                        , empcod              as IdEmpresa
                                                                                                        , dat_anomes          as DataClassif
                                                                                                        , prjcod              as IdProjeto
                                                                                                        , id_cat_classif      as IdCatClassif
                                                                                                        , c.clenom            as DescricaoCategoria
                                                                                                        , id_sub_cat_classif  as IdSubCatClassif
                                                                                                        , m.clemetnom         as DescricaoSubCategoria
                                                                                                        , justificativa
                                                                                                    from justif_classif_esg j 
                                                                                                            inner join claesg c on (j.id_cat_classif = c.clecod)
                                                                                                            inner join claesgmet m on (c.clecod = m.clecod and m.clemetcod = j.id_sub_cat_classif)
                                                                                                    where j.id_justif_classif_esg = :id_justif_classif_esg ",
                                                                                                    new
                                                                                                    {
                                                                                                        id_justif_classif_esg = id
                                                                                                    });
        }
        public async Task<IEnumerable<AprovacaoClassifEsg>> ConsultarAprovacoesPorId(int id)
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
        public async Task<bool> ExcluirClassificacao(int id)
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
