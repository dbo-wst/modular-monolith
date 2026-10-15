using Microsoft.Extensions.DependencyInjection;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Ordering handlers and validators. The shared <see cref="IMediator"/> is registered once
    /// by the host via <see cref="Template.Core.Shared.Kernel.Communication.ServiceCollectionExtensions.AddMediator"/>.
    /// </summary>
    public static IServiceCollection AddOrderingApplication(this IServiceCollection services) =>
        services.AddRequestHandlersFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
}
