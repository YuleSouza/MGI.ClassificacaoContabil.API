using DTO.Payload;
using Infra.Interface;
using Service.Helper;
using Service.DTO.Classificacao;
using Service.DTO.Filtros;
using Service.Interface.Classificacao;
using Service.Repository.Classificacao;

namespace Service.Classificacao
{
    public class ClassificacaoEsgService : IClassificacaoEsgService
    {
        private IClassificacaoRepository _repository;
        private IUnitOfWork _unitOfWork;
        private readonly ITransactionHelper _transactionHelper;

        public ClassificacaoEsgService(IClassificacaoRepository classificacaoRepository, ITransactionHelper transactionHelper)
        {
            _repository = classificacaoRepository;
            _transactionHelper = transactionHelper;
        }

        

        #region ESG
        public async Task<PayloadDTO> InserirClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
                async () => await _repository.InserirClassificacaoEsg(classificacao)
            , "Classificação Esg inserida com successo");
        }
        public async Task<PayloadDTO> AlterarClassificacaoEsg(ClassificacaoEsgDTO classificacao)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(
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
