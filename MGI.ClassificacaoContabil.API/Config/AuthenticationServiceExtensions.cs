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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Roles", policy => policy.RequireRole(new string[] { "PMO", "Sustentabilidade" }));
                //options.AddPolicy("SustentabilidadePolicy", policy => policy.RequireClaim("group", "Sustentabilidade"));
                //options.AddPolicy("PMOPolicy", policy => policy.RequireClaim("group", "PMO"));
                //options.AddPolicy("PMOAndSustentabilidade", policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Type == "group" && (c.Value == "PMO" || c.Value == "Sustentabilidade"))));
            });

        }
    }

}
