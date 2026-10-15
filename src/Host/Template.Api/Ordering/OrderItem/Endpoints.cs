using LanguageExt.Common;
using Template.Api.Infrastructure.CoreApi;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Responses;

namespace Template.Api.Ordering.OrderItem;

public static class Endpoints
{
    private const string Path = $"/{Segments.Ordering}/order-items";
    public static IEndpointRouteBuilder MapOrderItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Path)
            .WithTags("Core.Ordering.OrderItem");

        group.MapPost("/", async (CreateOrderItemRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.CreateAsync(body, ct);
                return result.Match(
                    Succ: response => Results.Created($"{Path}/{response.OrderItem.OrderItemId}", response),
                    Fail: ToProblem);
            })
            .WithName("Core.Ordering.CreateOrderItem")
            .Produces<CreateOrderItemResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.GetAsync(new GetOrderItemRequest(id), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.GetOrderItem")
            .Produces<GetOrderItemResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (IGrpcApiClient client, CancellationToken ct, int pageNumber = 1, int pageSize = 10) =>
            {
                var result = await client.ListAsync(new GetOrderItemsRequest(pageNumber, pageSize), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.ListOrderItems")
            .Produces<GetOrderItemsResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", async (UpdateOrderItemRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.UpdateAsync(body, ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.Ordering.UpdateOrderItem")
            .Produces<UpdateOrderItemResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.DeleteAsync(new DeleteOrderItemRequest(id), ct);
                return result.Match(Succ: _ => Results.NoContent(), Fail: ToProblem);
            })
            .WithName("Core.Ordering.DeleteOrderItem")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static IResult ToProblem(Error error)
    {
        var code = error is ApiError apiError ? apiError.ErrorCode : "internal";
        return Results.Problem(detail: error.Message, statusCode: StatusFor(code), title: code);
    }

    private static int StatusFor(string errorCode) => errorCode switch
    {
        "not_found" => StatusCodes.Status404NotFound,
        "validation" => StatusCodes.Status400BadRequest,
        "conflict" => StatusCodes.Status409Conflict,
        "cancelled" => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
