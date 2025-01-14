using MGI.ClassificacaoContabil.Infra.Service;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace Infra.Service
{
    public class EmailService
    {
        public async Task EnviarEmailAsync(string emailDestino, string assunto, string mensagemCorpo)
        {            
        }
    }
}
