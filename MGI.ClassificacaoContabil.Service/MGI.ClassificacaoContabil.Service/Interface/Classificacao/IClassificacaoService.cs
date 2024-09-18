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
        Task<PayloadDTO> ConsultarClassificacaoContabil();
        Task<PayloadDTO> ConsultarClassificacaoContabil(FiltroClassificacaoContabil filtro);

        Task<PayloadDTO> InserirProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);
        Task<PayloadDTO> AlterarProjetoClassificacaoContabil(ClassificacaoProjetoDTO projeto);

        Task<PayloadDTO> ConsultarProjetoClassificacaoContabil();
        Task<PayloadDTO> ConsultarProjetoClassificacaoContabil(FiltroClassificacaoContabil filtro);
        Task<bool> VerificarRegraExcessaoContabil(FiltroClassificacaoContabil filtro);
        Task<IEnumerable<ClassificacaoContabilMgpDTO>> ConsultarClassificacaoContabilMGP();
        #endregion

        #region ESG
        Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao);
        Task<PayloadDTO> ConsultarClassificacaoEsg();
        Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro);
        #endregion
    }
}
