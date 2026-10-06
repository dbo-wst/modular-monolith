using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.IntegrationTest;

public sealed class GatewayApiFactory : WebApplicationFactory<Program>
{
    public class BrokerSide
    {
        public static class Order
        {
            public static IGrpcApiClient GrpcApiClient { get; } = A.Fake<IGrpcApiClient>();

        }
        public static class Trade
        {
            public static Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.IGrpcApiClient>();
        }
    }

    // public static class ClientSide
    // {
    //     public static class Order
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>();
    //
    //     }
    //     public static class Trade
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.ClientSide.Trade.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.ClientSide.Trade.IGrpcApiClient>();
    //     }
    // }
    //
    // public static class Allocation
    // {
    //     public static class Batch
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient>();
    //
    //     }
    //     public static class Line
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient>();
    //     }
    // }
    //
    // public static class MasterData
    // {
    //     public static class Venue
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Venue.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Venue.IGrpcApiClient>();
    //     }
    //     public static class Client
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>();
    //     }
    //     public static class Broker
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Broker.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Broker.IGrpcApiClient>();
    //     }
    //     public static class Instrument
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Instrument.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Instrument.IGrpcApiClient>();
    //     }
    // }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGrpcApiClient>();
            services.AddScoped<IGrpcApiClient>(_ => BrokerSide.Order.GrpcApiClient);

            services.RemoveAll<Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.IGrpcApiClient>();
            services.AddScoped<Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.IGrpcApiClient>(_ => BrokerSide.Trade.GrpcApiClient);

            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>(_ => ClientSide.Order.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.ClientSide.Trade.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.ClientSide.Trade.IGrpcApiClient>(_ => ClientSide.Trade.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient>(_ => Allocation.Batch.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient>(_ => Allocation.Line.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Venue.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Venue.IGrpcApiClient>(_ => MasterData.Venue.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>(_ => MasterData.Client.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Broker.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Broker.IGrpcApiClient>(_ => MasterData.Broker.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Instrument.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Instrument.IGrpcApiClient>(_ => MasterData.Instrument.GrpcApiClient);

        });
    }
}
