using System.Collections.Immutable;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderDto
    {
        public required int Id { get; set; }

        public required OrderStatus Status { get; set; }
        public required bool IsFinished { get; set; }
        public IReadOnlyList<OrderEventDto>? Events { get; set; }

        public required string ShippingAddress { get; set; }

        public IReadOnlyList<OrderLineDto>? Lines { get; set; }

        public required decimal TotalAmount { get; set; }

        public static OrderDto FromEntity(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                IsFinished = order.IsFinished,
                Events = order.Events?.Select(OrderEventDto.FromEntity).ToImmutableList(),
                ShippingAddress = order.ShippingAddress,
                Lines = order.Lines?.Select(OrderLineDto.FromEntity).ToImmutableList(),
                TotalAmount = order.TotalAmount,
            };
        }
    }
}
