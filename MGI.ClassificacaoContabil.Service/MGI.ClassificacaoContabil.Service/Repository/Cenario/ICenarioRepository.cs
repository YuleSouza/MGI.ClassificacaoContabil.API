using Service.DTO.Cenario;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;

namespace Service.Repository.Cenario
{
    public interface ICenarioRepository
    {
        Task<bool> InserirCenario(CenarioDTO classificacao);
        Task<bool> AlterarCenario(CenarioDTO classificacao);
        Task<IEnumerable<CenarioDTO>> ConsultarCenario(CenarioFiltro filtro);
    }
}
