using LanguageExt.Common;
using Wst.Trios.Api.Infrastructure.CoreApi;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Responses;

namespace Wst.Trios.Api.BrokerSide.Trade;

public static class Endpoints
{
    private const string Path = $"/{Segments.BrokerSide}/trades";
    public static IEndpointRouteBuilder MapTradeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Path)
            .WithTags("Core.BrokerSide.Trade");

        group.MapPost("/", async (CreateTradeRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.CreateAsync(body, ct);
                return result.Match(
                    Succ: response => Results.Created($"{Path}/{response.Trade.TradeId}", response),
                    Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.CreateTrade")
            .Produces<CreateTradeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.GetAsync(new GetTradeRequest(id), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.GetTrade")
            .Produces<GetTradeResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (IGrpcApiClient client, CancellationToken ct, int pageNumber = 1, int pageSize = 10) =>
            {
                var result = await client.ListAsync(new GetTradesRequest(pageNumber, pageSize), ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.ListTrades")
            .Produces<GetTradesResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/", async (UpdateTradeRequest body, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.UpdateAsync(body, ct);
                return result.Match(Succ: response => Results.Ok(response), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.UpdateTrade")
            .Produces<UpdateTradeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:guid}", async (Guid id, IGrpcApiClient client, CancellationToken ct) =>
            {
                var result = await client.DeleteAsync(new DeleteTradeRequest(id), ct);
                return result.Match(Succ: _ => Results.NoContent(), Fail: ToProblem);
            })
            .WithName("Core.BrokerSide.DeleteTrade")
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
