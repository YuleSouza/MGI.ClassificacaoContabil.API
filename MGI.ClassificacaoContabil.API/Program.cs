using API.Config;

using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// Add services to the container.
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

builder.Services.AddAuthentication();
builder.Services.AddControllers();
builder.Services.AddApiConfig();

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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "APIs - ClassificacaoContabil");
});
app.UseDeveloperExceptionPage();
app.UseCors("ClientPermission");
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
