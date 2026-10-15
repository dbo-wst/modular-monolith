using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering;
// using Template.Api.Infrastructure.CoreApi.Prelaunch.Allocation;

// using Template.Api.Infrastructure.CoreApi.Prelaunch.ClientSide;
// using Template.Api.Infrastructure.CoreApi.Prelaunch.MasterData;

namespace Template.Api.Infrastructure.CoreApi;

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
