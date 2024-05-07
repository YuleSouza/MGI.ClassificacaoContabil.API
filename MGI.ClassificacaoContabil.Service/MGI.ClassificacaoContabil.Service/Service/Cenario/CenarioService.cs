using DTO.Payload;
using Infra.Interface;
using Service.DTO.Cenario;
using Service.DTO.Filtros;
using Service.Interface.Cenario;
using Service.Repository.Cenario;

namespace Service.Cenario
{
    public class CenarioService : ICenarioService
    {
        private ICenarioRepository _repository;
        private IUnitOfWork _unitOfWork;

        public CenarioService(ICenarioRepository cenarioRepository, IUnitOfWork unitOfWork)
        {
            _repository = cenarioRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PayloadDTO> InserirCenario(CenarioDTO cenario)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirCenario(cenario);
                    unitOfWork.Commit();
                    return new PayloadDTO("Cenário inserido com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir Cenário", false, ex.Message);
                }
            }
        }
        public async Task<PayloadDTO> AlterarCenario(CenarioDTO cenario)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarCenario(cenario);
                    unitOfWork.Commit();
                    return new PayloadDTO("Cenário alterado com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração Cenário", false, ex.Message);
                }
            }
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
