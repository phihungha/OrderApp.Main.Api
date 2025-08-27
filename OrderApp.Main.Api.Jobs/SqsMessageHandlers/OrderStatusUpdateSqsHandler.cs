using AWS.Messaging;
using FluentResults;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;

namespace OrderApp.Main.Api.WebApi.SqsMessageHandlers
{
    public record OrderStatusUpdateMessageDto
    {
        public const string MessageType = "Order.StatusUpdate";

        public enum StatusOptions
        {
            Shipping,
            Shipped,
            Completed,
        }

        public required int OrderId { get; set; }
        public required StatusOptions Status { get; set; }
    }

    public class OrderStatusUpdateSqsHandler(IOrderService orderService)
        : IMessageHandler<OrderStatusUpdateMessageDto>
    {
        private readonly IOrderService orderService = orderService;

        public async Task<MessageProcessStatus> HandleAsync(
            MessageEnvelope<OrderStatusUpdateMessageDto> messageEnvelope,
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
                OrderStatusUpdateMessageDto.StatusOptions.Shipping =>
                    await orderService.BeginShipping(orderId),
                OrderStatusUpdateMessageDto.StatusOptions.Shipped =>
                    await orderService.FinishShipping(orderId),
                OrderStatusUpdateMessageDto.StatusOptions.Completed => await orderService.Complete(
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
