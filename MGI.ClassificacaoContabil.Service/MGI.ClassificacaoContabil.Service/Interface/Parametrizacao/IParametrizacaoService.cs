using DTO.Payload;
using Service.DTO.Filtros;
using Service.DTO.Parametrizacao;


namespace Service.Interface.Parametrizacao
{
    public interface IParametrizacaoService
    {
        Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao);
        Task<PayloadDTO> ConsultarParametrizacaoCenario();
    }
}
