using DTO.Payload;
using Service.DTO.Esg;

namespace Service.Interface.PainelEsg
{
    public interface IEsgAprovadorService
    {
        Task<IEnumerable<EsgAprovadorDTO>> ConsultarUsuarioAprovador(string usuario, string email);
        Task<PayloadDTO> InserirUsuarioAprovador(string usuario, string email);
        Task<PayloadDTO> ExcluirUsuarioAprovador(int id);
        Task<PayloadDTO> AlterarUsuarioAprovador(string email, int id);
    }
}
