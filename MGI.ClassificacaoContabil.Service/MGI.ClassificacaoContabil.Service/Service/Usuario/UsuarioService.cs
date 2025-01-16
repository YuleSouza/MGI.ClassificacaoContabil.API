using Service.DTO;
using Service.Interface.Usuario;
using Service.Repository.Esg;
using Service.Repository.Usuario;

namespace Service.Usuario
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IEsgAprovadorRepository _esgAprovadorRepository;
        public UsuarioService(IUsuarioRepository repository, IEsgAprovadorRepository esgAprovadorRepository)
        {
            _repository = repository;
            _esgAprovadorRepository = esgAprovadorRepository;
        }
        public async Task<UsuarioDTO> ConsultarUsuarioPorLogin(string login)
        {
            if (!string.IsNullOrEmpty(login))
            {
                return await _repository.ConsultarUsuarioPorLogin(login.ToUpper());
            }
            return new UsuarioDTO();
        }
        
        public async Task<bool> EhUmUsuarioSustentabilidade(string login)
        {
            var usuarios = await _esgAprovadorRepository.ConsultarUsuariosSustentabilidade();
            return usuarios.Any(p => p.Usuario.Trim() == login);
        }
    }
}
