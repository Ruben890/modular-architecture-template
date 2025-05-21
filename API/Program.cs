using API.Extensions;
using JasperFx.Core;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;


var basePath = AppContext.BaseDirectory;
var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogHost(builder.Configuration);


builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("StringConnection")
                       ?? throw new NullReferenceException("The connection string 'StringConnection' was not found.");

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(connectionString);
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.OnException<TimeoutException>()
        .RetryWithCooldown(100.Milliseconds(), 1.Seconds(), 5.Seconds());
});

builder.Services.ConfigureLoggerService();
builder.Services.ConfigureAuthJWT(builder.Configuration);
builder.Services.AddGlobalCookiePolicy(builder.Environment);
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.RegisterModules(builder.Configuration);
builder.Services.ConfigureCors(builder.Configuration);
builder.Services.AddAuthentication();
builder.Services.AddConfiguredControllers();

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
