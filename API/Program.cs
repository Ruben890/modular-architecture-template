﻿using API.Extensions;
using JasperFx.Core;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Npgsql;
using Wolverine;
using Wolverine.EntityFrameworkCore;
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
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
