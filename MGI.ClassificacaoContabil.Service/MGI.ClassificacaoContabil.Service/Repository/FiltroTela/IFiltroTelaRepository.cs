using Service.DTO.Filtros;
using Service.DTO.Combos;

namespace Service.Repository.FiltroTela
{
    public interface IFiltroTelaRepository
    {
        Task<IEnumerable<PayloadComboDTO>> ConsultarEmpresaClassifContabil(FiltroEmpresa filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarProjetoClassifContabil(FiltroProjeto filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarDiretoriaClassifiContabil(FiltroDiretoria filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarGerenciaClassifContabil(FiltroGerencia filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarGestorClassifContabil(FiltroGestor filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarGrupoProgramaClassifContabil(FiltroGrupoPrograma filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarProgramaClassifContabil(FiltroPrograma filtro);
        Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoContabil();
        Task<IEnumerable<PayloadComboDTO>> ConsultarClassificacaoEsg();
        Task<IEnumerable<PayloadComboDTO>> ConsultarCenario();
        Task<IEnumerable<PayloadComboDTO>> ConsultarCoordenadoria(FiltroCoordenadoria filtro);
    }
}
