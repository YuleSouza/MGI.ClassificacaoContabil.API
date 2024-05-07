using Infra.Data;
using Infra.Interface;
using Service.Interface.Empresa;
using Service.Interface.Cenario;
using Service.Interface.FiltroTela;
using Service.Interface.Classificacao;
using Service.Interface.Parametrizacao;
using Service.Interface.PainelClassificacao;
using Service.Empresa;
using Service.Cenario;
using Service.FiltroTela;
using Service.Classificacao;
using Service.Parametrizacao;
using Service.PainelClassificacao;
using Service.Repository.Empresa;
using Service.Repository.Cenario;
using Service.Repository.FiltroTela;
using Service.Repository.Classificacao;
using Service.Repository.Parametrizacao;
using Service.Repository.PainelClassificacao;
using Repository.Empresa;
using Repository.Cenario;
using Repository.FiltroTela;
using Repository.Classificacao;
using Repository.Parametrizacao;
using Repository.PainelClassificacao;

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
            services.AddScoped<IPainelClassificacaoService, PainelClassificacaoService>();

            //Repository            
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<ICenarioRepository, CenarioRepository>();
            services.AddScoped<IClassificacaoRepository, ClassificacaoRepository>();
            services.AddScoped<IParametrizacaoRepository, ParametrizacaoRepository>();
            services.AddScoped<IFiltroTelaRepository, FiltroTelaRepository>();
            services.AddScoped<IPainelClassificacaoRepository, PainelClassificacaoRepository>();

            return services;
        }
    }
}
