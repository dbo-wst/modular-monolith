using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Acme.Core.BuildingBlocks.Application.Validation;

namespace Acme.Core.Api.Jobs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Binds and validates section <c>Jobs</c> and registers an in-memory Quartz scheduler (Core.Api runs as a single
    /// instance, so no shared job store is needed) that <see cref="JobSchedulerHostedService"/> fills and starts
    /// unless <c>Jobs:Enabled</c> is false.
    /// </summary>
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IValidator<JobsOptions>, JobsOptionsValidator>();
        services.AddOptions<JobsOptions>()
            .Bind(configuration.GetSection(JobsOptions.SectionName))
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.TryAddSingleton(TimeProvider.System);
        services.AddMetrics();
        services.TryAddSingleton<JobMetrics>();
        services.TryAddSingleton<JobMisfireListener>();

        services.AddQuartz(quartz => quartz.SchedulerName = JobsOptions.SchedulerName);
        services.AddHostedService<JobSchedulerHostedService>();

        return services;
    }
}
