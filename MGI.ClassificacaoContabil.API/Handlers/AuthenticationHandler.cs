using DTO.Payload;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Service.Interface.Usuario;
using Service.Usuario;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.Handlers
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;
        public AuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration,
        IUsuarioService usuarioService)
        : base(options, logger, encoder, clock) 
        {
            _configuration = configuration;
            _usuarioService = usuarioService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //if (!Request.Headers.ContainsKey("Authorization"))
            //    return Task.FromResult(AuthenticateResult.Fail("Não autorizado."));

            //var authHeader = Request.Headers["Authorization"].ToString();
            
            //if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            //    return Task.FromResult(AuthenticateResult.Fail("Não autorizado"));

            //var token = authHeader.Substring("Bearer ".Length).Trim();
            //var tokenApi = _configuration.GetSection("basicAuthentication").Value;
            
            //if (token != tokenApi)
            //    return Task.FromResult(AuthenticateResult.Fail("Não autorizado."));

            var nomeUsuario = Request.Headers["Usuario"];
            var usuarios = _usuarioService.ConsultarUsuarioPorLogin(nomeUsuario).Result;

            if (!usuarios.Any()) return Task.FromResult(AuthenticateResult.Fail("Não autorizado."));

            var claims = new Claim[] { };

            foreach (var usuario in usuarios)
            {
                claims = new[] 
                    {
                        new Claim(ClaimTypes.Name, usuario.Login),
                        new Claim(ClaimTypes.Role, usuario.Grupo)
                    };
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
