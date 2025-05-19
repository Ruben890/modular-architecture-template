using Microsoft.AspNetCore.HttpOverrides;
using Modular_Architecture_Template.Extencions;
using Modular_Architecture_Template.Extensions;
using Newtonsoft.Json.Serialization;
using Wolverine;

var basePath = AppContext.BaseDirectory;
var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogHost(builder.Configuration);


builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Host.UseWolverine();
builder.Services.RegisterModules(builder.Configuration);
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureCors(builder.Configuration);
builder.Services.AddAuthentication();



builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });


var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
