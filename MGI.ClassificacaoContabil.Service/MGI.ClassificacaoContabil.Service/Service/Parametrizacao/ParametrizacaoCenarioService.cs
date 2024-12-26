using DTO.Payload;
using Service.Base;
using Service.DTO.Parametrizacao;
using Service.Helper;
using Service.Interface.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Service.Parametrizacao
{
    public class ParametrizacaoCenarioService : ServiceBase, IParametrizacaoCenarioService
    {
        private IParametrizacaoRepository _repository;
        public ParametrizacaoCenarioService(IParametrizacaoRepository cenarioRepository, ITransactionHelper transactionHelper) : base (transactionHelper)
        {
            _repository = cenarioRepository;            
        }
        public async Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await ExecutarTransacao(
                async () => await _repository.InserirParametrizacaoCenario(parametrizacao),
                "Parametrização classificação esg geral inserida com successo"
            );
        }
        public async Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await ExecutarTransacao(
                async () => await _repository.AlterarParametrizacaoCenario(parametrizacao),
                "Parametrização do cenário alterado com successo"
            );
        }
        private async Task<PayloadDTO> Validar(ParametrizacaoCenarioDTO parametrizacao)
        {
            if (parametrizacao.IdCenario == 0 || parametrizacao.IdClassificacaoContabil == 0 || parametrizacao.IdClassificacaoEsg == 0)
            {
                return new PayloadDTO("Obrigatório Cenário, Classificação Contábil e Classificação ESG são obrigatórios", false);
            }
            PayloadDTO payloadDTO = new PayloadDTO(string.Empty, true);
            var parametrizacaoCenarios = await _repository.ConsultarParametrizacaoCenario();
            bool registroExistente = parametrizacaoCenarios.Any(p => p.IdCenario == parametrizacao.IdCenario
                                                        && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg
                                                        && p.IdClassificacaoContabil == parametrizacao.IdClassificacaoContabil
                                                        && p.Status == parametrizacao.Status);
            if (registroExistente)
            {
                payloadDTO = new PayloadDTO("Cenário, Classificação ESG e Classificação contábil já cadastrados!", false);
            }
            return await Task.FromResult(payloadDTO);
        }
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoCenarioDTO>>> ConsultarParametrizacaoCenario()
        {
            var resultado = await _repository.ConsultarParametrizacaoCenario();
            return new PayloadGeneric<IEnumerable<ParametrizacaoCenarioDTO>>(string.Empty, true, string.Empty, resultado);
        }
    }
}
