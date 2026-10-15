using FakeItEasy;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
// using BatchService = Template.Core.Protocol.Prelaunch.Allocation.Batch.BatchService;
using OrderingOrderService = Template.Core.Protocol.Prelaunch.Ordering.Order.OrderService;
using OrderingOrderItemService = Template.Core.Protocol.Prelaunch.Ordering.OrderItem.OrderItemService;
// using ClientSideOrderService = Template.Core.Protocol.Prelaunch.ClientSide.Order.OrderService;
// using ClientSideOrderItemService = Template.Core.Protocol.Prelaunch.ClientSide.OrderItem.OrderItemService;
// using IAllocationStore = Template.Core.Allocation.Domain.IStore;
using IUnitOfWork = Template.Core.Ordering.Domain.Abstractions.IUnitOfWork;
using IOrderRepository = Template.Core.Ordering.Domain.Orders.IOrderRepository;
using IOrderItemRepository = Template.Core.Ordering.Domain.OrderItems.IOrderItemRepository;
using IOrderingReadStore = Template.Core.Ordering.Application.IOrderingReadStore;
// using IClientSideStore = Template.Core.ClientSide.Domain.IStore;
// using IMasterDataStore = Template.Core.MasterData.Domain.IStore;
// using LineService = Template.Core.Protocol.Prelaunch.Allocation.Line.LineService;
// using MasterDataOwnerService = Template.Core.Protocol.Prelaunch.MasterData.Owner.OwnerService;
// using MasterDataClientService = Template.Core.Protocol.Prelaunch.MasterData.Client.ClientService;
// using MasterDataCategoryService = Template.Core.Protocol.Prelaunch.MasterData.Category.CategoryService;
// using MasterDataGroupService = Template.Core.Protocol.Prelaunch.MasterData.Group.GroupService;

namespace Template.Core.Api.IntegrationTest;

public sealed class CoreApiFactory : WebApplicationFactory<Program>
{
    public IUnitOfWork OrderingUnitOfWork { get; } = A.Fake<IUnitOfWork>();

    public IOrderRepository OrderRepository { get; } = A.Fake<IOrderRepository>();

    public IOrderItemRepository OrderItemRepository { get; } = A.Fake<IOrderItemRepository>();

    public IOrderingReadStore OrderingReadStore { get; } = A.Fake<IOrderingReadStore>();

    // public IClientSideStore ClientSideStore { get; } = A.Fake<IClientSideStore>();
    //
    // public IAllocationStore AllocationStore { get; } = A.Fake<IAllocationStore>();
    //
    // public IMasterDataStore MasterDataStore { get; } = A.Fake<IMasterDataStore>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork>(_ => OrderingUnitOfWork);

            services.RemoveAll<IOrderRepository>();
            services.AddScoped<IOrderRepository>(_ => OrderRepository);

            services.RemoveAll<IOrderItemRepository>();
            services.AddScoped<IOrderItemRepository>(_ => OrderItemRepository);

            services.RemoveAll<IOrderingReadStore>();
            services.AddScoped<IOrderingReadStore>(_ => OrderingReadStore);

            // services.RemoveAll<IClientSideStore>();
            // services.AddScoped<IClientSideStore>(_ => ClientSideStore);
            //
            // services.RemoveAll<IAllocationStore>();
            // services.AddScoped<IAllocationStore>(_ => AllocationStore);
            //
            // services.RemoveAll<IMasterDataStore>();
            // services.AddScoped<IMasterDataStore>(_ => MasterDataStore);
        });
    }

    public OrderingOrderService.OrderServiceClient CreateOrderingOrderClient() =>
        new(CreateGrpcChannel());

    public OrderingOrderItemService.OrderItemServiceClient CreateOrderingOrderItemClient() =>
        new(CreateGrpcChannel());

    // public ClientSideOrderService.OrderServiceClient CreateClientSideOrderClient() =>
    //     new(CreateGrpcChannel());
    //
    // public ClientSideOrderItemService.OrderItemServiceClient CreateClientSideOrderItemClient() =>
    //     new(CreateGrpcChannel());
    //
    // public BatchService.BatchServiceClient CreateBatchClient() =>
    //     new(CreateGrpcChannel());
    //
    // public LineService.LineServiceClient CreateLineClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataGroupService.GroupServiceClient CreateMasterDataGroupClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataClientService.ClientServiceClient CreateMasterDataClientClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataOwnerService.OwnerServiceClient CreateMasterDataOwnerClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataCategoryService.CategoryServiceClient CreateMasterDataCategoryClient() =>
    //     new(CreateGrpcChannel());

    private GrpcChannel CreateGrpcChannel()
    {
        var handler = new ResponseVersionHandler { InnerHandler = Server.CreateHandler() };
        return GrpcChannel.ForAddress(Server.BaseAddress, new GrpcChannelOptions
        {
            HttpHandler = handler
        });
    }

    private sealed class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;
            return response;
        }
    }
}
