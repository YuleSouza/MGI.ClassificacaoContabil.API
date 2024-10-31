using DTO.Payload;
using Infra.Interface;
using Service.DTO.Parametrizacao;
using Service.Interface.Parametrizacao;
using Service.Repository.Parametrizacao;

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

        #region Parametrização Cenário
        public async Task<PayloadDTO> InserirParametrizacaoCenario(ParametrizacaoCenarioDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {

                    unitOfWork.BeginTransaction();
                    var parametrizacaoCenarios = await _repository.ConsultarParametrizacaoCenario();
                    bool registroExistente = parametrizacaoCenarios.Any(p => p.IdCenario == parametrizacao.IdCenario 
                                                                && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg 
                                                                && p.IdClassificacaoContabil == parametrizacao.IdClassificacaoContabil);
                    if (registroExistente) {
                        return new PayloadDTO("Cenário, Classificação ESG e Classificação contábil já cadastrados!", false);
                    }
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
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoCenarioDTO>>> ConsultarParametrizacaoCenario()
        {
            var resultado = await _repository.ConsultarParametrizacaoCenario();
            return new PayloadGeneric<IEnumerable<ParametrizacaoCenarioDTO>>(string.Empty, true, string.Empty, resultado);
        }
        #endregion

        #region Parametrização Classificacação ESG Geral
        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    var parametrosEsgGEral = await _repository.ConsultarParametrizacaoClassificacaoGeral();
                    bool registroExistente = parametrosEsgGEral.Any(p => p.IdGrupoPrograma == parametrizacao.IdGrupoPrograma && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg);
                    if (registroExistente)
                    {
                        return new PayloadDTO("Parametrização geral Esg já inserida!", false);
                    }
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
        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    if (parametrizacao.IdGrupoPrograma == 0)
                    {
                        return new PayloadDTO("Obrigatório o envio do grupo de programa", false);
                    }
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
        #endregion

        #region Parametrização Classificacação ESG Exceção
        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = ValidarParametrizacaoClassificacaoExcecao(parametrizacao);
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.InserirParametrizacaoClassificacaoExcecao(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização classificação esg exceção inserido com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro ao inserir parametrização classificação esg exceção ", false, ex.Message);
                }
            }
        }

        private async Task<PayloadDTO> ValidarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            PayloadDTO payloadDTO = new PayloadDTO(string.Empty, true);
            if (parametrizacao.IdClassificacaoEsg <= 0)
            {
                payloadDTO = new PayloadDTO("Obrigatóio o envio da classificação Esg !", false, string.Empty);
            }
            if (parametrizacao.IdCenario <= 0)
            {
                payloadDTO = new PayloadDTO("Obrigatóio o envio do cenário !", false, string.Empty);
            }
            return await Task.FromResult(payloadDTO);
        }

        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    unitOfWork.BeginTransaction();
                    bool ok = await _repository.AlterarParametrizacaoClassificacaoExcecao(parametrizacao);
                    unitOfWork.Commit();
                    return new PayloadDTO("Parametrização da classificação esg exceção alterada com successo", ok, string.Empty);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new PayloadDTO("Erro na alteração parametrização classificação esg exceção ", false, ex.Message);
                }
            }
        }
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>>> ConsultarParametrizacaoClassificacaoExcecao()
        {
            var resultado = await _repository.ConsultarParametrizacaoClassificacaoExcecao();
            return new PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>>(string.Empty, true, string.Empty, resultado);
        }
        #endregion
    }
}
