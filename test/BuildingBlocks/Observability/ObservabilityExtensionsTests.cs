using System.Reflection;
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

        Assert.Equal(ServiceName, attributes["service.name"]);
        Assert.Equal(ObservabilityExtensions.ServiceNamespace, attributes["service.namespace"]);
        Assert.Equal("local", attributes[ObservabilityExtensions.EnvironmentAttribute]);
        Assert.Equal("core", attributes["team"]);
    }

    [Fact]
    public void ResourceAttributes_Defaults_InstanceIdIsMachineName()
    {
        var attributes = ObservabilityExtensions.ResourceAttributes(Configuration(), ServiceName);

        Assert.Equal(Environment.MachineName, attributes[ObservabilityExtensions.ServiceInstanceIdAttribute]);
    }

    [Fact]
    public void ResourceAttributes_Defaults_VersionIsEntryAssemblyVersionWithoutSourceRevision()
    {
        var informational = Assembly.GetEntryAssembly()!
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;

        var attributes = ObservabilityExtensions.ResourceAttributes(Configuration(), ServiceName);

        var version = Assert.IsType<string>(attributes[ObservabilityExtensions.ServiceVersionAttribute]);
        Assert.Equal(informational.Split('+')[0], version);
        Assert.DoesNotContain('+', version);
    }

    [Fact]
    public void ResourceAttributes_DeclaredVersionAndInstanceId_OverrideDefaults()
    {
        var configuration = Configuration(
            (ObservabilityExtensions.ResourceAttributesKey, "service.version=2.3.4,service.instance.id=core-api-1"));

        var attributes = ObservabilityExtensions.ResourceAttributes(configuration, ServiceName);

        Assert.Equal("2.3.4", attributes[ObservabilityExtensions.ServiceVersionAttribute]);
        Assert.Equal("core-api-1", attributes[ObservabilityExtensions.ServiceInstanceIdAttribute]);
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
