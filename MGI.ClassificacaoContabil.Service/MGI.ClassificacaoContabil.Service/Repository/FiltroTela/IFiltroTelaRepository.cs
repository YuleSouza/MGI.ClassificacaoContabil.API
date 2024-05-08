using Service.DTO.Filtros;
using Service.DTO.Empresa;
using Service.DTO.Projeto;
using Service.DTO.Classificacao;

namespace Service.Repository.FiltroTela
{
    public interface IFiltroTelaRepository
    {
        Task<IEnumerable<EmpresaDTO>> EmpresaClassificacaoContabil(FiltroEmpresa filtro);
        Task<IEnumerable<ProjetoDTO>> ProjetoClassificacaoContabil(FiltroProjeto filtro);
        Task<IEnumerable<DiretoriaDTO>> DiretoriaClassificacaoContabil(FiltroDiretoria filtro);
        Task<IEnumerable<GerenciaDTO>> GerenciaClassificacaoContabil(FiltroGerencia filtro);
        Task<IEnumerable<GestorDTO>> GestorClassificacaoContabil(FiltroGestor filtro);
        Task<IEnumerable<GrupoProgramaDTO>> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro);
        Task<IEnumerable<ProgramaDTO>> ProgramaClassificacaoContabil(FiltroPrograma filtro);
        Task<IEnumerable<ClassificacaoContabilFiltroDTO>> ClassificacaoContabil();
        Task<IEnumerable<ClassificacaoEsgFiltroDTO>> ClassificacaoEsg();
    }
}
