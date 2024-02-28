using Service.DTO.Parametrizacao;
using Service.DTO.Filtros;

namespace Service.Repository.Parametrizacao
{
    public interface IParametrizacaoRepository
    {

        Task<bool> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<bool> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<IEnumerable<ParametrizacaoCenarioDTO>> ConsultarParametrizacaoCenario();
    }
}
