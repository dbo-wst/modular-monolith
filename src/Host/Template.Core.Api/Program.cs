using Serilog;
using Template.Core.Shared.Kernel.Communication;
// using Template.Core.Api.Prelaunch.Allocation;
using Template.Core.Api.Prelaunch.Ordering;
// using Template.Core.Api.Prelaunch.ClientSide;
// using Template.Core.Api.Prelaunch.MasterData;

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

    builder.Services.AddGrpc();

    // One mediator (and its pipeline behaviours) shared by all modules; modules only register their handlers.
    builder.Services.AddMediator();

    builder.Services.AddOrderingModule(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapOrderingGrpcServices();

    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    Log.Information("Start Template.Core.Api. Env: {Environment}", environmentName ?? "Unknown");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Template.Core.Api terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace Template.Core.Api
{
    public partial class Program;
}
