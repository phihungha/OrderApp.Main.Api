using System.Configuration;
using OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs;

namespace OrderApp.Main.Api.Jobs.SqsHandlers
{
    internal static class Setup
    {
        public static void AddSqsHandlers(this IHostApplicationBuilder builder)
        {
            var appConfig =
                builder.Configuration.Get<AppConfig>()
                ?? throw new ConfigurationErrorsException("App config is null.");

            var awsSnsConfig = appConfig.AwsSns;
            var awsSqsConfig = appConfig.AwsSqs;

            builder.Services.AddAWSMessageBus(busBuilder =>
            {
                busBuilder.AddSQSPoller(
                    awsSqsConfig.OrderFulfillRequests.QueueUrl,
                    options =>
                    {
                        options.MaxNumberOfConcurrentMessages = 10;
                        options.WaitTimeSeconds = 20;
                    }
                );

                busBuilder.AddMessageHandler<OrderFulfillReqSqsHandler, OrderFulfillReqMessageDto>(
                    OrderFulfillReqMessageDto.MessageType
                );

                busBuilder.AddSQSPoller(
                    awsSqsConfig.OrderUpdates.QueueUrl,
                    options =>
                    {
                        options.MaxNumberOfConcurrentMessages = 10;
                        options.WaitTimeSeconds = 20;
                    }
                );

                busBuilder.AddMessageHandler<
                    OrderStatusUpdateSqsHandler,
                    OrderStatusUpdateMessageDto
                >(OrderStatusUpdateMessageDto.MessageType);
            });
        }
    }
}
