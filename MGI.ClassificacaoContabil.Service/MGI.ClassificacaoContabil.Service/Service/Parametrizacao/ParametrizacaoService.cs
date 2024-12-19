using DTO.Payload;
using Infra.Interface;
using Service.Helper;
using Service.DTO.Parametrizacao;
using Service.Interface.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Service.Parametrizacao
{
    public class ParametrizacaoService : IParametrizacaoService
    {
        private IParametrizacaoRepository _repository;
        private ITransactionHelper _transactionHelper;

        public ParametrizacaoService(IParametrizacaoRepository cenarioRepository, ITransactionHelper transactionHelper)
        {
            _repository = cenarioRepository;
            _transactionHelper = transactionHelper;
        }

        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _repository.InserirParametrizacaoClassificacaoExcecao(parametrizacao),
                "Parametrização classificação esg exceção inserido com successo"
            );
        }
        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoExcecao(ParametrizacaoClassificacaoEsgDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _repository.AlterarParametrizacaoClassificacaoExcecao(parametrizacao),
                "Parametrização da classificação esg exceção alterada com successo"
            );
        }
        private async Task<PayloadDTO> Validar(ParametrizacaoClassificacaoEsgDTO parametrizacao)
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
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>>> ConsultarParametrizacaoClassificacaoExcecao()
        {
            var resultado = await _repository.ConsultarParametrizacaoClassificacaoExcecao();
            return new PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoEsgFiltroDTO>>(string.Empty, true, string.Empty, resultado);
        }
    }
}
