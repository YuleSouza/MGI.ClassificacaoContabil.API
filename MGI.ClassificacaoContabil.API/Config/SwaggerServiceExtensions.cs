namespace MGI.ClassificacaoContabil.API.Config
{
    using Microsoft.OpenApi.Models;
    using System.Reflection;

    public static class SwaggerServiceExtensions
    {
        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0.0", new OpenApiInfo
                {
                    Title = "MGI.New",
                    Version = "v1",
                    Description = "API ClassificacaoContabil"
                });
                options.AddSecurityDefinition("Api", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Api" }
                    },
                    new string[] { }
                }
            });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //options.IncludeXmlComments(xmlPath);
            });
        }
    }

}
