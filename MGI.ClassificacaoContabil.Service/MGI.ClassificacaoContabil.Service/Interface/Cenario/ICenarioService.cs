using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Cenario;

namespace Service.Interface.Cenario
{
    public interface ICenarioService
    {
        Task<PayloadDTO> InserirCenario(CenarioDTO classificacao);
        Task<PayloadDTO> AlterarCenario(CenarioDTO classificacao);
        Task<PayloadDTO> ConsultarCenario();
        Task<PayloadDTO> ConsultarCenario(CenarioFiltro filtro);
    }
}
