using DTO.Payload;
using Service.Base;
using Service.DTO.Parametrizacao;
using Service.Helper;
using Service.Interface.Parametrizacao;
using Service.Repository.Parametrizacao;

namespace Service.Parametrizacao
{
    public class ParametrizacaoEsgGeralService : ServiceBase, IParametrizacaoEsgGeralService
    {
        private IParametrizacaoRepository _repository;        
        public ParametrizacaoEsgGeralService(IParametrizacaoRepository cenarioRepository, ITransactionHelper transactionHelper) : base (transactionHelper)
        {
            _repository = cenarioRepository;
        }
        public async Task<PayloadDTO> InserirParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await ExecutarTransacao(
                async () => await _repository.InserirParametrizacaoClassificacaoGeral(parametrizacao),
                "Parametrização classificação esg geral inserida com successo"
            );
        }
        public async Task<PayloadDTO> AlterarParametrizacaoClassificacaoGeral(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            var validacao = await Validar(parametrizacao);
            if (!validacao.Sucesso) return validacao;

            return await ExecutarTransacao(
                async () => await _repository.AlterarParametrizacaoClassificacaoGeral(parametrizacao),
                "Parametrização da classificação esg geral alterada com successo"
            );
        }
        private async Task<PayloadDTO> Validar(ParametrizacaoClassificacaoGeralDTO parametrizacao)
        {
            PayloadDTO payloadDTO = new PayloadDTO(string.Empty, true);
            if (parametrizacao.IdGrupoPrograma == 0 || parametrizacao.IdClassificacaoEsg == 0)
            {
                payloadDTO = new PayloadDTO("Obrigatório o envio do Grupo de Programa e Classificação ESG", false);
            }
            var parametrosEsgGEral = await _repository.ConsultarParametrizacaoClassificacaoGeral();
            bool registroExistente = parametrosEsgGEral.Any(p => p.IdGrupoPrograma == parametrizacao.IdGrupoPrograma && p.IdClassificacaoEsg == parametrizacao.IdClassificacaoEsg);
            if (registroExistente)
            {
                payloadDTO = new PayloadDTO("Parametrização geral Esg já inserida!", false);
            }
            return await Task.FromResult(payloadDTO);
        }
        public async Task<PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoGeralDTO>>> ConsultarParametrizacaoClassificacaoGeral()
        {
            var resultado = await _repository.ConsultarParametrizacaoClassificacaoGeral();
            return new PayloadGeneric<IEnumerable<ParametrizacaoClassificacaoGeralDTO>>(string.Empty, true, string.Empty, resultado);
        }
    }
}
