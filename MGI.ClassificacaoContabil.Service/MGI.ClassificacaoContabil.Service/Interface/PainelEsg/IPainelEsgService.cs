using DTO.Payload;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Interface.PainelEsg
{
    public interface IPainelEsgService
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetos(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<ProjetoEsg>> ConsultarProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<CategoriaEsgDTO>> ConsultarCategoriaEsg();
        Task<IEnumerable<SubCategoriaEsgDTO>> ConsultarSubCategoriaEsg(int idCategoria);
        Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa);
        Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<PayloadDTO> InserirAprovacao(int idClassifEsg, char aprovacao, string usuarioAprovacao);
    }
}