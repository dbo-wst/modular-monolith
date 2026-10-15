using Microsoft.Extensions.DependencyInjection;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Ordering handlers and validators. The shared <see cref="IMediator"/> is registered once
    /// by the host via <see cref="Template.Core.BuildingBlocks.Application.Communication.ServiceCollectionExtensions.AddMediator"/>.
    /// </summary>
    public static IServiceCollection AddOrderingApplication(this IServiceCollection services) =>
        services.AddRequestHandlersFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
}
