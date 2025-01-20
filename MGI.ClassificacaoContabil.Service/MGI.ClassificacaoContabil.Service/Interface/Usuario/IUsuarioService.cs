using Service.DTO;

namespace Service.Interface.Usuario
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ConsultarUsuarioPorLogin(string login);
        Task<bool> EhUmUsuarioSustentabilidade(string login);
    }
}
