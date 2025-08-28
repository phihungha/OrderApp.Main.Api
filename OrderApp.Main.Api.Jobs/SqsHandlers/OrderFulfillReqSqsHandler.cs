using AWS.Messaging;
using Hangfire;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs;

namespace OrderApp.Main.Api.Jobs.SqsHandlers
{
    public class OrderFulfillReqSqsHandler(IBackgroundJobClient backgroundJobClient)
        : IMessageHandler<OrderFulfillReqMessageDto>
    {
        private readonly IBackgroundJobClient backgroundJobClient = backgroundJobClient;

        public Task<MessageProcessStatus> HandleAsync(
            MessageEnvelope<OrderFulfillReqMessageDto> messageEnvelope,
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
