using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Classificacao;

namespace Service.Interface.Classificacao
{
    public interface IClassificacaoService
    {
        #region Contabil
        Task<PayloadDTO> InserirClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<PayloadDTO> AlterarClassificacaoContabil(ClassificacaoContabilDTO classificacao);
        Task<PayloadDTO> ConsultarClassificacaoContabil(ClassificacaoContabilFiltro filtro);
        #endregion

        #region ESG
        Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);
        #endregion
    }
}
