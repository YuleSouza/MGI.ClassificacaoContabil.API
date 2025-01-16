namespace API.Config
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddResponseCaching();
            services.AddEndpointsApiExplorer();
        }
    }

}
