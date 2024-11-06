using DTO.Payload;
using MGI.ClassificacaoContabil.Service.Helper;
using Service.DTO.Cenario;
using Service.DTO.Filtros;
using Service.Interface.Cenario;
using Service.Repository.Cenario;

namespace Service.Cenario
{
    public class CenarioService : ICenarioService
    {
        private ICenarioRepository _repository;
        private ITransactionHelper _transactionHelper;

        public CenarioService(ICenarioRepository cenarioRepository, ITransactionHelper transactionHelper)
        {
            _repository = cenarioRepository;
            _transactionHelper = transactionHelper;
        }
        public async Task<PayloadDTO> InserirCenario(CenarioDTO cenario)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _repository.InserirCenario(cenario),
                "Cenário criado com successo"
            );
        }
        public async Task<PayloadDTO> AlterarCenario(CenarioDTO cenario)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _repository.AlterarCenario(cenario),
                "Cenário alterado com successo"
            );
        }
        public async Task<PayloadDTO> ConsultarCenario()
        {
            var resultado = await _repository.ConsultarCenario();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarCenario(CenarioFiltro filtro)
        {
            var resultado = await _repository.ConsultarCenario(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
    }
}
