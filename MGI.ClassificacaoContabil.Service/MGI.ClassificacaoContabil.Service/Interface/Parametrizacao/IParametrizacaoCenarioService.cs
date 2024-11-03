using DTO.Payload;
using Service.DTO.Parametrizacao;

namespace Service.Interface.Parametrizacao
{
    public interface IParametrizacaoCenarioService
    {
        Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadGeneric<IEnumerable<ParametrizacaoCenarioDTO>>> ConsultarParametrizacaoCenario();
    }
}
