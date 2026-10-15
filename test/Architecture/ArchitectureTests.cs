using System.Reflection;
using NetArchTest.Rules;

namespace Template.Core.Architecture.Test;

/// <summary>Guard rails for the Clean Architecture dependency rule inside the Ordering module.</summary>
public class ArchitectureTests
{
    private const string Root = "Template.Core.Ordering";

    private static readonly Assembly Domain = typeof(Ordering.Domain.Orders.Order).Assembly;
    private static readonly Assembly Application = typeof(Ordering.Application.IOrderingReadStore).Assembly;
    private static readonly Assembly Contracts = typeof(Ordering.Contracts.Dto.Order).Assembly;

    private static readonly Assembly BuildingBlocksDomain = typeof(BuildingBlocks.Domain.IHasDomainEvents).Assembly;
    private static readonly Assembly BuildingBlocksApplication = typeof(BuildingBlocks.Application.Communication.IMediator).Assembly;

    // Infrastructure frameworks that must never leak into a domain layer.
    private static readonly string[] InfrastructureFrameworks =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.Extensions.DependencyInjection",
        "FluentValidation"
    ];

    // One project per adapter. Add new adapter projects (Messaging, ExternalServices, ...) here.
    private static readonly Assembly[] Adapters =
    [
        typeof(Ordering.Persistence.UnitOfWork).Assembly
    ];

    private static string[] AdapterNamespaces => Adapters.Select(a => a.GetName().Name!).ToArray();

    private static void AssertNoDependency(Assembly assembly, params string[] forbidden)
    {
        var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        Assert.True(result.IsSuccessful,
            $"{assembly.GetName().Name} must not depend on {string.Join(", ", forbidden)}. Offenders: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void AdapterAssemblies_LiveInTheModuleNamespace() =>
        Assert.All(AdapterNamespaces, name => Assert.StartsWith($"{Root}.", name));

    [Fact]
    public void Domain_DependsOnNeitherApplicationContractsNorAdapters() =>
        AssertNoDependency(Domain, [$"{Root}.Application", $"{Root}.Contracts", .. AdapterNamespaces]);

    [Fact]
    public void Contracts_DependOnNoOtherModuleLayer() =>
        AssertNoDependency(Contracts, [$"{Root}.Domain", $"{Root}.Application", .. AdapterNamespaces]);

    [Fact]
    public void Application_DoesNotDependOnAdaptersOrEfCore() =>
        AssertNoDependency(Application, [.. AdapterNamespaces, "Microsoft.EntityFrameworkCore"]);

    [Fact]
    public void Adapters_DoNotDependOnEachOther()
    {
        foreach (var adapter in Adapters)
        {
            var others = AdapterNamespaces.Where(n => n != adapter.GetName().Name).ToArray();
            if (others.Length > 0)
                AssertNoDependency(adapter, others);
        }
    }

    [Fact]
    public void Gateway_OnlyKnowsTheModuleThroughContractsOrProtocol()
    {
        var gateway = typeof(Template.Api.Segments).Assembly;
        AssertNoDependency(gateway, [$"{Root}.Domain", $"{Root}.Application", .. AdapterNamespaces]);
    }

    [Fact]
    public void Domain_DoesNotDependOnInfrastructureFrameworks() =>
        AssertNoDependency(Domain, InfrastructureFrameworks);

    [Fact]
    public void BuildingBlocksDomain_DoesNotDependOnInfrastructureFrameworks() =>
        AssertNoDependency(BuildingBlocksDomain, InfrastructureFrameworks);

    [Fact]
    public void BuildingBlocksDomain_DoesNotDependOnOtherBuildingBlocks() =>
        AssertNoDependency(BuildingBlocksDomain,
            "Template.Core.BuildingBlocks.Application", "Template.Core.BuildingBlocks.Persistence");

    [Fact]
    public void BuildingBlocksApplication_DoesNotDependOnEfCoreOrPersistence() =>
        AssertNoDependency(BuildingBlocksApplication,
            "Microsoft.EntityFrameworkCore", "Template.Core.BuildingBlocks.Persistence");

    [Fact]
    public void CommandsAndQueries_AreSealed()
    {
        var result = Types.InAssembly(Contracts)
            .That().ResideInNamespaceEndingWith("Commands").Or().ResideInNamespaceEndingWith("Queries")
            .Should().BeSealed()
            .GetResult();
        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }
}
