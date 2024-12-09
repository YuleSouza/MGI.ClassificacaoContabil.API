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
        private List<char> _aprovacoes = new List<char> { 'P', 'A', 'R' };
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
                async () =>
                {
                    justificativa.StatusAprovacao = 'P';
                    int id_classif_esg = await _painelEsgRepository.InserirJustificativaEsg(justificativa);
                    await _painelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = id_classif_esg,
                        Aprovacao = 'P', // Pendente
                        UsCriacao = justificativa.UsCriacao
                    });
                    // to-do enviar aprovacao pro email
                    return true;
                }, "Classificacao Inserido com sucesso"
            );
        }
        public async Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _painelEsgRepository.AlterarJustificativaEsg(justificativa)                    
                , "Classificacao alterada com sucesso"
            );
        }
        public async Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro)
        {
            return await _painelEsgRepository.ConsultarJustificativaEsg(filtro);
        }
        public async Task<PayloadDTO> InserirAprovacao(int idClassifEsg, char aprovacao, string usuarioAprovacao)
        {
            if (!_aprovacoes.Contains(aprovacao)) 
            {
                return new PayloadDTO("Tipo de aprovações permitidas A,P ou R", false);
            }
            await _transactionHelper.ExecuteInTransactionAsync(
                async () =>
                {
                    var classifEsg = await _painelEsgRepository.ConsultarJustificativaEsgPorId(idClassifEsg);
                    await _painelEsgRepository.InserirAprovacao(new AprovacaoClassifEsg()
                    {
                        IdJustifClassifEsg = classifEsg.IdJustifClassifEsg,
                        Aprovacao = aprovacao,
                        UsCriacao = usuarioAprovacao
                    });
                    await _painelEsgRepository.AlterarStatusJustificativaEsg(new AlteracaoJustificativaClassifEsg()
                    {
                        IdJustifClassifEsg = classifEsg.IdJustifClassifEsg,
                        StatusAprovacao = aprovacao
                    });
                    return true;
                }, "");
            return new PayloadDTO("Classificação aprovada com sucesso", true);
        }
    }
}
