using AWS.Messaging.Publishers.SQS;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Infrastructure.JobStart.MessageDTOs;

namespace OrderApp.Main.Api.Infrastructure.JobStart
{
    public class JobStartService(ISQSPublisher sqsPublisher) : IJobStartService
    {
        private readonly ISQSPublisher sqsPublisher = sqsPublisher;

        public async Task FulfillOrder(int orderId)
        {
            var messageBody = new OrderFulfillReqMessageDto() { OrderId = orderId };
            await sqsPublisher.SendAsync(messageBody);
        }
    }
}
