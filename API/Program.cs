using API.Extensions;
using JasperFx.Core;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Npgsql;
using Serilog;
using System.Runtime.InteropServices;
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
    opts.PersistMessagesWithPostgresql(connectionString, schemaName: "wolverine");
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Discovery.IncludeAllWolverineModuleHandlers();
    opts.OnException<TimeoutException>()
        .RetryWithCooldown(100.Milliseconds(), 1.Seconds(), 5.Seconds());
    opts.OnException<NpgsqlException>()
        .RetryWithCooldown(500.Milliseconds(), 5.Seconds(), 30.Seconds());
});

builder.Services.ConfigureLoggerService();
builder.Services.ConfigureAuthJWT(builder.Configuration);
builder.Services.AddGlobalCookiePolicy(builder.Environment);
builder.Services.ConfigureApiVersioning(builder.Configuration);
TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
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

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers["X-Content-Type-Options"] = "nosniff";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["X-XSS-Protection"] = "0";
    headers["X-Frame-Options"] = "DENY";

    headers["Content-Security-Policy"] = string.Join("; ",
        "default-src 'self'",
        "script-src 'self'",
        "style-src 'self'",
        "img-src 'self' data:",
        "font-src 'self'",
        "object-src 'none'",
        "frame-ancestors 'none'",
        "base-uri 'none'",
        "form-action 'self'"
    );

    headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), fullscreen=(), payment=(), usb=()";

    await next();
});

var webRootPath = app.Environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");

if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
}

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "wwwroot")),
        RequestPath = "/static",
        OnPrepareResponse = ctx =>
        {
            // Configuración de caché para archivos estáticos
            const int durationInSeconds = 60 * 60 * 24 * 7; // 7 días
            ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={durationInSeconds}");

            // Bloquear acceso a la carpeta Templates
            if (ctx.File.PhysicalPath!.Contains(Path.Combine("wwwroot", "Templates")))
            {
                ctx.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                ctx.Context.Response.ContentLength = 0;
                ctx.Context.Response.Body = Stream.Null;
            }
        }
    });

}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    app.UseStaticFiles();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
