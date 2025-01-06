using DTO.Payload;
using Service.DTO.Combos;
using Service.DTO.Esg;
using Service.DTO.Filtros;

namespace Service.Interface.PainelEsg
{
    public interface IPainelEsgService
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<PayloadComboDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<PayloadComboDTO>> ConsultarComboProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<PayloadComboDTO>> ConsultarSubClassificacaoEsg(int idClassificacao);
        Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa);
        Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<IEnumerable<AprovacaoClassifEsg>> ConsultarLogAprovacoesPorId(int id);
        Task<PayloadDTO> InserirAprovacao(int idClassifEsg, string statusAprovacao, string usuarioAprovacao);
        Task<PayloadDTO> ExcluirClassificacao(int id, string usCriacao);
    }
}