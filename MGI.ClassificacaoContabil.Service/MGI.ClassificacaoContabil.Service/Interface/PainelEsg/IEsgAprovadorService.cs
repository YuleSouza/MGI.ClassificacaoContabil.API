using DTO.Payload;
using Service.DTO.Esg;

namespace Service.Interface.PainelEsg
{
    public interface IEsgAprovadorService
    {
        Task<EsgAprovadorDTO> ConsultarAprovadorPorUsuario(string usuario);
        Task<PayloadDTO> InserirUsuarioAprovador(string usuario, string email);
        Task<PayloadDTO> ExcluirUsuarioAprovador(int id);
        Task<PayloadDTO> AlterarUsuarioAprovador(string email, int id);
    }
}
