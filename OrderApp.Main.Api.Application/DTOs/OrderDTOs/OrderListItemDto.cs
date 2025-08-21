using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderListItemDto
    {
        public required int Id { get; set; }
        public required OrderStatus Status { get; set; }
        public required bool IsFinished { get; set; }
        public required string ShippingAddress { get; set; }
        public required decimal TotalAmount { get; set; }

        public static OrderListItemDto FromEntity(Order order)
        {
            return new OrderListItemDto
            {
                Id = order.Id,
                Status = order.Status,
                IsFinished = order.IsFinished,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
            };
        }
    }
}
