using Microsoft.Extensions.DependencyInjection;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Ordering handlers and validators. The shared <see cref="IMediator"/> is registered once
    /// by the host via <see cref="Acme.Core.BuildingBlocks.Application.Communication.ServiceCollectionExtensions.AddMediator"/>.
    /// </summary>
    public static IServiceCollection AddOrderingApplication(this IServiceCollection services) =>
        services.AddRequestHandlersFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
}
