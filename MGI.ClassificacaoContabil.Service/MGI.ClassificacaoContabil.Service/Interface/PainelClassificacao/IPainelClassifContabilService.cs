using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;

namespace Service.Interface.PainelClassificacao
{
    public interface IPainelClassifContabilService
    {
        #region [Filtros]
        Task<PayloadDTO> FiltroPainelEmpresa(FiltroPainelEmpresa filtro);
        Task<PayloadDTO> FiltroPainelProjeto(FiltroPainelProjeto filtro);
        Task<PayloadDTO> FiltroPainelDiretoria(FiltroPainelDiretoria filtro);
        Task<PayloadDTO> FiltroPainelGerencia(FiltroPainelGerencia filtro);
        Task<PayloadDTO> FiltroPainelGestor(FiltroPainelGestor filtro);
        Task<PayloadDTO> FiltroPainelGrupoPrograma(FiltroPainelGrupoPrograma filtro);
        Task<PayloadDTO> FiltroPainelPrograma(FiltroPainelPrograma filtro); 
        Task<PayloadDTO> FiltroPainelCenario(FiltroPainelCenario filtro);
        Task<PayloadDTO> FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro);
        Task<PayloadDTO> FiltroPainelClassificacaoEsg(FiltroPainelClassificacaoEsg filtro);
        #endregion

        #region [Contabil]
        Task<PainelClassificacaoContabilDTO> ConsultarClassificacaoContabil(FiltroPainelClassificacaoContabil filtro);

        Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro);
        Task<byte[]> GerarRelatorioContabil(FiltroPainelClassificacaoContabil filtro);
        #endregion
    }
}
