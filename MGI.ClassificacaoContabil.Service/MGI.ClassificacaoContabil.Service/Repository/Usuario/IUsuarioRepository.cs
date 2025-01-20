using Service.DTO;

namespace Service.Repository.Usuario
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<UsuarioDTO>> ConsultarUsuarioPorLogin(string login);
    }
}
