using OrderApp.Main.Api.WebApi.SqsMessageHandlers;

namespace OrderApp.Main.Api.WebApi.SqsHandlers
{
    public static class Setup
    {
        public static void AddSqsHandlers(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;

            var awsSqsUrls =
                builder.Configuration.GetSection("AwsSqs:Urls").Get<IDictionary<string, string>>()
                ?? throw new InvalidOperationException("AwsSqs:Urls is not configured.");

            var orderUpdatesSqsUrl =
                awsSqsUrls["OrderUpdates"]
                ?? throw new InvalidOperationException(
                    "AwsSqs:Urls:OrderUpdates is not configured."
                );
            builder.Services.AddAWSMessageBus(builder =>
            {
                builder.AddSQSPoller(
                    orderUpdatesSqsUrl,
                    options =>
                    {
                        options.MaxNumberOfConcurrentMessages = 10;
                        options.WaitTimeSeconds = 20;
                    }
                );

                builder.AddMessageHandler<SqsOrderUpdateHandler, OrderUpdateMessage>(
                    OrderUpdateMessage.MessageType
                );
            });
        }
    }
}
