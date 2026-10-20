using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Acme.Core.BuildingBlocks.Observability.Test;

public class ObservabilityExtensionsTests
{
    private const string ServiceName = "acme-test";

    private static IConfiguration Configuration(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => KeyValuePair.Create(v.Key, (string?)v.Value)))
            .Build();

    private static IConfiguration Exporting() =>
        Configuration(
            (ObservabilityExtensions.EndpointKey, "http://localhost:4318"),
            (ObservabilityExtensions.ProtocolKey, "http/protobuf"));

    [Fact]
    public void ResourceAttributes_WithDeclaredAttributes_ContainsServiceAndDeclaredPairs()
    {
        var configuration = Configuration(
            (ObservabilityExtensions.ResourceAttributesKey, "deployment.environment.name=local, team = core"));

        var attributes = ObservabilityExtensions.ResourceAttributes(configuration, ServiceName);

        Assert.Equal(
            new Dictionary<string, object>
            {
                ["service.name"] = ServiceName,
                ["service.namespace"] = ObservabilityExtensions.ServiceNamespace,
                ["deployment.environment.name"] = "local",
                ["team"] = "core"
            },
            attributes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("team=core")]
    [InlineData("deployment.environment.name=")]
    public void ResourceAttributes_WithoutEnvironment_DefaultsToUnknown(string declared)
    {
        var configuration = Configuration((ObservabilityExtensions.ResourceAttributesKey, declared));

        var attributes = ObservabilityExtensions.ResourceAttributes(configuration, ServiceName);

        Assert.Equal("unknown", attributes[ObservabilityExtensions.EnvironmentAttribute]);
    }

    [Fact]
    public void AddObservability_WithoutEndpoint_RegistersNoTracerProvider()
    {
        using var provider = new ServiceCollection()
            .AddObservability(Configuration(), ServiceName)
            .BuildServiceProvider();

        Assert.Null(provider.GetService<TracerProvider>());
    }

    [Fact]
    public void AddObservability_WithEndpoint_RegistersTracerProvider()
    {
        using var provider = new ServiceCollection()
            .AddObservability(Exporting(), ServiceName)
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<TracerProvider>());
    }

    [Fact]
    public void AddObservability_WithoutEndpoint_RegistersNoMeterProvider()
    {
        using var provider = new ServiceCollection()
            .AddObservability(Configuration(), ServiceName)
            .BuildServiceProvider();

        Assert.Null(provider.GetService<MeterProvider>());
    }

    [Fact]
    public void AddObservability_WithEndpoint_RegistersMeterProvider()
    {
        using var provider = new ServiceCollection()
            .AddObservability(Exporting(), ServiceName)
            .BuildServiceProvider();

        Assert.NotNull(provider.GetService<MeterProvider>());
    }
}
