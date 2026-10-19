using System.Diagnostics;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Logs every request that ends in a failed <see cref="Fin{A}"/>, once. Expected outcomes (<c>not_found</c>,
/// <c>validation</c>, <c>conflict</c>, <c>cancelled</c>) are logged as Warning; everything else as Error with the
/// exception the error carries, and the current span is marked as failed. The result is returned unchanged;
/// exceptions thrown by handlers are not caught here.
/// </summary>
public sealed partial class FailureLoggingBehavior(ILogger<FailureLoggingBehavior> logger) : IPipelineBehavior
{
    private static readonly string[] RejectionCodes =
        [ErrorCodes.NotFound, ErrorCodes.Validation, ErrorCodes.Conflict, ErrorCodes.Cancelled];

    public async Task<Fin<TResponse>> HandleAsync<TResponse>(
        IRequest<TResponse> request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        var result = await next();
        result.IfFail(error => Log(request.GetType().Name, error));
        return result;
    }

    private void Log(string requestName, Error error)
    {
        var code = ErrorCodes.Of(error);
        if (RejectionCodes.Contains(code))
        {
            RequestRejected(logger, requestName, code, error.Message);
            return;
        }

        RequestFailed(logger, ExceptionOf(error), requestName, code, error.Message);
        Activity.Current?.SetStatus(ActivityStatusCode.Error).SetTag("error.type", error.GetType().Name);
    }

    private static Exception? ExceptionOf(Error error) =>
        error.Exception.MatchUnsafe(exception => exception, () => error.Inner.MatchUnsafe(ExceptionOf, () => null));

    [LoggerMessage(EventId = 1001, EventName = "RequestRejected", Level = LogLevel.Warning,
        Message = "{RequestName} rejected: {ErrorCode} {ErrorMessage}")]
    private static partial void RequestRejected(ILogger logger, string requestName, string errorCode, string errorMessage);

    [LoggerMessage(EventId = 1002, EventName = "RequestFailed", Level = LogLevel.Error,
        Message = "{RequestName} failed: {ErrorCode} {ErrorMessage}")]
    private static partial void RequestFailed(
        ILogger logger, Exception? exception, string requestName, string errorCode, string errorMessage);
}
