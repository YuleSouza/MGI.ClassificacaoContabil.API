using Dapper;
using Infra.Data;
using Service.DTO.Combos;
using Service.DTO.Filtros;
using Service.Repository.FiltroTela;
using System.Text;

namespace Repository.FiltroTela
{
    public class FiltroTelaRepository : IFiltroTelaRepository
    {
        private readonly DbSession _session;
        public FiltroTelaRepository(DbSession session)
        {
            _session = session;
        }
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarEmpresaClassifContabil(FiltroEmpresa filtro)
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"SELECT empcod as Id, ltrim(rtrim(empnomfan)) as Descricao
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
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarProjetoClassifContabil(FiltroProjeto filtro)
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>($@"SELECT concat(concat(lpad(prjcod, 5, '0'),' - '), trim(prjnom)) as Descricao,
                                                                              prjcod as Id
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

        public async Task<IEnumerable<PayloadComboDTO>> ConsultarDiretoriaClassifiContabil(FiltroDiretoria filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(filtro.IdEmpresa))
            {
                parametros.Append(@" AND EXISTS(SELECT 1 FROM PROJETO P
                                           WHERE P.GEREMP = G.GEREMP 
                                           AND P.GERSIG = G.GERSIG AND P.PRJEMPCUS IN :codEmpresa)");
            }
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"SELECT DISTINCT LTRIM(RTRIM(G.GERSIG)) as Id, LTRIM(RTRIM(G.GERDES)) as Descricao
                              FROM SERVDESK.GERENCIA G
                             WHERE G.GERSIT = 'A'
                               AND G.GEREMP = NVL(:codEmpresaExecutora, G.GEREMP)
                                {parametros}
                             ORDER BY 2,1", new
                    {
                        codEmpresa = string.IsNullOrEmpty(filtro.IdEmpresa) ? new int[1] : (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codEmpresaExecutora = filtro.IdEmpresaExecutora
                    });
        }

        public async Task<IEnumerable<PayloadComboDTO>> ConsultarGerenciaClassifContabil(FiltroGerencia filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(filtro.IdEmpresa))
            {
                parametros.Append(@"AND EXISTS (SELECT 1
                                      FROM projeto p, justificativa_ciclo j
                                     WHERE p.prjcod = j.prjcod
                                       AND p.geremp = c.geremp
                                       AND p.gersig = c.gersig
                                       AND p.prjempcus IN :codEmpresa
                                       AND p.gcocod = c.gcocod)");
            }
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"SELECT DISTINCT c.gcocod AS Id, ltrim(rtrim(c.gconom)) as Descricao
                              FROM servdesk.coordenadoria c
                             WHERE c.gcosit = 'A' 
                               AND c.geremp = nvl(:codEmpresaExecutora, c.geremp)
                               AND c.gersig = nvl(:codDiretoria, c.gersig)
                                 {parametros}
                             ORDER BY 2, 1", new
                    {
                        codEmpresa = string.IsNullOrEmpty(filtro.IdEmpresa) ? new int[1] : (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                        codEmpresaExecutora = filtro.IdEmpresaExecutora,
                        codDiretoria = filtro.IdDiretoria,
                    });
        }

        public async Task<IEnumerable<PayloadComboDTO>> ConsultarGestorClassifContabil(FiltroGestor filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(filtro.IdEmpresa))
            {
                parametros.Append(" AND p.prjempcus IN :codEmpresa ");
            }
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"SELECT DISTINCT ltrim(rtrim(u.usunom)) as Descricao,
                                          ltrim(rtrim(u.usulog)) as Id
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
                                           {parametros}
                                           AND gru.pgmgrucod  = nvl(:codGrupoPrograma,  gru.pgmgrucod)  
                                           AND pro.pgmcod   = nvl(:codPrograma,  pro.pgmcod)  
                                           AND p.prjcod = nvl(:codProjeto,  p.prjcod) 
                                    )
                             ORDER BY 1, 2", new
                    {
                        codEmpresa = !string.IsNullOrEmpty(filtro.IdEmpresa) ? filtro.IdEmpresa : "",
                        codGrupoPrograma = filtro.CodGrupoPrograma,
                        codPrograma = filtro.IdPrograma,
                        codProjeto = filtro.IdProjeto
                    });
        }

        public async Task<IEnumerable<PayloadComboDTO>> ConsultarGrupoProgramaClassifContabil(FiltroGrupoPrograma filtro)
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
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"SELECT pgmgrucod as Id, ltrim(rtrim(pgmgrunom)) as Descricao
                         FROM servdesk.pgmgru g
                        WHERE pgmgruver = 0
                          AND g.pgmgrusit = 'A'
                              {parametroEmpresa}
                        ORDER BY 2, 1", new
                    {
                        codEmpresa = !string.IsNullOrEmpty(filtro.IdEmpresa) ? (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray() : null,
                    });
        }

        public async Task<IEnumerable<PayloadComboDTO>> ConsultarProgramaClassifContabil(FiltroPrograma filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (!string.IsNullOrEmpty(filtro.IdGrupoPrograma))
            {
                parametros.Append(" and pgp.pgmgrucod = :codGrupoPrograma");
            }
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                    $@"select 
                              prg.pgmcod as Id, 
                              trim(pgmnom) as Descricao
                        from servdesk.pgmpro prg
                        join servdesk.pgmass pgp on pgp.pgmcod = prg.pgmcod
                        join servdesk.pgmgru gp on  gp.pgmgrucod = pgp.pgmgrucod
                        where 1 = 1
                        {parametros}
                        order by 2, 1", new
                    {
                        codGrupoPrograma = Convert.ToInt32(filtro.IdGrupoPrograma)
                    });
        }
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoContabil()
        {
            var resultado = await _session.Connection.QueryAsync<PayloadComboDTO>($@"
                                                    select cc.id_classificacao_contabil  as Id,    
                                                           ltrim(rtrim(a.empnomfan))     as Descricao
                                                     from classificacao_contabil cc 
                                                     inner join corpora.empres a on cc.id_empresa = a.empcod
                                                     where 1 = 1
                                                     order by mesano_fim");

            return resultado;
        }
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg()
        {
            var resultado = await _session.Connection.QueryAsync<PayloadComboDTO>($@"
                                           select 
                                                id_classificacao_esg  as Id,
                                                nome                  as Descricao
                                            from classificacao_esg
                                            where 1 = 1");
            return resultado;
        }
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarCenario()
        {
            var resultado = await _session.Connection.QueryAsync<PayloadComboDTO>($@"
                                           select 
                                                id_cenario          as Id,
                                                nome                as Descricao
                                            from cenario_classif_contabil
                                            where 1 = 1");
            return resultado;
        }
        
        public async Task<IEnumerable<PayloadComboDTO>> ConsultarCoordenadoria(FiltroCoordenadoria filtro)
        {
            return await _session.Connection.QueryAsync<PayloadComboDTO>(
                $@"SELECT DISTINCT s.gsucod as Id, ltrim(rtrim(s.gsunom)) as Descricao
                        FROM servdesk.supervisao s
                    WHERE s.gsusit = 'A'
                        AND s.gcocod = nvl(:idGerencia, s.gcocod)
                        ORDER BY 2, 1", new
                {
                    idEmpresa = (filtro.IdEmpresa ?? "").Split(',').Select(s => Convert.ToInt32(s)).ToArray(),
                    idGerencia = filtro.IdGerencia
                });
        }
    }
}
