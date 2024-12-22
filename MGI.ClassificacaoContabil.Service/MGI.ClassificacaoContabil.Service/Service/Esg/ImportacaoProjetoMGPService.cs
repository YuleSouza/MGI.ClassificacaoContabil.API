using Service.DTO.Esg;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class ImportacaoProjetoMGPService : IImportacaoProjetoMGPService
    {
        private readonly IPainelEsgRepository _repository;
        private ITransactionHelper _transactionHelper;

        public ImportacaoProjetoMGPService(IPainelEsgRepository painelEsgRepository, ITransactionHelper transactionHelper)
        {
            _repository = painelEsgRepository;
            _transactionHelper = transactionHelper;
        }
        public async Task ImportarProjetosEsg()
        {
            IEnumerable<ImportacaoProjetoEsgMGPDTO> projetosEsg = await _repository.ConsultarProjetosEsgMGP();
            DateTime dataCassif = new (DateTime.Now.Year, DateTime.Now.Month, 1);
            foreach (var projeto in projetosEsg)
            {
                await _transactionHelper.ExecuteInTransactionAsync(async () =>
                {
                    int id = await _repository.InserirJustificativaEsg(new JustificativaClassifEsg()
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
                    await _repository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        Aprovacao = projeto.StatusAprovacao,
                        IdJustifClassifEsg = id,
                        UsCriacao = projeto.Usuario
                    });
                    var anexos = await _repository.ConsultarAnexosMGP(projeto.IdProjeto, projeto.SeqMeta);
                    foreach (var anexo in anexos)
                    {
                        await _repository.InserirAnexoJustificativaEsg(new AnexoJustificaitvaClassifEsgDTO()
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
