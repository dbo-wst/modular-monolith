using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;
using Serilog;
// using Template.Api.Allocation;
using Template.Api.Ordering;
// using Template.Api.ClientSide;
using Template.Api.Infrastructure.CoreApi;

// using Template.Api.MasterData;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                      ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

var bootstrapConfig = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true);

if (!string.IsNullOrWhiteSpace(environmentName))
{
    bootstrapConfig.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(bootstrapConfig.Build())
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddOpenApi(options =>
    {
        options.CreateSchemaReferenceId = type
            => type.Type.Namespace == null ?
                null : type.Type.Namespace.StartsWith("Template.Api", StringComparison.Ordinal) ?
                    type.Type.FullName?.Replace("Template.Api.", "") : null;
    });

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    builder.Services.AddCoreApiInfrastructure(builder.Configuration);

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                                 | ForwardedHeaders.XForwardedProto
                                 | ForwardedHeaders.XForwardedHost
                                 | ForwardedHeaders.XForwardedPrefix;
        //TODO: to clear is not applicable. Configure proxies and networks
        options.KnownProxies.Clear();
        options.KnownIPNetworks.Clear();
    });

    var app = builder.Build();

    app.UseForwardedHeaders();
    app.UseSerilogRequestLogging();

    var pathBase = builder.Configuration["PathBase"];
    if (!string.IsNullOrWhiteSpace(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    app.MapOpenApi();
    app.MapScalarApiReference((options, context) =>
    {
        options.Title = "Template.Api Reference";
    });

    app.MapOrderingEndpoints();
    // app.MapClientSideEndpoints();
    // app.MapAllocationEndpoints();
    // app.MapMasterDataEndpoints();

    Log.Information("Start Template.Api. Env: {Environment}", environmentName ?? "Unknown");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Template.Api terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace Template.Api
{
    public partial class Program;
}
