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

        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro)
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
            return await _session.Connection.QueryAsync<Service.DTO.Esg.ProjetoEsgDTO>($@"
                select  sub.IdProjeto
                        , sub.Nomeprojeto
                        , sub.IdEmpresa
                        , sub.NomeEmpresa
                        , sub.IdGestor
                        , sub.IdStatusProjeto
                        , sub.DescricaoStatusProjeto
                        , case when :BaseOrcamento = 'O' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.OrcadoPartirAnoAtual,0))
                            when :BaseOrcamento = 'P' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.PrevistoPartirAnoAtual,0))
                            when :BaseOrcamento = '2' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.ReplanPartirAnoAtual,0)) 
                            else 0 end as ValorBaseOrcamento
                        , case when :FormatoAcompanhamento = '1' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.TedenciaMesAtualAteAnoVigente,0)) + sum(nvl(sub.CicloPartirAnoSeguinte,0))
                               when :FormatoAcompanhamento = 'J' then sum(nvl(sub.RealizadoAnoAnterior,0)) + sum(nvl(sub.TendenciaPartirMesAtual,0))
                               else 0 end as ValorFormatoAcompanhamento
                from (
                select p.prjcod                                            as IdProjeto
                        , trim(p.prjnom)                                   as NomeProjeto
                        , e.empcod                                         as IdEmpresa
                        , trim(e.empnomfan)                                as NomeEmpresa        
                        , LTRIM(RTRIM(U.USUNOM))                           as IdGestor
                        , ''                                               as Patrocinador
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
                  from projeto p
                        inner join prjorc orc on (p.prjcod = orc.prjcod 
                                                and orc.prjorcfse = 0
                                                and orc.prjorcver = 0 
                                                and orc.prjorctip in ('O','J','R','2','1') 
                                                and orc.prjorcmes > 0 and orc.prjorcano > 0)
                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                        inner join corpora.usuari u on (u.USULOG = p.PRJGES)
                        inner join prjsta st on (st.prjstacod = p.prjsta and prjstasit = 'A')
                where p.prjsit = 'A'
                   and orc.prjorcano > 2016
                   or p.prjesg = 'S'
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

        public async Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro)
        {
            return await _session.Connection.QueryAsync<ProjetoEsg>($@"SELECT to_char(prjcod, '00000') || ' - ' || ltrim(rtrim(prjnom)) Nome,
                                                                              prjcod Id
                                                                         FROM servdesk.projeto p, servdesk.pgmass a
                                                                        WHERE p.prjsit = 'A'
                                                                          AND a.pgmassver = 0
                                                                          AND p.prjstg > 0
                                                                          AND a.pgmasscod = p.pgmasscod
                                                                          AND p.prjempcus IN :codEmpresa
                                                                          AND (EXISTS (SELECT 1
                                                                                         FROM servdesk.geradm g
                                                                                        WHERE upper(g.geradmusu) = RPAD(upper(:usuario),20)
                                                                          AND g.geremp IN (p.prjempcus, p.prjgeremp, p.geremp, 999)
                                                                          AND g.gersig IN (p.prjger, p.gersig, 'AAA')) OR upper(p.prjges) = RPAD(upper(:usuario),20) OR upper(p.prjreq) = RPAD(upper(:usuario),20))
                                                                          AND p.prjesg is not null",
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
    }
}
