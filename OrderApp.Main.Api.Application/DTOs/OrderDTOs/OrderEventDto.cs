using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderEventDto
    {
        public required int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }

        public static OrderEventDto FromEntity(OrderEvent orderEvent)
        {
            return new OrderEventDto
            {
                Id = orderEvent.Id,
                Status = orderEvent.Status,
                Timestamp = orderEvent.Timestamp,
            };
        }
    }
}
