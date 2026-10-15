using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentValidation;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Persistence;
using Acme.Core.BuildingBlocks.Application.Validation;
using Acme.Core.Ordering.Persistence.DbContext;

namespace Acme.Core.Ordering.Persistence;

public static class ServiceCollectionExtensions
{
    public static void AddOrderingPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IValidator<PersistenceOptions>, PersistenceOptionsValidator>();
        services.AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection("Persistence"))
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.AddPooledDbContextFactory<OrderingDbContext>((s, b) =>
        {
            var config = s.GetRequiredService<IOptions<PersistenceOptions>>().Value;
            var connectionString = config.ConnectionString;

            b.UseSqlServer(
                connectionString,
                o =>
                {
                    o.EnableRetryOnFailure();
                    o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema);
                });
        });

        services.AddSingleton<IOrderingDbContextFactory, OrderingDbContextFactory>();
        // One OrderingDbContext per scope, shared by the unit of work and both repositories.
        services.AddScoped(sp => sp.GetRequiredService<IOrderingDbContextFactory>().CreateDbContext());
        // Aggregates touched by the repositories in this scope; their events are dispatched by the unit of work
        // (IDomainEventDispatcher is registered by the host via AddMediator).
        services.TryAddScoped<DomainEventCollector>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<Domain.Orders.IOrderRepository, Orders.OrderRepository>();
        services.AddScoped<Domain.OrderItems.IOrderItemRepository, OrderItems.OrderItemRepository>();
        services.AddScoped<Application.IOrderingReadStore, ReadStore>();
   }
}
