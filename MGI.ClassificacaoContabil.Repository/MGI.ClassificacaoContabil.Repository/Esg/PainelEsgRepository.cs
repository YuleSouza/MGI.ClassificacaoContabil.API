using Dapper;
using Infra.Data;
using Service.DTO.Esg;
using Service.DTO.Filtros;
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
        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro)
        {
            StringBuilder parametros = new StringBuilder();
            if (filtro.IdEmpresa >= 0)
            {
                parametros.Append(" and sub.IdEmpresa = :IdEmpresa");
            }
            if (!string.IsNullOrEmpty(filtro.IdGrupoPrograma))
            {
                parametros.Append(" AND sub.IdGrupoPrograma = :idgrupoprogrma ");
            }
            if (!string.IsNullOrEmpty(filtro.IdGerencia))
            {
                parametros.Append(" AND sub.IdGerencia = :idgerencia ");
            }
            if (!string.IsNullOrEmpty(filtro.IdGestor))
            {
                parametros.Append(" AND TRIM(sub.IdGestor) = :idgestor ");
            }
            if (!string.IsNullOrEmpty(filtro.IdDiretoria))
            {
                parametros.Append(" AND TRIM(sub.IdDiretoria) = :iddiretoria ");
            }
            if (!string.IsNullOrEmpty(filtro.StatusProjeto))
            {
                // TO-DO - definir regra
                parametros.Append(" AND 2 = 2");
            }
            if (!string.IsNullOrEmpty(filtro.StatusAprovacao))
            {
                // TO-DO - definir regra
                parametros.Append(" AND 2 = 2");
            }
            if (!string.IsNullOrEmpty(filtro.ClassificacaoInvestimento))
            {
                // TO-DO - não sei o que é
                parametros.Append(" AND 2 = 2");
            }
            return await _session.Connection.QueryAsync<ProjetoEsgDTO>($@"
                select  sub.IdProjeto
                        , sub.Nomeprojeto
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
                select p.prjcod as IdProjeto
                        , p.prjnom as NomeProjeto
                        , e.empnomfan as NomeEmpresa        
                        , LTRIM(RTRIM(U.USUNOM)) as IdGestor
                        , '' as Patrocinador
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
                        , sub.nomeprojeto
                        , sub.NomeEmpresa
                        , sub.IdGestor
            ", new
            {
                idEmpresa = filtro.IdEmpresa,
                idGrupoPrograma = filtro.IdGrupoPrograma,
                iddiretoria = filtro.IdDiretoria,
                idGestor = filtro.IdGestor,
                datainicial = filtro.MesAnoInicio.ToString("01/MM/yyyy"),
                datafinal = filtro.MesAnoFim.ToString("01/MM/yyyy"),
                TipoValorProjeto = filtro.TipoValorProjeto
            });
        }
    }
}
