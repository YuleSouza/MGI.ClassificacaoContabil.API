using DTO.Payload;
using Service.DTO.Filtros;

namespace Service.Interface.FiltroTela
{
    public interface IFiltroTelaService
    {
        Task<PayloadDTO> EmpresaClassificacaoContabil(FiltroEmpresa filtro);
        Task<PayloadDTO> ProjetoClassificacaoContabil(FiltroProjeto filtro);
        Task<PayloadDTO> DiretoriaClassificacaoContabil(FiltroDiretoria filtro);
        Task<PayloadDTO> GerenciaClassificacaoContabil(FiltroGerencia filtro);
        Task<PayloadDTO> GestorClassificacaoContabil(FiltroGestor filtro);
        Task<PayloadDTO> GrupoProgramaClassificacaoContabil(FiltroGrupoPrograma filtro);
        Task<PayloadDTO> ProgramaClassificacaoContabil(FiltroPrograma filtro);
        Task<PayloadDTO> ClassificacaoContabil(); 
        Task<PayloadDTO> ClassificacaoEsg();
        Task<PayloadDTO> Cenario();
        Task<PayloadDTO> ConsultarCoordenadoria(FiltroCoordenadoria filtro);
    }
}
