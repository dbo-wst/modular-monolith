using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.BuildingBlocks.Application.Test.Communication;

public class FailureLoggingBehaviorTests
{
    public sealed record Probe : IQuery<string>;

    public sealed record Entry(LogLevel Level, int EventId, string Message, Exception? Exception);

    public sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<Entry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add(new Entry(logLevel, eventId.Id, formatter(state, exception), exception));
    }

    public static TheoryData<string> Rejections() =>
        [ErrorCodes.NotFound, ErrorCodes.Validation, ErrorCodes.Conflict, ErrorCodes.Cancelled];

    public static TheoryData<string> Failures() => ["persistence", "untyped exception"];

    private static Error Rejection(string code) => code switch
    {
        ErrorCodes.NotFound => NotFoundError.New("missing"),
        ErrorCodes.Validation => ValidationError.New("invalid"),
        ErrorCodes.Conflict => ConflictError.New("taken"),
        ErrorCodes.Cancelled => CancelledError.New("cancelled"),
        _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
    };

    private static Error Failure(string shape) => shape switch
    {
        "persistence" => PersistenceFailed.New(new InvalidOperationException("db down")),
        "untyped exception" => Error.New(new InvalidOperationException("boom")),
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };

    private static async Task<(Fin<string> Result, RecordingLogger<FailureLoggingBehavior> Logger)> Run(Fin<string> outcome)
    {
        var logger = new RecordingLogger<FailureLoggingBehavior>();
        var result = await new FailureLoggingBehavior(logger).HandleAsync(new Probe(), () => Task.FromResult(outcome));
        return (result, logger);
    }

    [Fact]
    public async Task HandleAsync_Success_LogsNothing()
    {
        var (_, logger) = await Run(Fin<string>.Succ("ok"));

        Assert.Empty(logger.Entries);
    }

    [Theory]
    [MemberData(nameof(Rejections))]
    public async Task HandleAsync_ExpectedFailure_LogsOneWarning(string code)
    {
        var (_, logger) = await Run(Fin<string>.Fail(Rejection(code)));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal((LogLevel.Warning, 1001, $"Probe rejected: {code} {Rejection(code).Message}"),
            (entry.Level, entry.EventId, entry.Message));
    }

    [Theory]
    [MemberData(nameof(Failures))]
    public async Task HandleAsync_InternalFailure_LogsOneErrorWithTheException(string shape)
    {
        var (_, logger) = await Run(Fin<string>.Fail(Failure(shape)));

        var entry = Assert.Single(logger.Entries);
        Assert.Equal((LogLevel.Error, 1002, typeof(InvalidOperationException)),
            (entry.Level, entry.EventId, entry.Exception?.GetType()));
    }

    [Fact]
    public async Task HandleAsync_Failure_ReturnsTheResultUnchanged()
    {
        var outcome = Fin<string>.Fail(NotFoundError.New("missing"));

        var (result, _) = await Run(outcome);

        Assert.Equal(outcome, result);
    }

    [Fact]
    public async Task Mediator_ValidationFailure_IsLoggedAsRejected()
    {
        var logger = new RecordingLogger<FailureLoggingBehavior>();
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddRequestHandlersFromAssembly(typeof(FailureLoggingBehaviorTests).Assembly);
        services.AddSingleton<ILogger<FailureLoggingBehavior>>(logger);
        var mediator = services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IMediator>();

        await mediator.SendAsync(new ServiceCollectionExtensionsTests.Ping(""));

        var entry = Assert.Single(logger.Entries);
        Assert.StartsWith($"Ping rejected: {ErrorCodes.Validation}", entry.Message);
    }
}
