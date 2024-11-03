using DTO.Payload;
using Service.DTO.Parametrizacao;

namespace Service.Interface.Parametrizacao
{
    public interface IParametrizacaoService
    {

        #region [ESG Excecao]
        Task<PayloadDTO> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao);
        Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>>> ConsultarParametrizacaoClassificacaoExcecao();
        #endregion
    }
}
