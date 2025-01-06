using DTO.Payload;
using Service.DTO.Filtros;

namespace Service.Interface.FiltroTela
{
    public interface IFiltroTelaService
    {
        Task<PayloadDTO> ConsultarEmpresa(FiltroEmpresa filtro);
        Task<PayloadDTO> ConsultarProjeto(FiltroProjeto filtro);
        Task<PayloadDTO> ConsultarDiretoria(FiltroDiretoria filtro);
        Task<PayloadDTO> ConsultarGerencia(FiltroGerencia filtro);
        Task<PayloadDTO> ConsultarGestor(FiltroGestor filtro);
        Task<PayloadDTO> ConsultarGrupoPrograma(FiltroGrupoPrograma filtro);
        Task<PayloadDTO> ConsultarPrograma(FiltroPrograma filtro);
        Task<PayloadDTO> ConsultarClassificacaoContabil(); 
        Task<PayloadDTO> ConsultarClassificacaoEsg();
        Task<PayloadDTO> ConsultarCenario();
        Task<PayloadDTO> ConsultarCoordenadoria(FiltroCoordenadoria filtro);
    }
}
