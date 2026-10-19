using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Acme.Core.BuildingBlocks.Observability;

/// <summary>
/// OTLP export of logs (through Serilog) and traces. Both are off unless <see cref="EndpointKey"/> is configured;
/// metrics are deliberately not exported.
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>Base URL of the OTLP receiver, e.g. <c>https://otel-local.steubing.com:4318</c>.</summary>
    public const string EndpointKey = "OTEL_EXPORTER_OTLP_ENDPOINT";

    /// <summary><c>http/protobuf</c> (the standard) or <c>grpc</c> (the SDK default when unset).</summary>
    public const string ProtocolKey = "OTEL_EXPORTER_OTLP_PROTOCOL";

    /// <summary>Comma-separated <c>key=value</c> resource attributes, e.g. <c>deployment.environment.name=local</c>.</summary>
    public const string ResourceAttributesKey = "OTEL_RESOURCE_ATTRIBUTES";

    /// <summary>The <c>service.namespace</c> of every Acme service.</summary>
    public const string ServiceNamespace = "acme";

    /// <summary>The environment attribute; <c>unknown</c> when <see cref="ResourceAttributesKey"/> does not declare it.</summary>
    public const string EnvironmentAttribute = "deployment.environment.name";

    private const string HttpProtobuf = "http/protobuf";

    /// <summary>
    /// Registers tracing of ASP.NET Core requests, outgoing HTTP calls (gRPC clients included) and SQL Server commands
    /// with an OTLP exporter that takes endpoint and protocol from configuration. Does nothing when
    /// <see cref="EndpointKey"/> is not set.
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
    /// <c>service.name</c>, <c>service.namespace</c> and the pairs declared in <see cref="ResourceAttributesKey"/>;
    /// <see cref="EnvironmentAttribute"/> defaults to <c>unknown</c>.
    /// </summary>
    public static Dictionary<string, object> ResourceAttributes(IConfiguration configuration, string serviceName)
    {
        var attributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName,
            ["service.namespace"] = ServiceNamespace
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
}
