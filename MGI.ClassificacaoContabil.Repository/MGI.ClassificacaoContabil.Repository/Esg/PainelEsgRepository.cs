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
            return await _session.Connection.QueryAsync< CLassifInvestimentoDTO>(@$"select pgmtipcod as Id, pgmtipnom from PGMTIP");
        }

        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (filtro.IdEmpresa >= 0)
            {
                parametros.Append(" and sub.IdEmpresa = :IdEmpresa");
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
            return await _session.Connection.QueryAsync<Service.DTO.Esg.ProjetoEsgDTO>($@"
                select  sub.IdProjeto
                        , sub.Nomeprojeto
                        , sub.IdEmpresa
                        , sub.NomeEmpresa
                        , sub.IdGestor
                        , sum(sub.ValorReplan) as TotalReplan
                        , sum(sub.ValorTendencia) as TotalTendencia
                        ,nvl(case when :TipoValorProjeto = 'O' then sum(sub.ValorOrcado)
                            when :TipoValorProjeto = 'J' then sum(sub.ValorTendencia)
                            when :TipoValorProjeto = '1' then sum(sub.ValorCiclo)
                            when :TipoValorProjeto = '2' then sum(sub.ValorReplan)
                            when :TipoValorProjeto = 'P' then sum(sub.ValorPrevisto) end,0) as ValorProjeto 
                from (
                select p.prjcod                                            as IdProjeto
                        , trim(p.prjnom)                                   as NomeProjeto
                        , e.empcod                                         as IdEmpresa
                        , trim(e.empnomfan)                                as NomeEmpresa        
                        , LTRIM(RTRIM(U.USUNOM))                           as IdGestor
                        , ''                                               as Patrocinador
                        , decode(orc.prjorctip,'O',nvl(orc.prjorcval,0),0) as ValorOrcado
                        , decode(orc.prjorctip,'J',nvl(orc.prjorcval,0),0) as ValorTendencia
                        , decode(orc.prjorctip,'R',nvl(orc.prjorcval,0),0) as ValorRealizado
                        , decode(orc.prjorctip,'2',nvl(orc.prjorcval,0),0) as ValorReplan
                        , decode(orc.prjorctip,'1',nvl(orc.prjorcval,0),0) as ValorCiclo
                        , decode(orc.prjorctip,'P',nvl(orc.prjorcval,0),0) as ValorPrevisto
                  from projeto p
                        inner join prjorc orc on (p.prjcod = orc.prjcod 
                                                and orc.prjorcfse > 0 
                                                and orc.prjorcver = 0 
                                                and orc.prjorctip in ('O','J','R','2','1') 
                                                and orc.prjorcmes > 0 and orc.prjorcano > 0)
                        inner join corpora.empres e on (e.empcod = p.prjempcus)
                        inner join corpora.usuari u on (u.USULOG = p.PRJGES)
                where p.prjsit = 'A'
                   and orc.prjorcano > 2016
                   and p.prjcod = 24814
                ) sub
                where 1 = 1 {parametros.ToString()}
               group by  sub.IdProjeto
                        , sub.IdEmpresa
                        , sub.IdProjeto
                        , sub.nomeprojeto
                        , sub.NomeEmpresa
                        , sub.IdGestor
            ", new
            {
                idEmpresa = filtro.IdEmpresa,
                idGrupoPrograma = filtro.IdGrupoPrograma,
                iddiretoria = filtro.IdDiretoria,
                idGerencia = filtro.IdGerencia,
                datainicial = filtro.MesAnoInicio.ToString("01/MM/yyyy"),
                datafinal = filtro.MesAnoFim.ToString("01/MM/yyyy"),
                TipoValorProjeto = filtro.TipoValorProjeto
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
                                                                        AND prjstg > 0",
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
