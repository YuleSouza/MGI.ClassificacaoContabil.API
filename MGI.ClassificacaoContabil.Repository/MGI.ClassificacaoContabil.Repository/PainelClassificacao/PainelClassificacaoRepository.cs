using Dapper;
using Infra.Data;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using Service.DTO.Classificacao;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Projeto;
using Service.Repository.PainelClassificacao;
using System.Text;

namespace Repository.PainelClassificacao
{
    public class PainelClassificacaoRepository : IPainelClassificacaoRepository
    {
        private readonly DbSession _session;
        public PainelClassificacaoRepository(DbSession session)
        {
            _session = session;
        }

        
        public async Task<IEnumerable<EmpresaDTO>>FiltroPainelEmpresa(FiltroPainelEmpresa filtro)
        {
            return await _session.Connection.QueryAsync<EmpresaDTO>(
                    $@"SELECT empcod IdEmpresa, ltrim(rtrim(empnomfan)) Nome
                            FROM corpora.empres e
                            INNER JOIN servdesk.classificacao_contabil c on e.empcod = c.id_empresa 
                           WHERE e.empsit = 'A'
                             AND EXISTS (SELECT 1
                                           FROM projeto p
                                          WHERE p.prjsit = 'A'
                                            AND (EXISTS (SELECT 1
                                                           FROM servdesk.geradm g
                                                          WHERE g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                            AND upper(g.geradmusu) = RPAD(:usuario,20)
                                                            AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(:usuario,20) OR upper(p.prjreq) = RPAD(:usuario,20)
                                                )
                                        )
                       ORDER BY e.grecod, e.empnomfan", new
                    {
                        usuario = filtro.Usuario?.ToUpper()
                    });
        }
        public async Task<IEnumerable<ProjetoDTO>>FiltroPainelProjeto(FiltroPainelProjeto filtro)
        {
            return await _session.Connection.QueryAsync<ProjetoDTO>($@"SELECT to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prjnom)) nomeprojeto,
                                                                            prjcod codprojeto
                                                                       FROM servdesk.projeto p, servdesk.pgmass a, servdesk.classif_contabil_prj cp 
                                                                       WHERE p.prjsit = 'A'
                                                                       AND a.pgmassver = 0
                                                                       AND a.pgmasscod = p.pgmasscod
                                                                       AND p.prjcod = cp.id_projeto
                                                                       AND p.prjempcus IN :codEmpresa
                                                                       AND (EXISTS (SELECT 1
                                                                       FROM servdesk.geradm g
                                                                       WHERE upper(g.geradmusu) = RPAD(upper(:usuario),20)
                                                                       AND g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                                       AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(upper(:usuario),20) OR upper(p.prjreq) = RPAD(upper(:usuario),20))",
            new
            {

                codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                usuario = filtro.Usuario?.ToUpper()
            });
        }
        public async Task<IEnumerable<DiretoriaDTO>>FiltroPainelDiretoria(FiltroPainelDiretoria filtro)
        {
            return await _session.Connection.QueryAsync<DiretoriaDTO>(
                    $@"SELECT DISTINCT LTRIM(RTRIM(G.GERSIG)) codDiretoria, LTRIM(RTRIM(G.GERDES)) nome
                              FROM SERVDESK.GERENCIA G
                             WHERE G.GERSIT = 'A'
                               AND EXISTS(SELECT 1 FROM PROJETO P
                                           WHERE P.GEREMP = G.GEREMP 
                                           AND P.GERSIG = G.GERSIG AND P.PRJEMPCUS IN :codEmpresa)
                               AND G.GEREMP = NVL(:codEmpresaExecutora, G.GEREMP)
                             ORDER BY 2,1", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codEmpresaExecutora = filtro.IdEmpresaExecutora
                    });
        }
        public async Task<IEnumerable<GerenciaDTO>>FiltroPainelGerencia(FiltroPainelGerencia filtro)
        {
            return await _session.Connection.QueryAsync<GerenciaDTO>(
                    $@"SELECT DISTINCT c.gcocod AS CodGerencia, ltrim(rtrim(c.gconom)) Nome
                              FROM servdesk.coordenadoria c
                             WHERE c.gcosit = 'A'
                               AND EXISTS (SELECT 1
                                      FROM projeto p, servdesk.classif_contabil_prj cp
                                     WHERE p.prjcod = cp.id_projeto
                                       AND p.geremp = c.geremp
                                       AND p.gersig = c.gersig
                                       AND p.prjempcus IN :codEmpresa
                                       AND p.gcocod = c.gcocod)
                               AND c.geremp = nvl(:codEmpresaExecutora, c.geremp)
                               AND c.gersig = nvl(:codDiretoria, c.gersig)
                             ORDER BY 2, 1", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codEmpresaExecutora = filtro.IdEmpresaExecutora,
                        codDiretoria = filtro.IdDiretoria,
                    });
        }
        public async Task<IEnumerable<GestorDTO>>FiltroPainelGestor(FiltroPainelGestor filtro)
        {
            return await _session.Connection.QueryAsync<GestorDTO>(
                    $@"SELECT DISTINCT ltrim(rtrim(u.usunom)) NomeGestor,
                                          ltrim(rtrim(u.usulog)) Gestor
                             FROM corpora.usuari u
                             WHERE EXISTS (SELECT 1
                                           FROM corpora.empres e
                                           INNER JOIN projeto p on (e.empcod = p.prjempcus and p.prjsit = 'A')
                                           INNER JOIN pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                           INNER JOIN pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                           INNER JOIN pgmass m on (m.pgmasscod = p.pgmasscod and m.pgmassver = gru.pgmgruver and m.pgmgrucod = gru.pgmgrucod and m.pgmassver = 0)
                                           INNER JOIN pgmass m2 on (m2.pgmcod = pro.pgmcod and m2.pgmassver = 0)
                                           WHERE p.prjges = u.usulog
                                           AND m.pgmassver = 0
                                           AND m.pgmasscod = p.pgmasscod
                                           AND m.pgmgrucod = nvl(NULL, m.pgmgrucod)
                                           AND p.prjempcus IN :codEmpresa
                                           AND gru.pgmgrucod  = nvl(:codGrupoPrograma,  gru.pgmgrucod)  
                                           AND pro.pgmcod   = nvl(:codPrograma,  pro.pgmcod)  
                                           AND p.prjcod = nvl(:codProjeto,  p.prjcod) 
                                    )
                             ORDER BY 1, 2", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codGrupoPrograma = filtro.CodGrupoPrograma,
                        codPrograma = filtro.IdPrograma,
                        codProjeto = filtro.IdProjeto
                    });
        }
        public async Task<IEnumerable<GrupoProgramaDTO>>FiltroPainelGrupoPrograma(FiltroPainelGrupoPrograma filtro)
        {
            return await _session.Connection.QueryAsync<GrupoProgramaDTO>(
                    $@"SELECT pgmgrucod as IdGrupoPrograma, ltrim(rtrim(pgmgrunom)) Nome
                              FROM servdesk.pgmgru g
                             WHERE pgmgruver = 0
                               AND g.pgmgrusit = 'A'
                               AND EXISTS
                             (SELECT 1
                                      FROM projeto p, pgmass a
                                     WHERE p.prjempcus IN :codEmpresa
                                       AND a.pgmassver = 0
                                       AND a.pgmasscod = p.pgmasscod
                                       AND a.pgmgrucod = g.pgmgrucod)
                             ORDER BY 2, 1", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray()
                    });
        }
        public async Task<IEnumerable<ProgramaDTO>>FiltroPainelPrograma(FiltroPainelPrograma filtro)
        {
            return await _session.Connection.QueryAsync<ProgramaDTO>(
                    $@" SELECT prg.pgmcod AS codPrograma, 
                                   LTRIM(RTRIM(pgmnom)) AS Nome
                              FROM servdesk.pgmpro prg
                             WHERE pgmver = 0
                               AND prg.pgmsit = 'A'
                               AND EXISTS (
                                   SELECT 1
                                     FROM projeto p
                                     JOIN pgmass a ON a.pgmasscod = p.pgmasscod
                                    WHERE p.prjempcus IN :codEmpresa
                                      AND a.pgmassver = 0
                                      AND a.pgmcod = prg.pgmcod
                                      AND a.pgmgrucod = :codGrupoPrograma
                               )
                             ORDER BY 2, 1", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codGrupoPrograma = (filtro.IdGrupoPrograma ?? "").Select(s => Convert.ToInt32(s)),
                        codPrograma = (filtro.IdPrograma ?? "").Select(s => Convert.ToInt32(s)),
                    });
        }
        public async Task<IEnumerable<ParametrizacaoCenarioPainelDTO>> FiltroPainelCenario(FiltroPainelCenario filtro)
        {
            return await _session.Connection.QueryAsync<ParametrizacaoCenarioPainelDTO>(
                    $@"SELECT
                            pc.id_cenario                      as IdCenario,
                            cc.nome                            as Nome
                       FROM servdesk.cenario_classif_contabil cc
                       JOIN servdesk.parametrizacao_cenario  pc on cc.id_cenario = pc.id_cenario
                       WHERE cc.id_cenario = :id_cenario", new
                    {
                        id_cenario = Convert.ToInt32(filtro.IdCenarioClassificacaoContabil)
                    });
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>>FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            return await _session.Connection.QueryAsync<Service.DTO.Classificacao.ClassificacaoContabilDTO>(
                    $@"SELECT 
                            id_classificacao_contabil AS IdClassificacaoContabil,
                            id_empresa                AS IdEmpresa,
                            status                    AS Status, 
                            mesano_inicio             AS MesAnoInicio,
                            mesano_fim                AS MesAnoFim,
                            dtcriacao                 AS DataCriacao,
                            uscriacao                 AS UsuarioCriacao,
                            dtalteracao               AS DataAlteracao,
                            usalteracao               AS UsuarioAlteracao
                      FROM servdesk.classificacao_contabil
                      WHERE status = 'A'
                      AND id_empresa IN :codEmpresa", new
                    {
                        codEmpresa = filtro.IdEmpresa
                    });
        }
        public async Task<IEnumerable<ClassificacaoEsgDTO>>FiltroPainelClassificacaoESG(FiltroPainelClassificacaoEsg filtro)
        {
            return await _session.Connection.QueryAsync<ClassificacaoEsgDTO>($@"SELECT 
                                                                                    id_classificacao_esg      AS IdClassificacaoEsg,
                                                                                    nome                      AS Nome,
                                                                                    status                    AS Status, 
                                                                                    dtcriacao                 AS DataCriacao,
                                                                                    uscriacao                 AS UsuarioCriacao,
                                                                                    dtalteracao               AS DataAlteracao,
                                                                                    usalteracao               AS UsuarioAlteracao
                                                                                FROM servdesk.classificacao_esg
                                                                                WHERE status = 'A'
                                                                                AND id_classificacao_esg = :codClassificacaoEsg",
            new
            {
                codClassificacaoEsg = filtro.IdClassificacaoEsg
            });
        }

        public async Task<IEnumerable<ClassificacaoContabilItemDTO>> ConsultarClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            StringBuilder parametros = new StringBuilder();
            #region [ filtros ]
            parametros.AppendLine(" and sub.DtLancamentoProjeto between :dataInicio and :dataFim");
            if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGrupoPrograma = :idGrupoPrograma ");
            }
            if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdPrograma = :idPrograma ");
            }
            if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
            {
                parametros.AppendLine(" and sub.IdProjeto = :idProjeto");
            }
            if (filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGestor = :idGestor");
            }
            if (filtro.IdEmpresa >= 0)
            {
                parametros.AppendLine(" and sub.IdEmpresa = :idEmpresa");
            }
            #endregion
            return await _session.Connection.QueryAsync<ClassificacaoContabilItemDTO>($@"
                                select * 
                                  from (select e.empcod as IdEmpresa
                                      , e.empnom as NomeEmpresa
                                      , gru.pgmgrucod as IdGrupoPrograma
                                      , gru.pgmgrunom as GrupoDePrograma
                                      , pro.pgmcod as IdPrograma
                                      , pro.pgmnom as Programa
                                      , p.prjcod as IdProjeto
                                      , p.prjnom as NomeProjeto      
                                      , decode(orc.prjorctip,'O',nvl(orc.prjorcval,0),0) as ValorOrcado
                                      , decode(orc.prjorctip,'J',nvl(orc.prjorcval,0),0) as ValorTendencia
                                      , decode(orc.prjorctip,'R',nvl(orc.prjorcval,0),0) as ValorRealizado
                                      , decode(orc.prjorctip,'2',nvl(orc.prjorcval,0),0) as ValorReplan
                                      , decode(orc.prjorctip,'1',nvl(orc.prjorcval,0),0) as ValorCiclo
                                      , orc.prjorctip as TipoLancamento
                                      , to_date('01' || '/' || orc.prjorcmes || '/' || orc.prjorcano) as DtLancamentoProjeto
                                      , p.prjges as IdGestor      
                                      , cl.cconom as NomeClassifContabil
                                      , cl.ccocod as IdClassifContabil
                                      , orc.prjorcfse as FseSeq
                                      , fse.prjfsepep as Pep
                                  from projeto p
                                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                                        left join pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                        left join pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                        inner join prjorc orc on (p.prjcod = orc.prjcod and orc.prjorcfse > 0 and orc.prjorcver = 0 and orc.prjorctip in ('O','J','R','2','1') AND orc.prjorcmes > 0 and orc.prjorcano > 0)
                                        left join prjfse fse on (orc.prjcod = fse.prjcod and orc.prjorcfse = fse.prjfseseq)
                                        inner join clacon cl on (fse.ccocod = cl.ccocod)
                                 where p.prjsit = 'A'
                                   and orc.prjorcano > 2016
                                    ) sub where 1 = 1 {parametros}",
                new
                {
                    idEmpresa = filtro.IdEmpresa,
                    idGrupoPrograma = filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0 ? filtro.IdGrupoPrograma : 0,
                    idPrograma = filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0 ? filtro.IdPrograma : 0,
                    idProjeto = filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0 ? filtro.IdProjeto : 0,
                    idGestor = filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0 ? filtro.IdGestor : 0,
                    dataInicio = filtro.DataInicio.AddYears(-2),
                    dataFim = filtro.DataInicio.AddYears(2),
                });
        }

        public async Task<IEnumerable<LancamentoFaseContabilDTO>> ConsultarLancamentosDaFase(FiltroPainelClassificacaoContabil filtro)
        {
            StringBuilder parametros = new StringBuilder();
            #region [ filtros ]
            if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
            {
                parametros.AppendLine(" and gru.pgmgrucod = :idGrupoPrograma ");
            }
            if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
            {
                parametros.AppendLine(" and pro.pgmcod = :idPrograma ");
            }
            if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
            {
                parametros.AppendLine(" and p.prjcod = :idProjeto");
            }
            if (filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0)
            {
                parametros.AppendLine(" and p.prjges = :idGestor");
            }
            if (filtro.IdEmpresa >= 0)
            {
                parametros.AppendLine(" and e.empcod = :idEmpresa");
            }
            #endregion
            return await _session.Connection.QueryAsync<LancamentoFaseContabilDTO>($@"
                                select e.empcod as IdEmpresa
                                       , gru.pgmgrucod as IdGrupoPrograma
                                       , pro.pgmcod as IdPrograma
                                       , p.prjcod as IdProjeto
                                       , decode(orc.prjorctip,'O',nvl(orc.prjorcval,0),0) as ValorOrcado
                                       , decode(orc.prjorctip,'J',nvl(orc.prjorcval,0),0) as ValorTendencia
                                       , decode(orc.prjorctip,'R',nvl(orc.prjorcval,0),0) as ValorRealizado
                                       , decode(orc.prjorctip,'2',nvl(orc.prjorcval,0),0) as ValorReplan
                                       , decode(orc.prjorctip,'1',nvl(orc.prjorcval,0),0) as ValorCiclo
                                       , to_date('01' || '/' || orc.prjorcmes || '/' || orc.prjorcano) as DtLancamentoProjeto
                                       , p.prjges as IdGestor
                                       , nvl(fse.prjfsenom,'') as NomeFase
                                       , nvl(fse.prjfseseq,0) as FseSeq
                                      , fse.prjfsepep as Pep
                                  from projeto p
                                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                                        left join pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                        left join pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                        inner join prjorc orc on (p.prjcod = orc.prjcod and orc.prjorcfse > 0 and orc.prjorcver = 0 and orc.prjorctip in ('O','J','R','1','2') AND orc.prjorcmes > 0 and orc.prjorcano > 0)
                                        left join prjfse fse on (fse.prjcod = orc.prjcod and fse.prjfseseq = orc.prjorcfse)
                                 where p.prjsit = 'A'
                                   and orc.prjorcano > 2016
                                       {parametros}
                                 order by 1, fse.prjfseseq
                                    ",
                new
                {
                    idEmpresa = filtro.IdEmpresa,
                    idGrupoPrograma = filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0 ? filtro.IdGrupoPrograma : 0,
                    idPrograma = filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0 ? filtro.IdPrograma : 0,
                    idProjeto = filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0 ? filtro.IdProjeto : 0,
                    idGestor = filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0 ? filtro.IdGestor : 0,
                });

        }

        public async Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro)
        {   
            return await _session.Connection.QueryAsync<LancamentoSAP>(@$"
                select e.empcod as IdEmpresa
                      , e.empnom as NomeEmpresa
                      , p.prjcod as IdProjeto
                      , (select nvl(sum(orc.prjorcval),0)
                           from prjorc orc 
                           where orc.prjorcver = 0 
                             and orc.prjorcmes = extract(month from lcc.dt_lancamento_sap)
                             and orc.prjorcano = extract(year from lcc.dt_lancamento_sap)
                             and orc.prjorctip in ('2')
                             and orc.prjorcfse > 0
                             and orc.prjcod = p.prjcod
                        ) as VlrReplan
                      ,(select nvl(sum(orc.prjorcval),0)
                           from prjorc orc 
                           where orc.prjorcver = 0 
                             and orc.prjorcmes = extract(month from lcc.dt_lancamento_sap)
                             and orc.prjorcano = extract(year from lcc.dt_lancamento_sap)
                             and orc.prjorctip in ('O')
                             and orc.prjorcfse > 0
                             and orc.prjcod = p.prjcod
                      ) as VlrOrcado
                      , lcc.valor as RealizadoAcumulado
                      , lcc.dt_lancamento_sap as DtLancamentoSap
                      , lcc.pep
                      , lcc.nomenclatura as  DescricaoLancamento
                      , nvl((select min(cl.ccocod)
                          from prjfse fse
                                inner join clacon cl on (fse.ccocod = cl.ccocod)
                         where prjcod = p.prjcod and trim(prjfsepep) = lcc.pep ),'') as IdClassifContabil
                  from lanc_classif_contabil lcc
                        inner join projeto p on (lcc.prjcod = p.prjcod and p.prjsit = 'A')
                        inner join corpora.empres e on (e.empcod = p.prjempcus and e.empcod = lcc.empcod)
                 where lcc.pep = :pep", 
            new
            {
                pep = filtro.Pep
            });
        }
        public async Task<IEnumerable<LancamentoClassificacaoEsgDTO>> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro)
        {
            StringBuilder parametros = new StringBuilder();
            parametros.AppendLine(" and 1 = 1");
            parametros.AppendLine(" and a.DtLancamentoProjeto between :dataInicio and :dataFim");
            #region [ filtros ]
            if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
            {
                parametros.AppendLine(" and a.idGrupoPrograma = :idGrupoPrograma ");
            }
            if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
            {
                parametros.AppendLine(" and a.idPrograma = :idPrograma ");
            }
            if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
            {
                parametros.AppendLine(" and a.idProjeto = :idProjeto");
            }
            if (filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0)
            {
                parametros.AppendLine(" and a.idGestor = :idGestor");
            }
            #endregion
            return await _session.Connection.QueryAsync<LancamentoClassificacaoEsgDTO>($@"select * from v_lanc_classif_esg a where a.idEmpresa = :idEmpresa {parametros.ToString()}", 
                new {
                    idEmpresa = filtro.IdEmpresa,
                    idGrupoPrograma = filtro.IdGrupoPrograma.HasValue  && filtro.IdGrupoPrograma.Value > 0 ? filtro.IdGrupoPrograma : 0,
                    idPrograma = filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0 ? filtro.IdPrograma : 0,
                    idProjeto = filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0 ? filtro.IdProjeto : 0,
                    idGestor = filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0 ? filtro.IdGestor : 0,
                    dataInicio = filtro.DataInicio.AddYears(-2).ToString("dd/MM/yyyy"),
                    dataFim = filtro.DataInicio.AddYears(2).ToString("dd/MM/yyyy"),
                });
        }

        public async Task<LancamentoClassificacaoEsgDTO> ConsultarClassifEsgPorProjeto(int idProjeto, int seqFase, int idEmpresa)
        {
            var retorno = await _session.Connection.QueryFirstOrDefaultAsync<LancamentoClassificacaoEsgDTO>(@"select e.empcod  as IdEmpresa
                                                                                       ,p.prjcod as IdProjeto
                                                                                       ,gru.pgmgrucod as IdGrupoPrograma
                                                                                       ,pro.pgmcod as IdPrograma
                                                                                  from prjfse fse
                                                                                        inner join projeto p on (p.prjcod = fse.prjcod)
                                                                                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                                                                                        left join pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                                                                        left join pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                                                                  where fse.prjcod = :idProjeto
                                                                                  and fse.prjfseseq = :seqFase
                                                                                  and e.empcod = :idEmpresa", new
                                                                                            {
                                                                                                idProjeto,
                                                                                                seqFase,
                                                                                                idEmpresa
                                                                                            });
            return retorno;
        }
        public async Task<IEnumerable<RelatorioContabilDTO>> ConsultarDadosRelatorio(FiltroPainelClassificacaoContabil filtro)
        {
            StringBuilder parametros = new StringBuilder();
            parametros.AppendLine(" where sub.DtLancamentoProjeto between :datainicial and :datafinal");
            #region [ filtros ]
            if (filtro.IdEmpresa >= 0)
            {
                parametros.AppendLine(" and sub.IdEmpresa = :idEmpresa ");
            }
            if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGrupoPrograma = :idGrupoPrograma ");
            }
            if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdPrograma = :idPrograma ");
            }
            if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
            {
                parametros.AppendLine(" and sub.IdProjeto = :idProjeto");
            }
            if (filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGestor = ':idGestor'");
            }
            #endregion
            var retorno = await _session.Connection.QueryAsync<RelatorioContabilDTO>($@"
                               select * from (
                                select nvl(pro.pgmcod,0) ||'.'||cl.ccocod||pro.pgmnom||' - '||cl.cconom as CodExterno
                                      , orc.prjorcmes ||'/'||orc.prjorcano as Data
                                      , decode(orc.prjorctip,'O',nvl(orc.prjorcval,0),0) as ValorOrcado
                                      , decode(orc.prjorctip,'J',nvl(orc.prjorcval,0),0) as ValorTendencia
                                      , decode(orc.prjorctip,'R',nvl(orc.prjorcval,0),0) as ValorRealizado
                                      , decode(orc.prjorctip,'2',nvl(orc.prjorcval,0),0) as ValorReplan
                                      , decode(orc.prjorctip,'1',nvl(orc.prjorcval,0),0) as ValorCiclo
                                      , orc.prjorctip                                    as TipoValorProjeto
                                      , to_date('01' || '/' || orc.prjorcmes || '/' || orc.prjorcano) as DtLancamentoProjeto
                                      , ''                                               as QtdProdutcaoTotal
                                      , ''                                               as SaldoInicialAndamento
                                      , ''                                               as TxImobilizado
                                      , ''                                               as TxTransfDespesa
                                      , ''                                               as TxProducao
                                      , ''                                               as TxDepreciacao
                                      , e.empcod                                         as IdEmpresa
                                      , gru.pgmgrucod                                    as IdGrupoPrograma
                                      , pro.pgmcod                                       as IdPrograma
                                      , p.prjcod                                         as IdProjeto
                                      , LTRIM(RTRIM(U.USUNOM))                           as IdGestor
                                  from projeto p
                                        inner join corpora.empres e on (p.prjempcus = e.empcod)
                                        inner join prjorc orc on (p.prjcod = orc.prjcod 
                                                                and orc.prjorcfse > 0 
                                                                and orc.prjorcver = 0 
                                                                and orc.prjorcmes > 0 
                                                                and orc.prjorcano > 0
                                                                and orc.prjorctip in ('O','J','R','2','1'))
                                        left join pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                        left join pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                        left join corpora.usuari u on (u.USULOG = p.PRJGES)
                                        left join prjfse fse on (orc.prjcod = fse.prjcod and orc.prjorcfse = fse.prjfseseq)
                                        inner join clacon cl on (fse.ccocod = cl.ccocod)
                                 where p.prjsit = 'A'
                                   and orc.prjorcano > 2016
                                 order by orc.prjorcano, orc.prjorcmes ) sub 
                                 {parametros}",new
            {
                idEmpresa = filtro.IdEmpresa,
                idGrupoPrograma = filtro.IdGrupoPrograma,
                idPrograma = filtro.IdPrograma,
                idProjeto = filtro.IdProjeto,
                idGestor = filtro.IdGestor,
                datainicial = filtro.DataInicio.AddYears(-2).ToString("01/MM/yyyy"),
                datafinal = filtro.DataFim.AddYears(2).ToString("01/MM/yyyy"),
            });
            return retorno;
        }
        public async Task<IEnumerable<RelatorioEsgDTO>> ConsultarDadosRelatorio(FiltroPainelClassificacaoEsg filtro)
        {
            StringBuilder parametros = new StringBuilder();
            parametros.AppendLine(" where sub.DtLancamentoProjeto between :datainicial and :datafinal");
            if (filtro.IdEmpresa >= 0)
            {
                parametros.AppendLine(" and sub.IdEmpresa = :idEmpresa ");
            }
            if (filtro.IdGrupoPrograma.HasValue && filtro.IdGrupoPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGrupoPrograma = :idGrupoPrograma ");
            }
            if (filtro.IdPrograma.HasValue && filtro.IdPrograma.Value > 0)
            {
                parametros.AppendLine(" and sub.IdPrograma = :idPrograma ");
            }
            if (filtro.IdProjeto.HasValue && filtro.IdProjeto.Value > 0)
            {
                parametros.AppendLine(" and sub.IdProjeto = :idProjeto");
            }
            if (filtro.IdGestor.HasValue && filtro.IdGestor.Value > 0)
            {
                parametros.AppendLine(" and sub.IdGestor = ':idGestor'");
            }
            var retorno = await _session.Connection.QueryAsync<RelatorioEsgDTO>(@"
                         select * from (
                          select e.empcod as IdEmpresa, e.empnomfan                 as NomeEmpresa
                               , p.prjcod                                           as IdProjeto
                               , p.prjnom                                           as NomeProjeto       
                               , fse.prjfseseq || '.' || fse.prjfsenom              as NomeFase
                               , dirSol.gerdes                                      as DiretoriaSolicitante
                               , coalesce(trim(gerSol.gconom),'Não tem')            as GerenciaSolicitante       
                               , dirExec.gerdes                                     as DiretoriaExecutora
                               , coalesce(trim(gerExec.gconom),'Não tem')           as GerenciaExecutora
                               , LTRIM(RTRIM(U.USUNOM))                             as Gestor
                               , to_char(p.prjdat,'MM/yyyy')                        as MesAnoProjeto
                               , (select nvl(sum(orc.prjorcval),0)
                                    from prjorc orc
                                   where orc.prjorctip = 'O'
                                     and orc.prjorcfse = fse.prjfseseq
                                     and orc.prjorcver = 0
                                     and orc.prjcod = p.prjcod
                                     AND orc.prjorcmes > 0 
                                     and orc.prjorcano > 0)                         as ValorOrcado
                               , (select nvl(sum(orc.prjorcval),0)
                                    from prjorc orc
                                   where orc.prjorctip = 'R'
                                     and orc.prjorcfse = fse.prjfseseq
                                     and orc.prjorcver = 0
                                     and orc.prjcod = p.prjcod
                                     AND orc.prjorcmes > 0 
                                     and orc.prjorcano > 0)                         as ValorRealizado
                               , (select nvl(sum(orc.prjorcval),0)
                                    from prjorc orc
                                   where orc.prjorctip = 'J'
                                     and orc.prjorcfse = fse.prjfseseq
                                     and orc.prjorcver = 0
                                     and orc.prjcod = p.prjcod
                                     AND orc.prjorcmes > 0 
                                     and orc.prjorcano > 0)                         as ValorTendencia
                               , (select nvl(sum(orc.prjorcval),0)
                                    from prjorc orc
                                   where orc.prjorctip = '2'
                                     and orc.prjorcfse = fse.prjfseseq
                                     and orc.prjorcver = 0
                                     and orc.prjcod = p.prjcod
                                     AND orc.prjorcmes > 0 
                                     and orc.prjorcano > 0)                         as ValorReplan
                               , (select nvl(sum(orc.prjorcval),0)
                                    from prjorc orc
                                   where orc.prjorctip = '1'
                                     and orc.prjorcfse = fse.prjfseseq
                                     and orc.prjorcver = 0
                                     and orc.prjcod = p.prjcod
                                     AND orc.prjorcmes > 0 
                                     and orc.prjorcano > 0)                         as ValorCiclo
                                , nvl(fse.ccocod,0)                                 as IdClassificacaoContabil
                                , nvl(gru.pgmgrucod,0)                              as IdGrupoPrograma
                                , nvl(pro.pgmcod,0)                                 as IdPrograma
                                , to_date('01' || '/' || orc.prjorcmes || '/' || orc.prjorcano) as DtLancamentoProjeto
                          from corpora.empres e
                                inner join projeto p on (p.prjempcus = e.empcod)
                                left join pgmgru gru on (gru.pgmgrucod = p.prjpgmgru)
                                left join pgmpro pro on (pro.pgmcod = p.prjpgmcod)
                                inner join prjfse fse on (fse.prjcod = p.prjcod)        
                                left join corpora.usuari u on (u.USULOG = p.PRJGES)
                                left join gerencia dirExec on (p.geremp = dirExec.geremp and dirExec.gersig = p.gersig)        
                                left join coordenadoria gerExec on (p.geremp = gerExec.geremp and p.gersig = gerExec.gersig and p.gcocod = gerExec.gcocod)
                                left join gerencia dirSol on (p.prjgeremp = dirSol.geremp and dirSol.gersig = p.prjger)
                                left join coordenadoria gerSol on (p.prjgeremp = gerSol.geremp and p.gersig = gerSol.gersig and p.PrjGco = gerSol.gcocod)
                        where p.prjsit = 'A'
                          and orc.prjorcano > 2016) sub
                        where 1 = 1", new 
                        {
                            dataInicio = filtro.DataInicio.AddYears(-2),
                            dataFim = filtro.DataInicio.AddYears(2),
                        });
            return retorno;
        }
    }
}
