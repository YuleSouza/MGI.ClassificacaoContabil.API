using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Repository.Esg
{
    public interface IPainelEsgRepository
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<CategoriaEsgDTO>> ConsultarCategoriaEsg();
        Task<IEnumerable<SubCategoriaEsgDTO>> ConsultarSubCategoriaEsg(int idCategoria);
        Task<int> InserirJustificativaEsg(JustificativaClassifEsg justificativa);
        Task<bool> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<bool> AlterarStatusJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<bool> InserirAprovacao(AprovacaoClassifEsg aprovacaoClassifEsg);
        Task<JustificativaClassifEsgDTO> ConsultarJustificativaEsgPorId(int id);
    }
}
