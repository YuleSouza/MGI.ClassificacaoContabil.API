using API.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace MGI.ClassificacaoContabil.API.Config
{
    public static class AuthenticationServiceExtensions
    {
        public static void AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("Bearer", null);
        }
    }

}
