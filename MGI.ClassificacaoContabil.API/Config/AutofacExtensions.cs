using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace API.Config
{
    public static class AutofacExtensions
    {
        public static void ConfigureAutofac(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
        {
            hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            hostBuilder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.RegisterRepositories("MGI.ClassificacaoContabil.Repository");
                containerBuilder.RegisterServices("MGI.ClassificacaoContabil.Service");
                containerBuilder.RegisterConnection(configuration);
            });
        }
    }

}
