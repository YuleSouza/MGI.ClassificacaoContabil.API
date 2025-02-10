using Infra.DTO;
using Infra.Service.Interfaces;
using MGI.ClassificacaoContabil.Infra.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace Infra.Service
{
    public class EmailService : IEmailService
    {
        private readonly IKeyVaultService _keyVaultService;
        private readonly IConfiguration _configuration;
        private EmailAutenticacaoServicoDTO _emailAutenticacaoServicoDTO;
        private GraphServiceClient _graphServiceClient;

        public EmailService(IKeyVaultService keyVaultService, IConfiguration configuration)
        {
            _keyVaultService = keyVaultService;
            _configuration = configuration;
        }

        public async Task EnviarEmailAprovacao(EmailAprovacaoDTO email)
        {
            await SetGraphClient();
            var message = new Message()
            {
                Subject = "Aprovação de Projeto",
                Body = new ItemBody()
                {
                    ContentType = BodyType.Html,
                    Content = await GetTemplateApproval(email)
                },
                ToRecipients = await ProcessEmailString(email.EmailDestinatario)
            };

            await EnviarEmail(message);
        }

        public async Task EnviarEmailGestor(GestorEmailDTO email)
        {
            await SetGraphClient();
            var message = new Message()
            {
                Subject = "Aprovação de Projeto",
                Body = new ItemBody()
                {
                    ContentType = BodyType.Html,
                    Content = await GetTemplateManger(email)
                },
                ToRecipients = await ProcessEmailString(email.EmailDestinatario)
            };

            await EnviarEmail(message);
        }

        private async Task EnviarEmail(Message message)
        {
            try
            {
                var send = _graphServiceClient.Users[_emailAutenticacaoServicoDTO.SenderEmail].SendMail;
                await send.PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody()
                {
                    Message = message,
                });
            }
            catch (ServiceException)
            {
            }
        }
        private async Task SetGraphClient()
        {
            await SetAuthentication();
            string[] scopes = new[] { _emailAutenticacaoServicoDTO.Scope };
            string accessToken = await GetAccessToken(scopes);
            var authProvider = new CustomAuthenticationProvider(accessToken);
            _graphServiceClient = new GraphServiceClient(authProvider);
        }
        private async Task SetAuthentication()
        {
            var emailAuth = _configuration.GetSection("EmailAuthentication");
            _emailAutenticacaoServicoDTO = new()
            {
                ClientId = emailAuth.GetSection("clientid").Value!,
                TenantId = emailAuth.GetSection("tenantid").Value!,
                ClientSecret = emailAuth.GetSection("clientSecret").Value!,
                Scope = emailAuth.GetSection("scope").Value!,
                SenderEmail = emailAuth.GetSection("senderEmail").Value!,
                Uri = emailAuth.GetSection("uri").Value!,
                TemplateDirectory = emailAuth.GetSection("templateDirectory").Value!,
                UrlAprovacao = emailAuth.GetSection("urlAprovacao").Value!,                
            };
        }
        private async Task<List<Recipient>> ProcessEmailString(string emails)
        {
            string[] emailArray = emails.Split(';');            
            List<Recipient> recipients = new List<Recipient>();
            foreach (string email in emailArray)
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    recipients.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = email.Trim()
                        }
                    });
                }
            }
            return recipients;
        }
        private async Task<string> GetAccessToken(string[] scopes)
        {
            var app = ConfidentialClientApplicationBuilder.Create(_emailAutenticacaoServicoDTO.ClientId)
                .WithClientSecret(_emailAutenticacaoServicoDTO.ClientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{_emailAutenticacaoServicoDTO.TenantId}"))
            .Build();

            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return authResult.AccessToken;
        }
        private async Task<string> GetTemplateApproval(EmailAprovacaoDTO email)
        {
            var template = await GetTemplateContent("prototipo_aprovacao_email.html");
            template = template.Replace("#IDPROJETO", email.IdProjeto.ToString());
            template = template.Replace("#NOMEPROJETO", email.NomeProjeto);
            template = template.Replace("#NOMEGESTOR", email.NomeGestor);
            template = template.Replace("#PATROCINADOR", email.NomePatrocinador);
            template = template.Replace("#PERCENTUALKPI", email.PercentualKPI.ToString());
            template = template.Replace("#CLASSIFICACAO", email.NomeClassificacao);
            template = template.Replace("#SUBCLASSIFICACAO", email.NomeSubClassificacao);
            template = template.Replace("#USUARIO", email.UsuarioCripto);
            template = template.Replace("#URL_APROVACAO", @$"{_emailAutenticacaoServicoDTO.UrlAprovacao}{email.IdClassifEsg}/A/{email.Usuario}");
            template = template.Replace("#URL_REPROVACAO", @$"{_emailAutenticacaoServicoDTO.UrlAprovacao}{email.IdClassifEsg}/R/{email.Usuario}");
            return template;
        }

        private async Task<string> GetTemplateManger(GestorEmailDTO email)
        {
            var template = await GetTemplateContent("prototipo_gestor_email.html");
            template = template.Replace("#IDPROJETO", email.IdProjeto.ToString());
            template = template.Replace("#NOMEPROJETO", email.NomeProjeto);            
            template = template.Replace("#PATROCINADOR", email.NomePatrocinador);
            template = template.Replace("#PERCENTUALKPI", email.PercentualKPI.ToString());
            template = template.Replace("#CLASSIFICACAO", email.NomeClassificacao);
            template = template.Replace("#SUBCLASSIFICACAO", email.NomeSubClassificacao);
            template = template.Replace("#APROVACAO", email.Aprovacao.ToUpper());
            template = template.Replace("#USUARIO", email.Usuario);
            return template;
        }
        private async Task<string> GetTemplateContent(string templateFileName)
        {
            return File.ReadAllText($@"{_emailAutenticacaoServicoDTO.TemplateDirectory}{templateFileName}");
        }

        
    }
}
