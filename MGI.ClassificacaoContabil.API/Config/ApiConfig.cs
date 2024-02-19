using Infra.Data;
using Infra.Interface;
using Repository.Empresa;

using Service.Empresa;
using Service.Repository.Empresa;
using Service.Interface.Empresa;

namespace API.Config
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            services.AddScoped<DbSession>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();


            //Services
            services.AddScoped<IEmpresaService, EmpresaService>();
            //Repository            
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();

            return services;
        }
    }
}
