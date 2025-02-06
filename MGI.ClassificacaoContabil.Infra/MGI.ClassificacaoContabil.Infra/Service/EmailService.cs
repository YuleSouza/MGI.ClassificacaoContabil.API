using Infra.DTO;
using Infra.Service.Interfaces;
using MGI.ClassificacaoContabil.Infra.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;



namespace Infra.Service
{   
    public class EmailService : IEmailService
    {
        private readonly IKeyVaultService _keyVaultService;
        private readonly IConfiguration _configuration;
        private EmailAutenticacaoServicoDTO _emailAutenticacaoServicoDTO;

        public EmailService(IKeyVaultService keyVaultService, IConfiguration configuration)
        {
            _keyVaultService = keyVaultService;
            _configuration = configuration;
        }

        public async Task EnviarEmailAsync(EmailAprovacaoDTO email)
        {
            await SetAuthentication();
            string[] scopes = new[] { _emailAutenticacaoServicoDTO.Scope };
            var app = ConfidentialClientApplicationBuilder.Create(_emailAutenticacaoServicoDTO.ClientId)
                .WithClientSecret(_emailAutenticacaoServicoDTO.ClientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{_emailAutenticacaoServicoDTO.TenantId}"))
            .Build();

            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            string accessToken = authResult.AccessToken;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(authResult.AccessToken);
            var roles = jwt.Claims.Where(c => c.Type == "Role").Select(c => c.Value);
            var listaRoles = string.Join(", ", roles);
            var containRoles = roles.Contains("Mail.Send");

            var authProvider = new CustomAuthenticationProvider(accessToken);
            var graphClient = new GraphServiceClient(authProvider);

            string basePath = AppContext.BaseDirectory;
            string currentDirectory = Directory.GetCurrentDirectory();
            string templatePath = Path.Combine(currentDirectory, @"..\MGI.ClassificacaoContabil.Infra\MGI.ClassificacaoContabil.Infra\Template\prototipo_aprovacao_email.html");
            string fullPath = Path.GetFullPath(templatePath);
            var template = File.ReadAllText(@"C:\Projetos\mgi-classificacao-contabil-api\MGI.ClassificacaoContabil.Infra\MGI.ClassificacaoContabil.Infra\Template\prototipo_aprovacao_email.html");
            template = template.Replace("#IDPROJETO", email.IdProjeto.ToString());
            template = template.Replace("#NOMEPROJETO", email.NomeProjeto);
            template = template.Replace("#NOMEGESTOR", email.NomeGestor);
            template = template.Replace("#PATROCINADOR", email.NomePatrocinador);
            template = template.Replace("#PERCENTUALKPI", email.PercentualKPI.ToString());
            template = template.Replace("#CLASSIFICACAO", email.NomeClassificacao.ToString());
            template = template.Replace("#SUBCLASSIFICACAO", email.NomeSubClassificacao.ToString());

            var message = new Message()
            {
                Subject = "Aprovação de Projeto",
                Body = new ItemBody()
                {
                    ContentType = BodyType.Html,
                    Content = template
                },
                ToRecipients = await ProcessEmailString(email.EmailDestinatario)
            };

            try
            {
                var send = graphClient.Users[_emailAutenticacaoServicoDTO.SenderEmail].SendMail;
                await send.PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody()
                {
                    Message = message,
                });
            }
            catch (ServiceException)
            {

            }
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
            };
        }

        public async Task<List<Recipient>> ProcessEmailString(string emails)
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
    }
}
