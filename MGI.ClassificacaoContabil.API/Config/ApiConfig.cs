using Infra.Data;
using Infra.Interface;
using Service.Helper;
using MGI.ClassificacaoContabil.Service.Service.Classificacao;
using MGI.ClassificacaoContabil.Service.Service.PainelClassificacao;
using Repository.Cenario;
using Repository.Classificacao;
using Repository.Empresa;
using Repository.FiltroTela;
using Repository.PainelClassificacao;
using Repository.PainelEsg;
using Repository.Parametrizacao;
using Service.Cenario;
using Service.Classificacao;
using Service.Empresa;
using Service.Esg;
using Service.FiltroTela;
using Service.Interface.Cenario;
using Service.Interface.Classificacao;
using Service.Interface.Empresa;
using Service.Interface.FiltroTela;
using Service.Interface.PainelClassificacao;
using Service.Interface.PainelEsg;
using Service.Interface.Parametrizacao;
using Service.PainelClassificacao;
using Service.Parametrizacao;
using Service.Repository.Cenario;
using Service.Repository.Classificacao;
using Service.Repository.Empresa;
using Service.Repository.Esg;
using Service.Repository.FiltroTela;
using Service.Repository.PainelClassificacao;
using Service.Repository.Parametrizacao;

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
            services.AddScoped<IClassificacaoEsgService, ClassificacaoEsgService>();
            services.AddScoped<IParametrizacaoService, ParametrizacaoService>();
            services.AddScoped<IFiltroTelaService, FiltroTelaService>();
            services.AddScoped<IPainelClassificacaoService, PainelClassificacaoService>();
            services.AddScoped<IParametrizacaoCenarioService, ParametrizacaoCenarioService>();
            services.AddScoped<IParametrizacaoEsgGeralService, ParametrizacaoEsgGeralService>();
            services.AddScoped<IClassificacaoContabilService, ClassificacaoContabilService>();
            services.AddScoped<IPainelClassificacaoEsgService, PainelClassificacaoEsgService>();
            services.AddScoped<IPainelEsgService, PainelEsgService>();
            services.AddScoped<IEsgAnexoService, EsgAnexoService>();

            //Repository            
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<ICenarioRepository, CenarioRepository>();
            services.AddScoped<IClassificacaoRepository, ClassificacaoRepository>();
            services.AddScoped<IParametrizacaoRepository, ParametrizacaoRepository>();
            services.AddScoped<IFiltroTelaRepository, FiltroTelaRepository>();
            services.AddScoped<IPainelClassificacaoRepository, PainelClassificacaoRepository>();
            services.AddScoped<IPainelEsgRepository, PainelEsgRepository>();

            services.AddScoped<ITransactionHelper, TransactionHelper>(); 

            return services;
        }
    }
}
