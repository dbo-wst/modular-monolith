using FluentValidation;
using Quartz;

namespace Acme.Core.Api.Jobs;

public sealed class JobsOptionsValidator : AbstractValidator<JobsOptions>
{
    public JobsOptionsValidator()
    {
        RuleFor(x => x.Heartbeat).NotNull().SetValidator(new JobScheduleOptionsValidator());
        RuleFor(x => x.Housekeeping).NotNull().SetValidator(new JobScheduleOptionsValidator());
    }
}

public sealed class JobScheduleOptionsValidator : AbstractValidator<JobScheduleOptions>
{
    public JobScheduleOptionsValidator()
    {
        RuleFor(x => x.Cron)
            .NotEmpty()
            .Must(CronExpression.IsValidExpression)
            .WithMessage("'{PropertyName}' must be a valid Quartz cron expression, e.g. '0 0/5 * * * ?'.");
    }
}
