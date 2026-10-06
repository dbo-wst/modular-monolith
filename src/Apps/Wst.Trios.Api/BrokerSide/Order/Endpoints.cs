using LanguageExt.Common;
using Wst.Trios.Api.Infrastructure.CoreApi;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Responses;

namespace Wst.Trios.Api.BrokerSide.Order;

public static class Endpoints
{
    private const string Path = $"/{Segments.BrokerSide}/orders";
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Path)
            .WithTags("Core.BrokerSide.Order");

        group.MapPost("/", async (CreateOrderRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.CreateAsync(body, ct);
                return result.Match(
                    Succ: response => Results.Created($"{Path}/{response.Order.OrderId}", response),
                    Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.GetAsync(new GetOrderRequest(id), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.GetOrder")
            .Produces<GetOrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (IGrpcApiClient client, CancellationToken ct, int pageNumber = 1, int pageSize = 10) =>
            {
                var result = await client.ListAsync(new GetOrdersRequest(pageNumber, pageSize), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.ListOrders")
            .Produces<GetOrdersResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", async (UpdateOrderRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.UpdateAsync(body, ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.UpdateOrder")
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
            .WithName("Core.BrokerSide.DeleteOrder")
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
