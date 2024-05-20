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
        Task<IEnumerable<Service.DTO.Classificacao.ClassificacaoContabilDTO>>FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro);
        Task<IEnumerable<ClassificacaoEsgDTO>>FiltroPainelClassificacaoESG(FiltroPainelClassificacaoEsg filtro);
        #endregion

        #region [ Consulta ]
        Task<IEnumerable<ClassificacaoContabilItemDTO>> ConsultarClassificacaoContabil(FiltroPainelClassificacaoContabil filtro);

        Task<IEnumerable<ClassificacaoContabilItemDTO>> GerarRelatorioContabil(FiltroPainelClassificacaoContabil filtro);
        Task<IEnumerable<LancamentoClassificacaoEsgDTO>> ConsultarClassificacaoEsg(FiltroPainelClassificacaoEsg filtro);
        #endregion
    }
}
