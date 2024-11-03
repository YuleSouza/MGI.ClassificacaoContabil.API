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

        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = await ValidarParametrizacaoClassificacaoExcecao(parametrizacao);
                    if (!validacao.Sucesso) return validacao;                    
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
            var excecoes = await ConsultarParametrizacaoClassificacaoExcecao();
            excecoes.ObjetoRetorno = excecoes.ObjetoRetorno?.Where(p => p.IdCenario == parametrizacao.IdCenario && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg);
            if (parametrizacao.IdGrupoPrograma > 0)
            {
                excecoes.ObjetoRetorno = excecoes.ObjetoRetorno.Where(p => p.IdGrupoPrograma == parametrizacao.IdGrupoPrograma);
            }
            if (parametrizacao.IdPrograma > 0)
            {
                excecoes.ObjetoRetorno = excecoes.ObjetoRetorno.Where(p => p.IdPrograma == parametrizacao.IdPrograma);
            }
            if (parametrizacao.IdProjeto > 0)
            {
                excecoes.ObjetoRetorno = excecoes.ObjetoRetorno.Where(p => p.IdProjeto == parametrizacao.IdProjeto);
            }
            if (parametrizacao.IdEmpresa > 0) 
            {
                excecoes.ObjetoRetorno = excecoes.ObjetoRetorno.Where(p => p.IdEmpresa == parametrizacao.IdEmpresa);
            }
            bool registroExistente = excecoes.ObjetoRetorno.Any();
            if (registroExistente) 
            {
                payloadDTO = new PayloadDTO("Já existe um cadastro de exceção com essas informações, favor escolher uma diferente !", false, string.Empty);
            }
            return await Task.FromResult(payloadDTO);
        }
        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            using (IUnitOfWork unitOfWork = _unitOfWork)
            {
                try
                {
                    var validacao = await ValidarParametrizacaoClassificacaoExcecao(parametrizacao);
                    if (!validacao.Sucesso) return validacao;
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
    }
}
