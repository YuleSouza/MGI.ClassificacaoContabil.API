using Dapper;
using Infra.Data;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.Projeto;
using Service.Repository.FiltroTela;

namespace Repository.FiltroTela
{
    public class FiltroTelaRepository : IFiltroTelaRepository
    {
        private readonly DbSession _session;
        public FiltroTelaRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<EmpresaDTO>> EmpresaClassificacaoContabil(FiltroEmpresa filtro)
        {
            return await _session.Connection.QueryAsync<EmpresaDTO>(
                    $@"SELECT empcod IdEmpresa, ltrim(rtrim(empnomfan)) Nome
                            FROM corpora.empres e
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
        public async Task<IEnumerable<ProjetoDTO>> ProjetoClassificacaoContabil(FiltroProjeto filtro)
        {
            return await _session.Connection.QueryAsync<ProjetoDTO>($@"SELECT to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prjnom)) nomeprojeto,
                                                                            prjcod codprojeto
                                                                       FROM servdesk.projeto p, servdesk.pgmass a
                                                                       WHERE p.prjsit = 'A'
                                                                       AND a.pgmassver = 0
                                                                       AND a.pgmasscod = p.pgmasscod
                                                                       AND p.prjempcus IN :codEmpresa
                                                                       AND ltrim(rtrim(p.prjges)) = nvl(:codGestor, ltrim(rtrim(p.prjges)))
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
        public async Task<IEnumerable<DiretoriaDTO>> DiretoriaClassificacaoContabil(FiltroDiretoria filtro)
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
        public async Task<IEnumerable<GerenciaDTO>> GerenciaClassificacaoContabil(FiltroGerencia filtro)
        {
            return await _session.Connection.QueryAsync<GerenciaDTO>(
                    $@"SELECT DISTINCT c.gcocod AS CodGerencia, ltrim(rtrim(c.gconom)) Nome
                              FROM servdesk.coordenadoria c
                             WHERE c.gcosit = 'A'
                               AND EXISTS (SELECT 1
                                      FROM projeto p, justificativa_ciclo j
                                     WHERE p.prjcod = j.prjcod
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
        public async Task<IEnumerable<GestorDTO>> GestorClassificacaoContabil(FiltroGestor filtro)
        {
            return await _session.Connection.QueryAsync<GestorDTO>(
                    $@"SELECT DISTINCT ltrim(rtrim(u.usunom)) NomeGestor,
                                          ltrim(rtrim(u.usulog)) Gestor
                              FROM corpora.usuari u
                             WHERE EXISTS (SELECT 1
                                      FROM projeto p, justificativa_ciclo j, pgmass a
                                     WHERE p.prjcod = j.prjcod
                                       AND p.prjges = u.usulog
                                       AND a.pgmassver = 0
                                       AND a.pgmasscod = p.pgmasscod
                                       AND a.pgmgrucod = nvl(NULL, a.pgmgrucod)
                                       AND p.prjempcus IN :codEmpresa
                                       AND p.geremp = nvl(:codEmpresaExecutora, p.geremp)
                                       AND p.gersig = nvl(:codDiretoria, p.gersig)
                                       AND p.gcocod = nvl(:codGerencial, p.gcocod)
                                       AND p.gsucod = nvl(:codCoordenadoria, p.gsucod)
                                    )
                             ORDER BY 1, 2", new
                    {
                        codEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codEmpresaExecutora = filtro.IdEmpresaExecutora,
                        codDiretoria = filtro.IdDiretoria,
                        codGerencial = filtro.IdGerencia,
                        codCoordenadoria = filtro.IdCoordenadoria
                    });
        }
        public async Task<IEnumerable<GrupoProgramaDTO>> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro)
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
    }
}
