using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Repository.Esg
{
    public interface IPainelEsgRepository
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<ProjetoEsg>> ConsultarComboProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<ClassificacaoEsgDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<SubClassificacaoEsgDTO>> ConsultarSubClassificacaoEsg(int idClassificacao);
        Task<int> InserirJustificativaEsg(JustificativaClassifEsg justificativa);
        Task<bool> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<bool> AlterarStatusJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<bool> InserirAprovacao(AprovacaoClassifEsg aprovacaoClassifEsg);
        Task<JustificativaClassifEsgDTO> ConsultarJustificativaEsgPorId(int id);
        Task<IEnumerable<AprovacaoClassifEsg>> ConsultarAprovacoesPorId(int id);
        Task<bool> ExcluirClassificacao(int id);
    }
}
