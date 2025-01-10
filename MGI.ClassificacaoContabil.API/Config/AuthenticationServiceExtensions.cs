using API.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace API.Config
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
