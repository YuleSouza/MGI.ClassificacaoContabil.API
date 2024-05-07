using Service.DTO.Filtros;
using Service.DTO.Empresa;
using Service.DTO.Projeto;
using Service.DTO.Cenario;
using Service.DTO.Classificacao;
using Service.DTO.Parametrizacao;

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
        Task<IEnumerable<ClassificacaoContabilDTO>>FiltroPainelClassificacaoContabil(FiltroPainelClassificacaoContabil filtro);
        Task<IEnumerable<ClassificacaoEsgDTO>>FiltroPainelClassificacaoESG(FiltroPainelClassificacaoEsg filtro);
        #endregion
    }
}
