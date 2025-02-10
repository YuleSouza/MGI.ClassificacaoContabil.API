using Infra.DTO;

namespace Infra.Service.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailAprovacao(EmailAprovacaoDTO email);
        Task EnviarEmailGestor(GestorEmailDTO email);
    }
}
