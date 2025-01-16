using Service.DTO;

namespace Service.Repository.Usuario
{
    public interface IUsuarioRepository
    {
        Task<UsuarioDTO> ConsultarUsuarioPorLogin(string login);
    }
}
