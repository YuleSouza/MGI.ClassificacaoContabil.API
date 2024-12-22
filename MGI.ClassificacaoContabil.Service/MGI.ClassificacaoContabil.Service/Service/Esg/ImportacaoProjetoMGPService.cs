using Service.DTO.Esg;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class ImportacaoProjetoMGPService : IImportacaoProjetoMGPService
    {
        private readonly IEsgAnexoRepository _esgAnexoRepository;
        private readonly IPainelEsgRepository _panelEsgRepository;
        private ITransactionHelper _transactionHelper;

        public ImportacaoProjetoMGPService(IEsgAnexoRepository esgAnexoRepository
            , IPainelEsgRepository painelEsgRepository
            , ITransactionHelper transactionHelper)
        {
            _esgAnexoRepository = esgAnexoRepository;
            _transactionHelper = transactionHelper;
            _panelEsgRepository = painelEsgRepository;
        }
        public async Task ImportarProjetosEsg()
        {
            IEnumerable<ImportacaoProjetoEsgMGPDTO> projetosEsg = await _esgAnexoRepository.ConsultarProjetosEsgMGP();
            DateTime dataCassif = new (DateTime.Now.Year, DateTime.Now.Month, 1);
            foreach (var projeto in projetosEsg)
            {
                await _transactionHelper.ExecuteInTransactionAsync(async () =>
                {
                    int id = await _panelEsgRepository.InserirJustificativaEsg(new JustificativaClassifEsg()
                    {
                        IdEmpresa = projeto.IdEmpresa,
                        IdClassif = projeto.IdClassif,
                        IdSubClassif = projeto.IdSubClassif,
                        IdProjeto = projeto.IdProjeto,
                        Justificativa = projeto.Justificativa,
                        StatusAprovacao = projeto.StatusAprovacao,
                        DataClassif = dataCassif,
                        PercentualKpi = projeto.PercentualKpi,
                    });
                    await _panelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        Aprovacao = projeto.StatusAprovacao,
                        IdJustifClassifEsg = id,
                        UsCriacao = projeto.Usuario
                    });
                    var anexos = await _esgAnexoRepository.ConsultarAnexosMGP(projeto.IdProjeto, projeto.SeqMeta);
                    foreach (var anexo in anexos)
                    {
                        await _esgAnexoRepository.InserirAnexoJustificativaEsg(new AnexoJustificaitvaClassifEsgDTO()
                        {
                            IdJustifClassifEsg = projeto.IdJustifClassifEsg,
                            IdProjeto = projeto.IdProjeto,
                            Descricao = anexo.Descricao,
                            NomeAnexo = anexo.NomeAnexo
                        });
                    }
                    return true;
                }, "Importacao feito com sucess!");
            }
        }
    }
}
