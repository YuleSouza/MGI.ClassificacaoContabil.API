using Service.DTO.Esg;

namespace Service.Repository.Esg
{
    public interface IEsgAprovadorRepository
    {
        Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuarioAprovador(string usuario, string email);
        Task<bool> InserirUsuarioAprovador(string usuario, string email);
        Task<bool> RemoverUsuarioAprovador(int id);
        Task<bool> AlterarUsuarioAprovador(string email, int id);
        Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuariosSustentabilidade();        
    }
}
