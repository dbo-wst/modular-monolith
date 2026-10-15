using LanguageExt.Common;
using Acme.Api.Infrastructure.CoreApi;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Requests;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Responses;

namespace Acme.Api.Ordering.Order;

public static class Endpoints
{
    private const string Path = $"/{Segments.Ordering}/orders";
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Path)
            .WithTags("Core.Ordering.Order");

        group.MapPost("/", async (CreateOrderRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.CreateAsync(body, ct);
                return result.Match(
                    Succ: response => Results.Created($"{Path}/{response.Order.OrderId}", response),
                    Fail: ToProblem);
            })
            .WithName("Core.Ordering.CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.GetAsync(new GetOrderRequest(id), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.GetOrder")
            .Produces<GetOrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (IGrpcApiClient client, CancellationToken ct, int pageNumber = 1, int pageSize = 10) =>
            {
                var result = await client.ListAsync(new GetOrdersRequest(pageNumber, pageSize), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.ListOrders")
            .Produces<GetOrdersResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", async (UpdateOrderRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.UpdateAsync(body, ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.UpdateOrder")
            .Produces<UpdateOrderResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.DeleteAsync(new DeleteOrderRequest(id), ct);
                return result.Match(Succ: _ => Results.NoContent(), Fail: ToProblem);
            })
            .WithName("Core.Ordering.DeleteOrder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static IResult ToProblem(Error error)
    {
        var code = error is ApiError apiError ? apiError.ErrorCode : ErrorCodes.Internal;
        return Results.Problem(detail: error.Message, statusCode: StatusFor(code), title: code);
    }

    private static int StatusFor(string errorCode) => errorCode switch
    {
        ErrorCodes.NotFound => StatusCodes.Status404NotFound,
        ErrorCodes.Validation => StatusCodes.Status400BadRequest,
        ErrorCodes.Conflict => StatusCodes.Status409Conflict,
        ErrorCodes.Cancelled => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
