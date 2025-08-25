using AWS.Messaging;
using FluentResults;
using OrderApp.Main.Api.Application.Interfaces;

namespace OrderApp.Main.Api.Infrastructure.SqsMessageHandler
{
    public record OrderStatusUpdateMessage
    {
        public static string MessageType => "Order.StatusUpdate";

        public enum StatusOptions
        {
            Shipping,
            Shipped,
            Completed,
        }

        public required int OrderId { get; set; }
        public required StatusOptions Status { get; set; }
    }

    public class SqsOrderUpdateHandler(IOrderService orderService)
        : IMessageHandler<OrderStatusUpdateMessage>
    {
        private readonly IOrderService orderService = orderService;

        public async Task<MessageProcessStatus> HandleAsync(
            MessageEnvelope<OrderStatusUpdateMessage> messageEnvelope,
            CancellationToken token = default
        )
        {
            if (messageEnvelope == null || messageEnvelope.Message == null)
            {
                return MessageProcessStatus.Failed();
            }

            var orderStatus = messageEnvelope.Message.Status;
            var orderId = messageEnvelope.Message.OrderId;

            var result = orderStatus switch
            {
                OrderStatusUpdateMessage.StatusOptions.Shipping => await orderService.BeginShipping(
                    orderId
                ),
                OrderStatusUpdateMessage.StatusOptions.Shipped => await orderService.FinishShipping(
                    orderId
                ),
                OrderStatusUpdateMessage.StatusOptions.Completed => await orderService.Complete(
                    orderId
                ),
                _ => Result.Fail("Invalid Status."),
            };

            if (result.IsSuccess)
            {
                return MessageProcessStatus.Success();
            }
            return MessageProcessStatus.Failed();
        }
    }
}
