using DTO.Payload;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;

namespace Service.Interface.PainelClassificacao
{
    public interface IPainelClassificacaoService
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
        Task<byte[]> GerarRelatorioContabilg(FiltroPainelClassificacaoContabil filtro);

        Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro);
        #endregion

        #region [ESG]

        Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro);

        Task<byte[]> GerarRelatorioContabilEsg(FiltroPainelClassificacaoEsg filtro);
        byte[] GerarExcel<T>(IEnumerable<T> data);
        #endregion
    }
}
