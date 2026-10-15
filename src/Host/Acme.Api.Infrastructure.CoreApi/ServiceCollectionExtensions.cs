using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering;
// using Acme.Api.Infrastructure.CoreApi.Prelaunch.Allocation;

// using Acme.Api.Infrastructure.CoreApi.Prelaunch.ClientSide;
// using Acme.Api.Infrastructure.CoreApi.Prelaunch.MasterData;

namespace Acme.Api.Infrastructure.CoreApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApiInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CoreApiOptions>()
            .Bind(configuration.GetSection(CoreApiOptions.SectionName))
            .Validate(
                options => Uri.IsWellFormedUriString(options.BaseUrl, UriKind.Absolute),
                $"{CoreApiOptions.SectionName}:{nameof(CoreApiOptions.BaseUrl)} must be a valid absolute URL.");

        services.AddOrderingGrpcClients(configuration);

        return services;
    }
}
