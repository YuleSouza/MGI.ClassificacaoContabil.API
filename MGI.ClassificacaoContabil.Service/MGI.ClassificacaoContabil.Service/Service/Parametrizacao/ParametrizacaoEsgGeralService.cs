using DTO.Payload;
using Infra.Interface;
using Service.Interface.Parametrizacao;
using Service.DTO.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Service.Parametrizacao
{
    public class ParametrizacaoEsgGeralService : IParametrizacaoEsgGeralService
    {
        private IParametrizacaoRepository _repository;
        private IUnitOfWork _unitOfWork;
        public ParametrizacaoEsgGeralService(IParametrizacaoRepository cenarioRepository, IUnitOfWork unitOfWork)
        {
            _repository = cenarioRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = await ValidarParametrizacaoClassificacaoGeral(parametrizacao);
                    if (!validacao.Sucesso) return validacao;
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarParametrizacaoClassificacaoGeral(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização da classificação esg geral alterada com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração parametrização classificação esg geral ", false, ex.Message);
                }
            }
        }
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoGeralDTO>>> ConsultarParametrizacaoClassificacaoGeral()
        {
            var resultado = await _repository.ConsultarParametrizacaoClassificacaoGeral();
            return new PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoGeralDTO>>(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = await ValidarParametrizacaoClassificacaoGeral(parametrizacao);
                    if (!validacao.Sucesso) return validacao;
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirParametrizacaoClassificacaoGeral(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização classificação esg geral inserida com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir parametrização classificação esg geral ", false, ex.Message);
                }
            }
        }
        private async Task<PayloadDTO> ValidarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            PayloadDTO payloadDTO = new PayloadDTO(string.Empty, true);
            if (parametrizacao.IdGrupoPrograma == 0)
            {
                payloadDTO = new PayloadDTO("Obrigatório o envio do grupo de programa", false);
            }
            var parametrosEsgGEral = await _repository.ConsultarParametrizacaoClassificacaoGeral();
            bool registroExistente = parametrosEsgGEral.Any(p => p.IdGrupoPrograma == parametrizacao.IdGrupoPrograma && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg);
            if (registroExistente)
            {
                payloadDTO = new PayloadDTO("Parametrização geral Esg já inserida!", false);
            }
            return await Task.FromResult(payloadDTO);
        }
    }
}
