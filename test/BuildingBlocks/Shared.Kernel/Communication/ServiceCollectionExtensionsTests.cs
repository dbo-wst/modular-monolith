using FluentValidation;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Shared.Kernel.Test.Communication;

public class ServiceCollectionExtensionsTests
{
    public sealed record Ping(string Text) : IQuery<string>;

    public sealed class PingHandler : IQueryHandler<Ping, string>
    {
        public Task<Fin<string>> HandleAsync(Ping request, CancellationToken ct = default) =>
            Task.FromResult(Fin<string>.Succ($"pong:{request.Text}"));
    }

    public sealed class PingValidator : AbstractValidator<Ping>
    {
        public PingValidator() => RuleFor(x => x.Text).NotEmpty();
    }

    private static IMediator BuildMediator(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
        return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public void AddMediator_CalledTwice_RegistersOneMediatorAndOneValidationBehavior()
    {
        var services = new ServiceCollection();

        services.AddMediator();
        services.AddMediator();

        Assert.Single(services, d => d.ServiceType == typeof(IMediator));
        Assert.Single(services, d => d.ServiceType == typeof(IPipelineBehavior));
    }

    [Fact]
    public async Task Mediator_DispatchesToHandlersRegisteredFromAnAssembly()
    {
        var mediator = BuildMediator(s => s.AddMediator().AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly));

        var result = await mediator.SendAsync(new Ping("hi"));

        Assert.Equal("pong:hi", result.Match(Succ: v => v, Fail: e => e.Message));
    }

    [Fact]
    public async Task Mediator_RunsValidatorsRegisteredFromAnAssembly()
    {
        var mediator = BuildMediator(s => s.AddMediator().AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly));

        var result = await mediator.SendAsync(new Ping(""));

        Assert.True(result.IsFail);
    }

    [Fact]
    public void AddRequestHandlersFromAssembly_SameAssemblyTwice_DoesNotDuplicateRegistrations()
    {
        var services = new ServiceCollection();

        services.AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly);
        services.AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly);

        Assert.Single(services, d => d.ServiceType == typeof(IRequestHandler<Ping, string>));
        Assert.Single(services, d => d.ServiceType == typeof(IValidator<Ping>));
    }

    [Fact]
    public void AddRequestHandlersFromAssembly_SecondHandlerForSameRequest_Throws()
    {
        var services = new ServiceCollection();
        services.AddScoped<IRequestHandler<Ping, string>, OtherPingHandler>();

        Assert.Throws<InvalidOperationException>(() =>
            services.AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly));
    }

    // Abstract so assembly scanning skips it; registered manually to simulate a second module handling Ping.
    private abstract class OtherPingHandler : IRequestHandler<Ping, string>
    {
        public Task<Fin<string>> HandleAsync(Ping request, CancellationToken ct = default) =>
            Task.FromResult(Fin<string>.Succ("other"));
    }
}
