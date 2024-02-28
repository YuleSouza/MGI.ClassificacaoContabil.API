using DTO.Payload;
using Infra.Interface;
using Service.Interface.Parametrizacao;
using Service.Repository.Parametrizacao;
using Service.DTO.Parametrizacao;
using Service.DTO.Filtros;

namespace Service.Parametrizacao
{
    public class ParametrizacaoService : IParametrizacaoService
    {
        private IParametrizacaoRepository _repository;
        private IUnitOfWork _unitOfWork;

        public ParametrizacaoService(IParametrizacaoRepository cenarioRepository, IUnitOfWork unitOfWork)
        {
            _repository = cenarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
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
        public async Task<PayloadDTO> AlterarParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
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
        public async Task<PayloadDTO> ConsultarParametrizacaoCenario()
        {
            var resultado = await _repository.ConsultarParametrizacaoCenario();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
    }
}
