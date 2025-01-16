using API.Config;


var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddCustomCors(configuration);
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomSwagger();
builder.Services.AddCustomServices(configuration);
builder.Host.ConfigureAutofac(configuration);

var app = builder.Build();
app.UseBuilderConfiguration();
app.Run();