# Acme.Core.Api Tests

Unit tests for the background jobs of `src/Host/Acme.Core.Api/Jobs` (assembly `Acme.Core.Api.Test`; the host's
internals are visible to it). No network, database or real clock.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Jobs/JobTestContext.cs` | helper | Per test: own `IMeterFactory`, `FakeTimeProvider`, `MetricCollector`s for every `acme.job.*` instrument, a span listener, a unique job name and a faked `IJobExecutionContext` |
| `Jobs/MonitoredJobTests.cs` | test class | A delegate-backed plain job run through `MonitoredJob<TJob>`: outcomes `success`/`idle`/`failed`/`cancelled` from `context.Result` or exceptions, duration from the clock, last-success gauge, `error.type` from the error code or exception type, `JobExecutionException` without refire, active count back to zero, new root span with tags and status |
| `Jobs/JobSchedulerHostedServiceTests.cs` | test class | Both jobs scheduled every 5 minutes inside `MonitoredJob<TJob>` with `[DisallowConcurrentExecution]` kept, a disabled job is skipped, `Jobs:Enabled = false` creates no scheduler, misfire listener registered, shutdown on stop |
| `Jobs/JobMisfireListenerTests.cs` | test class | A misfire is counted for its job |
| `Jobs/JobsOptionsValidatorTests.cs` | test class | Defaults valid; invalid cron reported per job |

## How it works

- Metrics are asserted with `MetricCollector<T>` (`Microsoft.Extensions.Diagnostics.Testing`) on a meter factory
  owned by the test.
- Durations come from `FakeTimeProvider` (`Microsoft.Extensions.TimeProvider.Testing`).
- Spans are captured by an `ActivityListener` and filtered by the test's unique job name, so tests can run in
  parallel.
- Scheduler tests use `NullLoggerFactory`, because Quartz keeps the logger factory in a static log provider. They run
  in one collection, because Quartz keeps schedulers in a process-wide repository by name.

## Running

```powershell
dotnet test .\test\Host\Acme.Core.Api\Acme.Core.Api.Test.csproj
```

## Dependencies
- References: `src/Host/Acme.Core.Api`.
- Packages: `xunit.v3`, `FakeItEasy`, `Microsoft.Extensions.Diagnostics.Testing`,
  `Microsoft.Extensions.TimeProvider.Testing`.
