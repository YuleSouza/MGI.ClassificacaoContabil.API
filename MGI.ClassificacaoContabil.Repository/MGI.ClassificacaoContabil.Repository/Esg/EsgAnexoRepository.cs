using Dapper;
using Infra.Data;
using Service.DTO.Esg;
using Service.Repository.Esg;

namespace Repository.AnexoEsg
{
    public class EsgAnexoRepository : IEsgAnexoRepository
    {
        private readonly DbSession _session;

        public EsgAnexoRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<bool> ApagarAnexo(int id)
        {
            int qtdDeletado = await _session.Connection.ExecuteAsync(@"delete from justif_classif_esg_anexo where id_anexo = :id", new { id });
            return qtdDeletado == 1;
        }
        public async Task<AnexoJustificaitvaClassifEsgDTO> ConsultarAnexoiPorId(int idAnexo)
        {
            return await _session.Connection.QueryFirstOrDefaultAsync<AnexoJustificaitvaClassifEsgDTO>(@"select 
                                                                                                            id_anexo as IdAnexo, 
                                                                                                            id_justif_classif as IdJustifClassifEsg, 
                                                                                                            nome_anexo as NomeAnexo, 
                                                                                                            descricao_anexo as DescricaoAnexo 
                                                                                                         from justif_classif_esg_anexo
                                                                                                        where id_anexo = :idAnexo", new { idAnexo });
        }
        public async Task<IEnumerable<AnexoJustificaitvaClassifEsgDTO>> ConsultarAnexos(int idJustifClassif)
        {
            return await _session.Connection.QueryAsync<AnexoJustificaitvaClassifEsgDTO>(@"select 
                                                                                                id_anexo as IdAnexo, 
                                                                                                id_justif_classif as IdJustifClassifEsg, 
                                                                                                nome_anexo as NomeAnexo, 
                                                                                                descricao_anexo as DescricaoAnexo 
                                                                                             from justif_classif_esg_anexo
                                                                                            where id_justif_classif = :idJustifClassif", new { idJustifClassif });
        }
        public async Task<IEnumerable<AnexosMGPDTO>> ConsultarAnexosMGP(int idProjeto, int seqMeta)
        {
            return await _session.Connection.QueryAsync<AnexosMGPDTO>(@"select a.prjanearq as NomeAnexo
                                                                             , a.prjanedes as Descricao 
                                                                          from prjane a 
                                                                         where a.prjcod = :idProjeto 
                                                                           and prjanetip = 'MET'
                                                                           and prjanecod2 = :seqMeta", new { idProjeto, seqMeta });
        }
        public async Task<IEnumerable<ImportacaoProjetoEsgMGPDTO>> ConsultarProjetosEsgMGP()
        {
            return await _session.Connection.QueryAsync<ImportacaoProjetoEsgMGPDTO>(@"select 0                   as idjustifclassifesg
                                                                                          , p.prjempcus        as IdEmpresa
                                                                                          , sysdate            as DataClassif
                                                                                          , p.prjcod           as IdProjeto
                                                                                          , c1.clecod          as IdClassif
                                                                                          , c2.clemetcod       as IdSubClassif
                                                                                          , ''                 as Justificativa
                                                                                          , 'P'                as StatusAprovacao
                                                                                          , 0                  as ClassificacaoBloqueada
                                                                                          , trim(p.prjreq)     as Usuario
                                                                                          , nvl(m.prjmetvalfim,0) as PercentualKpi
                                                                                          , m.prjmetcod        as SeqMeta
                                                                                          , sysdate DataClassif
                                                                                     from projeto p, prjmet m, claesg c1, claesgmet c2
                                                                                    where c1.clecod = m.clecod 
                                                                                      and c2.clecod = m.clecod 
                                                                                      and c2.clemetcod = m.clemetcod 
                                                                                      and m.prjcod = p.prjcod
                                                                                      and prjmetesg  = 'S'
                                                                                      and p.prjesg = 'S'
                                                                                      and nvl(m.prjmetvalfim,0) > 0
                                                                                      and not exists (select 1 
                                                                                                        from justif_classif_esg j
                                                                                                       where j.prjcod     = p.prjcod 
                                                                                                         and j.empcod     = p.prjempcus 
                                                                                                         and c2.clemetcod = j.id_sub_classif)");
        }
        public async Task<int> InserirAnexoJustificativaEsg(List<AnexoJustificaitvaClassifEsgDTO> anexos)
        {
            if (anexos.Any())
            {
                foreach (var anexo in anexos)
                {
                    int result = await _session.Connection.ExecuteAsync(@"insert into justif_classif_esg_anexo (
                                                                                id_justif_classif,
                                                                                nome_anexo,
                                                                                descricao_anexo
                                                                            ) values (
                                                                                :id_justif_classif,
                                                                                :nome_anexo,
                                                                                :descricao_anexo
                                                                            )", new
                    {
                        id_justif_classif = anexo.IdJustifClassifEsg,
                        nome_anexo = anexo.NomeAnexo,
                        descricao_anexo = anexo.Descricao,
                    });
                }
            }
            return 1;
        }
        public async Task<int> InserirAnexoJustificativaEsg(AnexoJustificaitvaClassifEsgDTO anexo)
        {
            int result = await _session.Connection.ExecuteAsync(@"insert into justif_classif_esg_anexo (
                                                                        id_justif_classif,
                                                                        nome_anexo,
                                                                        descricao_anexo
                                                                    ) values (
                                                                        :id_justif_classif,
                                                                        :nome_anexo,
                                                                        :descricao_anexo
                                                                    )", new
            {
                id_justif_classif = anexo.IdJustifClassifEsg,
                nome_anexo = anexo.NomeAnexo,
                descricao_anexo = anexo.Descricao,
            });
            return 1;
        }
    }
}
