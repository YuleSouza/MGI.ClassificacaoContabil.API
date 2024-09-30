using Dapper;
using Infra.Data;
using Service.DTO.Empresa;
using Service.DTO.Projeto;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;
using Service.Repository.FiltroTela;
using Service.DTO.Cenario;

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
            return await _session.Connection.QueryAsync<ProjetoDTO>($@"SELECT to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prjnom)) nome,
                                                                            prjcod codprojeto
                                                                       FROM servdesk.projeto p, servdesk.pgmass a
                                                                       WHERE p.prjsit = 'A'
                                                                       AND a.pgmassver = 0
                                                                       AND a.pgmasscod = p.pgmasscod
                                                                       AND p.prjempcus IN :codEmpresa
                                                                       AND (EXISTS (SELECT 1
                                                                       FROM servdesk.geradm g
                                                                       WHERE upper(g.geradmusu) = RPAD(upper(:usuario),20)
                                                                       AND g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                                       AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(upper(:usuario),20) OR upper(p.prjreq) = RPAD(upper(:usuario),20))",
            new
            {
                
                codEmpresa = !string.IsNullOrEmpty(filtro.IdEmpresa) ? (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray() : null,
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
        public async Task<IEnumerable<GrupoProgramaDTO>> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro)
        {
            string parametroEmpresa = string.Empty;
            if (!string.IsNullOrEmpty(filtro.IdEmpresa))
            {
                parametroEmpresa = @" AND EXISTS (SELECT 1
                                      FROM projeto p, pgmass a
                                     WHERE p.prjempcus IN :codEmpresa
                                       AND a.pgmassver = 0
                                       AND a.pgmasscod = p.pgmasscod
                                       AND a.pgmgrucod = g.pgmgrucod)";
            }
            return await _session.Connection.QueryAsync<GrupoProgramaDTO>(
                    $@"SELECT pgmgrucod as IdGrupoPrograma, ltrim(rtrim(pgmgrunom)) Nome
                         FROM servdesk.pgmgru g
                        WHERE pgmgruver = 0
                          AND g.pgmgrusit = 'A'
                              {parametroEmpresa}
                        ORDER BY 2, 1", new
                    {
                        codEmpresa = !string.IsNullOrEmpty(filtro.IdEmpresa) ? (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray() : null,
                    });
        }
        public async Task<IEnumerable<ProgramaDTO>> ProgramaClassificacaoContabil(FiltroPrograma filtro)
        {
            return await _session.Connection.QueryAsync<ProgramaDTO>(
                    $@"select 
                              prg.pgmcod codPrograma, 
                              ltrim(rtrim(pgmnom)) Nome
                        from servdesk.pgmpro prg
                        join servdesk.pgmass pgp on pgp.pgmcod = prg.pgmcod
                        join servdesk.pgmgru gp on  gp.pgmgrucod = pgp.pgmgrucod
                        where pgp.pgmgrucod = :codGrupoPrograma
                        order by 2, 1", new
                    {
                        codGrupoPrograma = Convert.ToInt32(filtro.IdGrupoPrograma)
                    });
        }
        public async Task<IEnumerable<ClassificacaoContabilFiltroDTO>> ClassificacaoContabil()
        {
            var resultado = await _session.Connection.QueryAsync<ClassificacaoContabilFiltroDTO>($@"
                                                    select cc.id_classificacao_contabil  as IdClassificacaoContabil,    
                                                           ltrim(rtrim(a.empnomfan))     as Nome
                                                     from classificacao_contabil cc 
                                                     inner join corpora.empres a on cc.id_empresa = a.empcod
                                                     where 1 = 1
                                                     order by mesano_fim");

            return resultado;
        }
        public async Task<IEnumerable<ClassificacaoEsgFiltroDTO>> ClassificacaoEsg()
        {
            var resultado = await _session.Connection.QueryAsync<ClassificacaoEsgFiltroDTO>($@"
                                           select 
                                                id_classificacao_esg  as IdClassificacaoEsg,
                                                nome                  as Nome
                                            from classificacao_esg
                                            where 1 = 1");
            return resultado;
        }
        public async Task<IEnumerable<CenarioDTO>> Cenario()
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
                                            where 1 = 1");
            return resultado;
        }
    }
}
