using FakeItEasy;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
// using BatchService = Wst.Trios.Core.Protocol.Prelaunch.Allocation.Batch.BatchService;
using BrokerSideOrderService = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order.OrderService;
using BrokerSideTradeService = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade.TradeService;
// using ClientSideOrderService = Wst.Trios.Core.Protocol.Prelaunch.ClientSide.Order.OrderService;
// using ClientSideTradeService = Wst.Trios.Core.Protocol.Prelaunch.ClientSide.Trade.TradeService;
// using IAllocationStore = Wst.Trios.Core.Allocation.Domain.IStore;
using IBrokerSideStore = Wst.Trios.Core.BrokerSide.Domain.IStore;
// using IClientSideStore = Wst.Trios.Core.ClientSide.Domain.IStore;
// using IMasterDataStore = Wst.Trios.Core.MasterData.Domain.IStore;
// using LineService = Wst.Trios.Core.Protocol.Prelaunch.Allocation.Line.LineService;
// using MasterDataBrokerService = Wst.Trios.Core.Protocol.Prelaunch.MasterData.Broker.BrokerService;
// using MasterDataClientService = Wst.Trios.Core.Protocol.Prelaunch.MasterData.Client.ClientService;
// using MasterDataInstrumentService = Wst.Trios.Core.Protocol.Prelaunch.MasterData.Instrument.InstrumentService;
// using MasterDataVenueService = Wst.Trios.Core.Protocol.Prelaunch.MasterData.Venue.VenueService;

namespace Wst.Trios.Core.Api.IntegrationTest;

public sealed class CoreApiFactory : WebApplicationFactory<Program>
{
    public IBrokerSideStore BrokerSideStore { get; } = A.Fake<IBrokerSideStore>();

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
            services.RemoveAll<IBrokerSideStore>();
            services.AddScoped<IBrokerSideStore>(_ => BrokerSideStore);

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

    public BrokerSideOrderService.OrderServiceClient CreateBrokerSideOrderClient() =>
        new(CreateGrpcChannel());

    public BrokerSideTradeService.TradeServiceClient CreateBrokerSideTradeClient() =>
        new(CreateGrpcChannel());

    // public ClientSideOrderService.OrderServiceClient CreateClientSideOrderClient() =>
    //     new(CreateGrpcChannel());
    //
    // public ClientSideTradeService.TradeServiceClient CreateClientSideTradeClient() =>
    //     new(CreateGrpcChannel());
    //
    // public BatchService.BatchServiceClient CreateBatchClient() =>
    //     new(CreateGrpcChannel());
    //
    // public LineService.LineServiceClient CreateLineClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataVenueService.VenueServiceClient CreateMasterDataVenueClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataClientService.ClientServiceClient CreateMasterDataClientClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataBrokerService.BrokerServiceClient CreateMasterDataBrokerClient() =>
    //     new(CreateGrpcChannel());
    //
    // public MasterDataInstrumentService.InstrumentServiceClient CreateMasterDataInstrumentClient() =>
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
