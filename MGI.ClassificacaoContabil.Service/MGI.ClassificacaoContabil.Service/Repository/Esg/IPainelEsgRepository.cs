using Service.DTO.Combos;
using Service.DTO.Esg;
using Service.DTO.Filtros;

namespace Service.Repository.Esg
{
    public interface IPainelEsgRepository
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosEsg(FiltroProjetoEsg filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<PayloadComboDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<PayloadComboDTO>> ConsultarComboProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<PayloadComboDTO>> ConsultarSubClassificacaoEsg(int idClassificacao);
        Task<int> InserirJustificativaEsg(JustificativaClassifEsg justificativa);        
        Task<bool> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<bool> AlterarStatusJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<bool> InserirAprovacao(AprovacaoClassifEsg aprovacaoClassifEsg);
        Task<JustificativaClassifEsgDTO> ConsultarJustificativaEsgPorId(int id);
        Task<IEnumerable<AprovacaoClassifEsg>> ConsultarLogAprovacoesPorId(int id);
        Task<bool> RemoverClassificacao(int id);
    }
}
