using DTO.Payload;
using MGI.ClassificacaoContabil.Service.Helper;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class PainelEsgService : IPainelEsgService
    {
        private readonly IPainelEsgRepository _painelEsgRepository;
        private ITransactionHelper _transactionHelper;
        public PainelEsgService(IPainelEsgRepository painelEsgRepository, ITransactionHelper transactionHelper)
        {
            _painelEsgRepository = painelEsgRepository;
            _transactionHelper = transactionHelper;
        }

        #region [ Categoria Esg]
        public async Task<IEnumerable<CategoriaEsgDTO>> ConsultarCategoriaEsg()
        {
            return await _painelEsgRepository.ConsultarCategoriaEsg();
        }

        public async Task<IEnumerable<SubCategoriaEsgDTO>> ConsultarSubCategoriaEsg(int idCategoria)
        {
            return await _painelEsgRepository.ConsultarSubCategoriaEsg(idCategoria);
        }


        #endregion
        public async Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento()
        {
            return await _painelEsgRepository.ConsultarCalssifInvestimento();
        }

        public async Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro)
        {
            return await _painelEsgRepository.ConsultarProjetos(filtro);
        }

        public async Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto()
        {
            return await _painelEsgRepository.ConsultarStatusProjeto();
        }
        public async Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro)
        {
            return await _painelEsgRepository.ConsultarProjetosEsg(filtro);
        }

        public async Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _painelEsgRepository.InserirJustificativaEsg(justificativa),
                "Classificacao Inserido com sucesso"
            );
        }
        public async Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _painelEsgRepository.AlterarJustificativaEsg(justificativa),
                "Classificacao alterada com sucesso"
            );
        }
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            return await _painelEsgRepository.ConsultarJustificativaEsg(filtro);
        }

    }
}
