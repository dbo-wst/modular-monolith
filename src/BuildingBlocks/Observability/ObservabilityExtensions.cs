using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Acme.Core.BuildingBlocks.Observability;

/// <summary>
/// OTLP export of logs (through Serilog), traces and metrics. All are off unless <see cref="EndpointKey"/> is
/// configured. Acme's own meters (<see cref="AcmeInstrumentation"/>) and the .NET runtime meter
/// (<see cref="RuntimeInstrumentation"/>) are exported as metrics, ASP.NET Core and HTTP meters are not (ADR 0002).
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>Base URL of the OTLP receiver, e.g. <c>https://otel-local.steubing.com:4317</c> (gRPC).</summary>
    public const string EndpointKey = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary><c>grpc</c> (the standard, port 4317; also the SDK default when unset) or <c>http/protobuf</c> (port 4318).</summary>
    public const string ProtocolKey = "OTEL_EXPORTER_OTLP_PROTOCOL";

    /// <summary>Comma-separated <c>key=value</c> resource attributes, e.g. <c>deployment.environment.name=local</c>.</summary>
    public const string ResourceAttributesKey = "OTEL_RESOURCE_ATTRIBUTES";

    /// <summary>The <c>service.namespace</c> of every Acme service.</summary>
    public const string ServiceNamespace = "acme";

    /// <summary>The environment attribute; <c>unknown</c> when <see cref="ResourceAttributesKey"/> does not declare it.</summary>
    public const string EnvironmentAttribute = "deployment.environment.name";

    /// <summary>
    /// The version attribute: the entry assembly's informational version without the source revision (the image
    /// version passed to <c>dotnet publish -p:Version</c>), <see cref="UnknownVersion"/> when there is none.
    /// </summary>
    public const string ServiceVersionAttribute = "service.version";

    /// <summary>
    /// The instance attribute: the machine name, i.e. the container's host name, which the compose files set to the
    /// container name so that a recreated container keeps its instance.
    /// </summary>
    public const string ServiceInstanceIdAttribute = "service.instance.id";

    /// <summary>The <see cref="ServiceVersionAttribute"/> without an informational version.</summary>
    public const string UnknownVersion = "0.0.0";

    /// <summary>
    /// Wildcard for Acme's own <c>ActivitySource</c>s and <c>Meter</c>s. Name custom sources and meters <c>Acme.*</c>
    /// (e.g. <c>Acme.Core.Api.Jobs</c>) and they are exported without further registration.
    /// </summary>
    public const string AcmeInstrumentation = "Acme.*";

    /// <summary>The .NET runtime's built-in meter: GC, heap, thread pool, lock contention, exceptions, CPU.</summary>
    public const string RuntimeInstrumentation = "System.Runtime";

    private const string HttpProtobuf = "http/protobuf";

    /// <summary>
    /// Registers tracing of ASP.NET Core requests, outgoing HTTP calls (gRPC clients included), SQL Server commands and
    /// <see cref="AcmeInstrumentation"/> sources, plus the <see cref="AcmeInstrumentation"/> and
    /// <see cref="RuntimeInstrumentation"/> meters with trace-based exemplars. Both use an OTLP exporter that takes
    /// endpoint and protocol from configuration. Does nothing when <see cref="EndpointKey"/> is not set.
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services, IConfiguration configuration, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(configuration[EndpointKey]))
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddAttributes(ResourceAttributes(configuration, serviceName)))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddSource(AcmeInstrumentation)
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddMeter(AcmeInstrumentation, RuntimeInstrumentation)
                // Exemplars link a histogram bucket to the trace that was active when the value was recorded.
                .SetExemplarFilter(ExemplarFilterType.TraceBased)
                .AddOtlpExporter());

        return services;
    }

    /// <summary>
    /// Adds an OTLP sink for Information and above with the same resource attributes as the traces; each event carries
    /// the current trace and span id. Does nothing when <see cref="EndpointKey"/> is not set. Export failures are
    /// written to stderr.
    /// </summary>
    public static LoggerConfiguration WriteToOtlp(
        this LoggerConfiguration loggerConfiguration, IConfiguration configuration, string serviceName)
    {
        var endpoint = configuration[EndpointKey];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return loggerConfiguration;
        }

        var http = string.Equals(configuration[ProtocolKey]?.Trim(), HttpProtobuf, StringComparison.OrdinalIgnoreCase);
        SelfLog.Enable(Console.Error);

        return loggerConfiguration.WriteTo.OpenTelemetry(
            options =>
            {
                options.Endpoint = null;
                options.LogsEndpoint = http ? $"{endpoint.TrimEnd('/')}/v1/logs" : endpoint;
                options.Protocol = http ? OtlpProtocol.HttpProtobuf : OtlpProtocol.Grpc;
                options.ResourceAttributes = ResourceAttributes(configuration, serviceName);
                options.RestrictedToMinimumLevel = LogEventLevel.Information;
                // Keeps the sink's own export requests out of the traces.
                options.OnBeginSuppressInstrumentation = SuppressInstrumentationScope.Begin;
            },
            ignoreEnvironment: true);
    }

    /// <summary>
    /// <c>service.name</c>, <c>service.namespace</c>, <see cref="ServiceVersionAttribute"/>,
    /// <see cref="ServiceInstanceIdAttribute"/> and the pairs declared in <see cref="ResourceAttributesKey"/>, which
    /// override them; <see cref="EnvironmentAttribute"/> defaults to <c>unknown</c>.
    /// </summary>
    public static Dictionary<string, object> ResourceAttributes(IConfiguration configuration, string serviceName)
    {
        var attributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName,
            ["service.namespace"] = ServiceNamespace,
            [ServiceVersionAttribute] = EntryAssemblyVersion(),
            [ServiceInstanceIdAttribute] = Environment.MachineName
        };

        var declared = (configuration[ResourceAttributesKey] ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(pair => pair.Split('=', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts is [{ Length: > 0 }, { Length: > 0 }]);

        foreach (var parts in declared)
        {
            attributes[Uri.UnescapeDataString(parts[0])] = Uri.UnescapeDataString(parts[1]);
        }

        attributes.TryAdd(EnvironmentAttribute, "unknown");
        return attributes;
    }

    /// <summary>
    /// The entry assembly's informational version cut at <c>+</c>, where the SDK appends the source revision
    /// (<c>1.4.0+3f2a9c1</c> becomes <c>1.4.0</c>).
    /// </summary>
    private static string EntryAssemblyVersion()
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
            .Split('+', 2)[0];

        return string.IsNullOrWhiteSpace(version) ? UnknownVersion : version;
    }
}
