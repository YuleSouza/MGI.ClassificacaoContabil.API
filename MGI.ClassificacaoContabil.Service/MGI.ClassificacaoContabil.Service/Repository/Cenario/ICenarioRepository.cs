using Service.DTO.Filtros;
using Service.DTO.Cenario;

namespace Service.Repository.Cenario
{
    public interface ICenarioRepository
    {
        Task<bool> InserirCenario(CenarioDTO classificacao);
        Task<bool> AlterarCenario(CenarioDTO classificacao);
        Task<IEnumerable<CenarioDTO>> ConsultarCenario();
        Task<IEnumerable<CenarioDTO>> ConsultarCenario(CenarioFiltro filtro);
    }
}
