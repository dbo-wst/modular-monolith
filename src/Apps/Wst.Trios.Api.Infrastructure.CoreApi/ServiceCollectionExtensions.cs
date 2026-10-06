using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide;
// using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.Allocation;

// using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.ClientSide;
// using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.MasterData;

namespace Wst.Trios.Api.Infrastructure.CoreApi;

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

        services.AddBrokerSideGrpcClients(configuration);

        return services;
    }
}
