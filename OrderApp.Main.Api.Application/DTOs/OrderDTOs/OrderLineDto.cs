using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderLineDto
    {
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }

        public required decimal UnitPrice { get; set; }
        public required int Quantity { get; set; }
        public required decimal TotalPrice { get; set; }

        public static OrderLineDto FromEntity(OrderLine orderLine)
        {
            return new OrderLineDto
            {
                ProductId = orderLine.ProductId,
                ProductName = orderLine.Product.Name,
                UnitPrice = orderLine.UnitPrice,
                Quantity = orderLine.Quantity,
                TotalPrice = orderLine.TotalPrice,
            };
        }
    }
}
