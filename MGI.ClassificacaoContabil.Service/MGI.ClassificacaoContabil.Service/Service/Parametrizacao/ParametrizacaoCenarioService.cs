using DTO.Payload;
using Infra.Interface;
using Service.Interface.Parametrizacao;
using Service.DTO.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Service.Parametrizacao
{
    public class ParametrizacaoCenarioService : IParametrizacaoCenarioService
    {
        private IParametrizacaoRepository _repository;
        private IUnitOfWork _unitOfWork;
        public ParametrizacaoCenarioService(IParametrizacaoRepository cenarioRepository, IUnitOfWork unitOfWork)
        {
            _repository = cenarioRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = await ValidarParametrizacaoCenario(parametrizacao);
                    if (!validacao.Sucesso) return validacao;
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarParametrizacaoCenario(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização do cenário alterado com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração parametrização cenário ", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {

                    var validacao = await ValidarParametrizacaoCenario(parametrizacao);
                    if (!validacao.Sucesso) return validacao;
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirParametrizacaoCenario(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização cenário inserido com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir parametrização cenário ", false, ex.Message);
                }
            }
        }
        private async Task<PayloadDTO> ValidarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
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
