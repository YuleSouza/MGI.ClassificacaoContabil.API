using Infra.DTO;
using Infra.Service.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace Infra.Service
{   
    public class EmailService : IEmailService
    {
        private readonly IKeyVaultService _keyVaultService;

        public EmailService(IKeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
        }
        public async Task EnviarEmailAsync(EmailAprovacaoDTO email)
        {
            string tenantId = await _keyVaultService.ConsultarSegredo("Esg-Capes-tenantId");
            string clientId = await _keyVaultService.ConsultarSegredo("Esg-Capes-clientid");
            string clientSecret = await _keyVaultService.ConsultarSegredo("Esg-Capes-clientSecret"); 
            string scope = await _keyVaultService.ConsultarSegredo("Esg-Capex-scopes");
            string[] scopes = new[] { scope };
            string senderEmail = "T_Andre.Silva@ecorodovias.com.br";

        }
    }
}
