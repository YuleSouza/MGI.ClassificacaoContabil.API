using Infra.Data;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.Projeto;
using Service.DTO.Cenario;
using Service.DTO.Classificacao;
using Service.Repository.PainelClassificacao;

using Dapper;
using Service.DTO.Parametrizacao;

namespace Repository.PainelClassificacao
{
    public class PainelClassificacaoRepository : IPainelClassificacaoRepository
    {
        private readonly DbSession _session;
        public PainelClassificacaoRepository(DbSession session)
        {
            _session = session;
        }

        #region [Filtros]
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
                    $@"SELECT pgmgrucod codGrupoPrograma, ltrim(rtrim(pgmgrunom)) Nome
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
                            pc.id_cenario_classif_contabil     as IdCenarioClassificacaoContabil,
                            cc.nome                            as Nome
                       FROM servdesk.cenario_classif_contabil cc
                       JOIN servdesk.parametrizacao_cenario  pc on cc.id_cenario_classif_contabil = pc.id_cenario_classif_contabil
                       WHERE cc.id_cenario_classif_contabil = :codCenarioClassif", new
                    {
                        codCenarioClassif = Convert.ToInt32(filtro.IdCenarioClassificacaoContabil)
                    });
        }
        public async Task<IEnumerable<ClassificacaoContabilDTO>>FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro)
        {
            return await _session.Connection.QueryAsync<ClassificacaoContabilDTO>(
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
                        codEmpresa = (filtro.IdEmpresa ?? "").Select(s => Convert.ToInt32(s))
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
                codClassificacaoEsg = (filtro.IdClassificacaoEsg ?? "").Select(s => Convert.ToInt32(s))
            });
        }

        #endregion

        #region [Contabil]

        #endregion
    }
}
