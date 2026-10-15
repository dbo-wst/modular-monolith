using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;

namespace Template.Api.IntegrationTest;

public sealed class GatewayApiFactory : WebApplicationFactory<Program>
{
    public class Ordering
    {
        public static class Order
        {
            public static IGrpcApiClient GrpcApiClient { get; } = A.Fake<IGrpcApiClient>();

        }
        public static class OrderItem
        {
            public static Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.IGrpcApiClient>();
        }
    }

    // public static class ClientSide
    // {
    //     public static class Order
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>();
    //
    //     }
    //     public static class OrderItem
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.ClientSide.OrderItem.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.ClientSide.OrderItem.IGrpcApiClient>();
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
    //     public static class Group
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Group.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Group.IGrpcApiClient>();
    //     }
    //     public static class Client
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>();
    //     }
    //     public static class Owner
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Owner.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Owner.IGrpcApiClient>();
    //     }
    //     public static class Category
    //     {
    //         public static Infrastructure.CoreApi.Prelaunch.MasterData.Category.IGrpcApiClient GrpcApiClient { get; } = A.Fake<Infrastructure.CoreApi.Prelaunch.MasterData.Category.IGrpcApiClient>();
    //     }
    // }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IGrpcApiClient>();
            services.AddScoped<IGrpcApiClient>(_ => Ordering.Order.GrpcApiClient);

            services.RemoveAll<Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.IGrpcApiClient>();
            services.AddScoped<Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.IGrpcApiClient>(_ => Ordering.OrderItem.GrpcApiClient);

            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.ClientSide.Order.IGrpcApiClient>(_ => ClientSide.Order.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.ClientSide.OrderItem.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.ClientSide.OrderItem.IGrpcApiClient>(_ => ClientSide.OrderItem.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.Allocation.Batch.IGrpcApiClient>(_ => Allocation.Batch.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.Allocation.Line.IGrpcApiClient>(_ => Allocation.Line.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Group.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Group.IGrpcApiClient>(_ => MasterData.Group.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Client.IGrpcApiClient>(_ => MasterData.Client.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Owner.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Owner.IGrpcApiClient>(_ => MasterData.Owner.GrpcApiClient);
            //
            // services.RemoveAll<Infrastructure.CoreApi.Prelaunch.MasterData.Category.IGrpcApiClient>();
            // services.AddScoped<Infrastructure.CoreApi.Prelaunch.MasterData.Category.IGrpcApiClient>(_ => MasterData.Category.GrpcApiClient);

        });
    }
}
