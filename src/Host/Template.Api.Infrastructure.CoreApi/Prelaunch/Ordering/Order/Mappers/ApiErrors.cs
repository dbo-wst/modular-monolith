using Template.Core.BuildingBlocks.Domain.Errors;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers;

// Builds an ApiError from a Protocol error's code/message, with a fallback for an empty error.
internal static class ApiErrors
{
    public static ApiError From(string? code, string? message) =>
        code is null
            ? ApiError.New(ErrorCodes.Internal, "The Core API returned an empty error.")
            : ApiError.New(code, message ?? string.Empty);
}
