using Service.DTO.Esg;

namespace Service.Repository.Esg
{
    public interface IEsgAprovadorRepository
    {
        Task<EsgAprovadorDTO> ConsultarAprovadorPorUsuario(string usuario);
        Task<bool> InserirUsuarioAprovador(string usuario, string email);
        Task<bool> RemoverUsuarioAprovador(int id);
        Task<bool> AlterarUsuarioAprovador(string email, int id);
    }
}
