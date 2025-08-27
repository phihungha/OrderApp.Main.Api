using Hangfire;
using Hangfire.PostgreSql;
using OrderApp.Main.Api.Application;
using OrderApp.Main.Api.Infrastructure;
using OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs;
using OrderApp.Main.Api.Jobs.SqsMessageHandlers;
using OrderApp.Main.Api.WebApi.SqsMessageHandlers;

var builder = Host.CreateApplicationBuilder(args);

var jobsConnectionString =
    builder.Configuration.GetConnectionString("Jobs")
    ?? throw new InvalidOperationException("ConnectionStrings:Jobs is not configured.");
builder.Services.AddHangfire(provider =>
    provider.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(jobsConnectionString))
);
builder.Services.AddHangfireServer();

builder.AddInfrastructureServices();
builder.AddApplicationServices();

var orderUpdatesSqsUrl =
    builder.Configuration.GetValue<string>("AwsSqs:OrderUpdates:Url")
    ?? throw new InvalidOperationException("AwsSqs:OrderUpdates:Url is not configured.");
var orderFulfillReqsSqsUrl =
    builder.Configuration.GetValue<string>("AwsSqs:OrderFulfillRequests:Url")
    ?? throw new InvalidOperationException("AwsSqs:OrderFulfillRequests:Url is not configured.");

builder.Services.AddAWSMessageBus(busBuilder =>
{
    busBuilder.AddSQSPublisher<OrderFulfillRequestMessageDto>(
        orderFulfillReqsSqsUrl,
        OrderFulfillRequestMessageDto.MessageType
    );

    busBuilder.AddSQSPoller(
        orderFulfillReqsSqsUrl,
        options =>
        {
            options.MaxNumberOfConcurrentMessages = 10;
            options.WaitTimeSeconds = 20;
        }
    );

    busBuilder.AddMessageHandler<OrderFulfillRequestSqsHandler, OrderFulfillRequestMessageDto>(
        OrderFulfillRequestMessageDto.MessageType
    );

    busBuilder.AddSQSPoller(
        orderUpdatesSqsUrl,
        options =>
        {
            options.MaxNumberOfConcurrentMessages = 10;
            options.WaitTimeSeconds = 20;
        }
    );

    busBuilder.AddMessageHandler<OrderStatusUpdateSqsHandler, OrderStatusUpdateMessageDto>(
        OrderStatusUpdateMessageDto.MessageType
    );
});

var app = builder.Build();

app.Run();
