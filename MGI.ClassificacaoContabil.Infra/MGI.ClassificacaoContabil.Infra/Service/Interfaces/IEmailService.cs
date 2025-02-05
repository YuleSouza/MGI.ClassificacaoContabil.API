using Infra.DTO;

namespace Infra.Service.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(EmailAprovacaoDTO email);
    }
}
