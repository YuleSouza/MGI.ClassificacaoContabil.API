using Service.DTO;

namespace Service.Interface.Usuario
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> ConsultarUsuarioPorLogin(string login);
        Task<bool> EhUmUsuarioSustentabilidade(string login);
    }
}
