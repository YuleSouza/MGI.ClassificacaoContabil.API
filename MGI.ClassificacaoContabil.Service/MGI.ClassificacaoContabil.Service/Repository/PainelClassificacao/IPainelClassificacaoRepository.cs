﻿using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.Contabil;
using MGI.ClassificacaoContabil.Service.DTO.PainelClassificacao.ESG;
using Service.DTO.Classificacao;
using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.PainelClassificacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Projeto;

namespace Service.Repository.PainelClassificacao
{
    public interface IPainelClassificacaoRepository
    {
        #region [Filtros]
        Task<IEnumerable<EmpresaDTO>>FiltroPainelEmpresa(FiltroPainelEmpresa filtro);
        Task<IEnumerable<GrupoProgramaDTO>>FiltroPainelGrupoPrograma(FiltroPainelGrupoPrograma filtro);
        Task<IEnumerable<ProgramaDTO>>FiltroPainelPrograma(FiltroPainelPrograma filtro);
        Task<IEnumerable<ProjetoDTO>>FiltroPainelProjeto(FiltroPainelProjeto filtro);
        Task<IEnumerable<GestorDTO>>FiltroPainelGestor(FiltroPainelGestor filtro);
        Task<IEnumerable<DiretoriaDTO>>FiltroPainelDiretoria(FiltroPainelDiretoria filtro);
        Task<IEnumerable<GerenciaDTO>>FiltroPainelGerencia(FiltroPainelGerencia filtro);
        Task<IEnumerable<ParametrizacaoCenarioPainelDTO>> FiltroPainelCenario(FiltroPainelCenario filtro);
        Task<IEnumerable<Service.DTO.Classificacao.ClassificacaoContabilDTO>>FiltroPainelClassificacaoContabil(FiltroLancamentoFase filtro);
        Task<IEnumerable<ClassificacaoEsgDTO>>FiltroPainelClassificacaoESG(FiltroPainelClassificacaoEsg filtro);
        #endregion

        #region [ Consulta ]
        Task<IEnumerable<ClassificacaoContabilItemDTO>> ConsultarClassificacaoContabil(FiltroLancamentoFase filtro);
        Task<IEnumerable<RelatorioContabilDTO>> ConsultarDadosRelatorio(FiltroLancamentoFase filtro);
        Task<IEnumerable<RelatorioEsgDTO>> ConsultarDadosRelatorio(FiltroPainelClassificacaoEsg filtro);
        Task<IEnumerable<LancamentoClassificacaoEsgDTO>> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro);
        Task<IEnumerable<LancamentoFaseContabilDTO>> ConsultarLancamentosDaFase(FiltroLancamentoFase filtro);
        Task<IEnumerable<LancamentoSAP>> ConsultarLancamentoSap(FiltroLancamentoSap filtro);
        Task<LancamentoClassificacaoEsgDTO> ConsultarClassifEsgPorProjeto(int idProjeto, int seqFase, int idEmpresa);
        
        #endregion
    }
}
