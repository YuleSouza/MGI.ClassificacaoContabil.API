using DTO.Payload;
using Service.DTO.Esg;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

namespace Service.Interface.PainelEsg
{
    public interface IPainelEsgService
    {
        Task<IEnumerable<ProjetoEsgDTO>> ConsultarProjetosPainelEsg(FiltroProjetoEsg filtro);
        Task<IEnumerable<CLassifInvestimentoDTO>> ConsultarCalssifInvestimento();
        Task<IEnumerable<StatusProjetoDTO>> ConsultarStatusProjeto();
        Task<IEnumerable<ProjetoEsg>> ConsultarComboProjetosEsg(FiltroProjeto filtro);
        Task<IEnumerable<CategoriaEsgDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<SubCategoriaEsgDTO>> ConsultarSubClassificacaoEsg(int idCategoria);
        Task<PayloadDTO> InserirJustificativaEsg(JustificativaClassifEsg justificativa);
        Task<PayloadDTO> AlterarJustificativaEsg(AlteracaoJustificativaClassifEsg justificativa);
        Task<IEnumerable<JustificativaClassifEsgDTO>> ConsultarJustificativaEsg(FiltroJustificativaClassifEsg filtro);
        Task<PayloadDTO> InserirAprovacao(int idClassifEsg, char aprovacao, string usuarioAprovacao);
        Task<PayloadDTO> ExcluirClassificacao(int id, string usCriacao);
    }
}