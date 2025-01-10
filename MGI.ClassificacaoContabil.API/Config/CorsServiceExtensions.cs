namespace MGI.ClassificacaoContabil.API.Config
{
    public static class CorsServiceExtensions
    {
        public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var arrayClientAddress = configuration.GetSection("ClientPermission").GetChildren().Select(x => x.Value).ToArray();

            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                          .WithOrigins(arrayClientAddress)
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }
    }

}
