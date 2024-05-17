﻿using DTO.Payload;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using Service.DTO.Empresa;
using Service.DTO.Filtros;

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
        Task<IList<EmpresaDTO>> ConsultarClassificacaoContabil();
        #endregion

        #region [ESG]

        Task<PainelClassificacaoEsg> ConsultarClassificacaoEsg();
        #endregion
    }
}
