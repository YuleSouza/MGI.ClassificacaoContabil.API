using API.Config;
using API.Handlers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var arrayClientAddress = configuration.GetSection("ClientPermission").GetChildren().Select(x => x.Value).ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .WithOrigins(arrayClientAddress)
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("Bearer", null);

builder.Services.AddControllers();
builder.Services.AddResponseCaching();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterRepositories("MGI.ClassificacaoContabil.Repository");
    containerBuilder.RegisterServices("MGI.ClassificacaoContabil.Service", configuration);
    containerBuilder.RegisterConnection(configuration);
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0.0", new OpenApiInfo { Title = "MGI.New", Version = "v1", Description = "API ClassificacaoContabil" });
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
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
IWebHostEnvironment environment = app.Environment;

app.UseSwagger();
if (!environment.IsProduction())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "APIs - ClassificacaoContabil");
    });
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("ClientPermission");
app.UseRouting();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
