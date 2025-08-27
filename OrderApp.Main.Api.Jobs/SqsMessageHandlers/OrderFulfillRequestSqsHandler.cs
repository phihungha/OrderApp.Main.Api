using AWS.Messaging;
using Hangfire;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs;

namespace OrderApp.Main.Api.Jobs.SqsMessageHandlers
{
    public class OrderFulfillRequestSqsHandler(IBackgroundJobClient backgroundJobClient)
        : IMessageHandler<OrderFulfillRequestMessageDto>
    {
        private readonly IBackgroundJobClient backgroundJobClient = backgroundJobClient;

        public Task<MessageProcessStatus> HandleAsync(
            MessageEnvelope<OrderFulfillRequestMessageDto> messageEnvelope,
            CancellationToken token = default
        )
        {
            if (messageEnvelope == null || messageEnvelope.Message == null)
            {
                return Task.FromResult(MessageProcessStatus.Failed());
            }

            var orderId = messageEnvelope.Message.OrderId;

            backgroundJobClient.Enqueue<IOrderService>(s => s.Fulfill(orderId));

            return Task.FromResult(MessageProcessStatus.Success());
        }
    }
}
