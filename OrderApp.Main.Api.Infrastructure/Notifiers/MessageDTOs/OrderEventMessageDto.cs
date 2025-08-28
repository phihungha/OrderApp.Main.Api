using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Infrastructure.Notifiers.MessageDTOs
{
    public class OrderEventMessageDto
    {
        public const string MessageType = "Order.Event";

        public required int OrderId { get; set; }
        public required OrderStatus Status { get; set; }
        public required DateTime Time { get; set; }
    }
}
