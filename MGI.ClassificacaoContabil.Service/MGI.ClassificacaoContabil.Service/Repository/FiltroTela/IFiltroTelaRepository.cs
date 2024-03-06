using Service.DTO.Empresa;
using Service.DTO.Filtros;
using Service.DTO.Projeto;

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
    }
}
