using System.Reflection;
using NetArchTest.Rules;

namespace Template.Core.Architecture.Test;

/// <summary>Guard rails for the Clean Architecture dependency rule inside the Ordering module.</summary>
public class ArchitectureTests
{
    private const string Root = "Template.Core.Ordering";

    private static readonly Assembly Domain = typeof(Ordering.Domain.Abstractions.IUnitOfWork).Assembly;
    private static readonly Assembly Application = typeof(Ordering.Application.IOrderingReadStore).Assembly;
    private static readonly Assembly Contracts = typeof(Ordering.Contracts.Dto.Order).Assembly;

    private static void AssertNoDependency(Assembly assembly, params string[] forbidden)
    {
        var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        Assert.True(result.IsSuccessful,
            $"{assembly.GetName().Name} must not depend on {string.Join(", ", forbidden)}. Offenders: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_DependsOnNeitherApplicationInfrastructureNorContracts() =>
        AssertNoDependency(Domain, $"{Root}.Application", $"{Root}.Infrastructure", $"{Root}.Contracts");

    [Fact]
    public void Contracts_DependOnNoOtherModuleLayer() =>
        AssertNoDependency(Contracts, $"{Root}.Domain", $"{Root}.Application", $"{Root}.Infrastructure");

    [Fact]
    public void Application_DoesNotDependOnInfrastructure() =>
        AssertNoDependency(Application, $"{Root}.Infrastructure", "Microsoft.EntityFrameworkCore");

    [Fact]
    public void Gateway_OnlyKnowsTheModuleThroughContractsOrProtocol()
    {
        var gateway = typeof(Template.Api.Segments).Assembly;
        AssertNoDependency(gateway, $"{Root}.Domain", $"{Root}.Application", $"{Root}.Infrastructure");
    }

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
