using DTO.Payload;
using Service.DTO.Esg;
using Service.Helper;
using Service.Interface.PainelEsg;
using Service.Repository.Esg;

namespace Service.Esg
{
    public class EsgAprovadorService : IEsgAprovadorService
    {
        private readonly IEsgAprovadorRepository _repository;
        private ITransactionHelper _transactionHelper;

        public EsgAprovadorService(IEsgAprovadorRepository esgAprovadorRepository, ITransactionHelper transactionHelper)
        {
            _repository = esgAprovadorRepository;
            _transactionHelper = transactionHelper;
        }

        public async Task<PayloadDTO> AlterarUsuarioAprovador(string email, int id)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(async () =>
                await _repository.AlterarUsuarioAprovador(email, id)
                ,"Usuário aprovador alterado com sucesso!"
            );
        }

        public async Task<EsgAprovadorDTO> ConsultarAprovadorPorUsuario(string usuario)
        {
            return await _repository.ConsultarAprovadorPorUsuario(usuario);
        }

        public async Task<PayloadDTO> InserirUsuarioAprovador(string usuario, string email)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(async () =>
                await _repository.InserirUsuarioAprovador(usuario, email)
                , "Usuário aprovador removido com sucesso!"
            );
        }

        public async Task<PayloadDTO> ExcluirUsuarioAprovador(int id)
        {
            return await _transactionHelper.ExecuteInTransactionAsync(async () =>
                await _repository.RemoverUsuarioAprovador(id)
                , "Usuário aprovador removido com sucesso!"
            );
        }
    }
}
