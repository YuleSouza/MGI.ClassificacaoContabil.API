using DTO.Payload;
using Infra.Interface;
using Service.Helper;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;
using Service.Base;

namespace Service.Classificacao
{
    public class ClassificacaoEsgService : ServiceBase, IClassificacaoEsgService
    {
        private IClassificacaoRepository _repository;
        public ClassificacaoEsgService(IClassificacaoRepository classificacaoRepository, ITransactionHelper transactionHelper) : base(transactionHelper)
        {
            _repository = classificacaoRepository;            
        }

        #region ESG
        public async Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            return await ExecutarTransacao(
                async () => await _repository.InserirClassificacaoEsg(classificacao)
            , "Classificação Esg inserida com successo");
        }
        public async Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            return await ExecutarTransacao(
                async () => await _repository.AlterarClassificacaoEsg(classificacao)
            , "Classificação Esg alterada com successo");
        }
        public async Task<PayloadDTO> ConsultarClassificacaoEsg()
        {
            var resultado = await _repository.ConsultarClassificacaoEsg();
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }
        public async Task<PayloadDTO> ConsultarClassificacaoEsg(ClassificacaoEsgFiltro filtro)
        {
            var resultado = await _repository.ConsultarClassificacaoEsg(filtro);
            return new PayloadDTO(string.Empty, true, string.Empty, resultado);
        }

        


        #endregion
    }
}
