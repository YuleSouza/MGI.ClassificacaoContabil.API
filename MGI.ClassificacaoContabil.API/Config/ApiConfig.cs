using Infra.Data;
using Infra.Interface;
using Repository.Empresa;

using Service.Empresa;
using Service.Repository.Empresa;
using Service.Interface.Empresa;
using Service.Interface.Cenario;
using Service.Cenario;
using Service.Classificacao;
using Service.Interface.Classificacao;
using Service.Repository.Cenario;
using Repository.Cenario;
using Service.Repository.Classificacao;
using Repository.Classificacao;

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
            services.AddScoped<ICenarioService, CenarioService>();
            services.AddScoped<IClassificacaoService, ClassificacaoService>();

            //Repository            
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<ICenarioRepository, CenarioRepository>();
            services.AddScoped<IClassificacaoRepository, ClassificacaoRepository>();

            return services;
        }
    }
}
