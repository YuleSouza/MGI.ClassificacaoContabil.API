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

        public async Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuarioAprovador(string usuario, string email)
        {
            return await _repository.ConsultarUsuarioAprovador(usuario, email);
        }

        public async Task<PayloadDTO> InserirUsuarioAprovador(string usuario, string email)
        {
            var validacao = await ValidarUsuarioAprovador(usuario, email);
            if (!validacao.Sucesso) return validacao;
            return await _transactionHelper.ExecuteInTransactionAsync(async () =>
                await _repository.InserirUsuarioAprovador(usuario, email)
                , "Usuário aprovador inserido com sucesso!"
            );
        }

        public async Task<PayloadDTO> ValidarUsuarioAprovador(string usuario, string email)
        {
            if (!UtilsService.EmailValido(email))
            {
                return new(string.Empty, false, "E-mail inválido");
            }

            var payloadDTO = new PayloadDTO(string.Empty, true, string.Empty);
            if (await UsuarioJaExiste(usuario, email))
            {
                return new PayloadDTO(string.Empty, false, "Usuário já existe");
            }
            if (await EmailJaExiste(email))
            {
                return new PayloadDTO(string.Empty, false, "E-mail já existe");
            }
            return payloadDTO;
        }        
        private async Task<bool> UsuarioJaExiste(string usuario, string email)
        {
            var usuarioAprovador = await ConsultarUsuarioAprovador(usuario, string.Empty);
            return usuarioAprovador.Any();
        }
        private async Task<bool> EmailJaExiste(string email)
        {
            var usuarioAprovador = await ConsultarUsuarioAprovador(string.Empty, email);
            return usuarioAprovador.Any();
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
