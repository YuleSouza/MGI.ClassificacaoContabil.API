using Infra.Data;
using Infra.Interface;
using Service.Interface.Empresa;
using Service.Interface.Cenario;
using Service.Interface.Classificacao;
using Service.Interface.Parametrizacao;
using Service.Cenario;
using Service.Classificacao;
using Service.Empresa;
using Service.Parametrizacao;
using Service.Repository.Empresa;
using Service.Repository.Cenario;
using Service.Repository.Classificacao;
using Service.Repository.Parametrizacao;
using Repository.Empresa;
using Repository.Cenario;
using Repository.Classificacao;
using Repository.Parametrizacao;
using Service.Interface.FiltroTela;
using Service.FiltroTela;
using Service.Repository.FiltroTela;
using Repository.FiltroTela;

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
            services.AddScoped<IParametrizacaoService, ParametrizacaoService>();
            services.AddScoped<IFiltroTelaService, FiltroTelaService>();

            //Repository            
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<ICenarioRepository, CenarioRepository>();
            services.AddScoped<IClassificacaoRepository, ClassificacaoRepository>();
            services.AddScoped<IParametrizacaoRepository, ParametrizacaoRepository>();
            services.AddScoped<IFiltroTelaRepository, FiltroTelaRepository>();

            return services;
        }
    }
}
