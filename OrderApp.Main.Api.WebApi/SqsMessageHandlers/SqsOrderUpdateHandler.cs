using AWS.Messaging;
using FluentResults;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;

namespace OrderApp.Main.Api.WebApi.SqsMessageHandlers
{
    public record OrderUpdateMessage
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
        : IMessageHandler<OrderUpdateMessage>
    {
        private readonly IOrderService orderService = orderService;

        public async Task<MessageProcessStatus> HandleAsync(
            MessageEnvelope<OrderUpdateMessage> messageEnvelope,
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
                OrderUpdateMessage.StatusOptions.Shipping => await orderService.BeginShipping(
                    orderId
                ),
                OrderUpdateMessage.StatusOptions.Shipped => await orderService.FinishShipping(
                    orderId
                ),
                OrderUpdateMessage.StatusOptions.Completed => await orderService.Complete(orderId),
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
