using Acme.Core.BuildingBlocks.Domain.Errors;
using Microsoft.Extensions.DependencyInjection;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Persistence;

namespace Acme.Core.Ordering.Application.Test;

public class MediatorPipelineTests
{
    private static IMediator CreateMediator()
    {
        var factory = Support.Infrastructure.DbContextFactory();
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddOrderingApplication();
        services.AddScoped(_ => factory.CreateDbContext());
        services.AddScoped<DomainEventCollector>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<Domain.Orders.IOrderRepository, Persistence.Orders.OrderRepository>();
        services.AddScoped<Domain.OrderItems.IOrderItemRepository, Persistence.OrderItems.OrderItemRepository>();
        services.AddScoped<IOrderingReadStore>(_ => new ReadStore(factory));
        return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Command_WithInvalidInput_FailsValidationBeforeReachingTheHandler()
    {
        var result = await CreateMediator().SendAsync(new CreateOrderCommand(new string('x', 129), null));

        Assert.True(result.IsFail);
        var error = Assert.IsType<ValidationError>(result.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.Validation, error.ErrorCode);
    }

    [Fact]
    public async Task Command_WithValidInput_IsHandled()
    {
        var result = await CreateMediator().SendAsync(new CreateOrderCommand("ORD-1", new DateOnly(2026, 7, 10)));

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task OrderItemCommand_WithNegativeQuantity_FailsValidation()
    {
        var result = await CreateMediator().SendAsync(new CreateOrderItemCommand(Guid.NewGuid(), -1m, 1m));

        Assert.True(result.IsFail);
    }
}
