using Microsoft.Extensions.DependencyInjection;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Application.Test;

public class MediatorPipelineTests
{
    private static IMediator CreateMediator()
    {
        var factory = Support.Infrastructure.DbContextFactory();
        var services = new ServiceCollection();
        services.AddApplication(null!);
        services.AddScoped(_ => factory.CreateDbContext());
        services.AddScoped<Domain.Abstractions.IUnitOfWork, UnitOfWork>();
        services.AddScoped<Domain.Orders.IOrderRepository, Infrastructure.Order.OrderRepository>();
        services.AddScoped<Domain.OrderItems.IOrderItemRepository, Infrastructure.OrderItem.OrderItemRepository>();
        services.AddScoped<IOrderingReadStore>(_ => new ReadStore(factory));
        return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task Command_WithInvalidInput_FailsValidationBeforeReachingTheHandler()
    {
        var result = await CreateMediator().SendAsync(new CreateOrderCommand(new string('x', 129), null));

        Assert.True(result.IsFail);
        var message = result.Match(Succ: _ => string.Empty, Fail: e => e.Message);
        Assert.Contains("Validation Error", message);
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
