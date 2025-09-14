namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderCreateDto
    {
        public required string ShippingAddress { get; set; }
        public required ICollection<OrderLineCreateDto> Lines { get; set; }
    }
}
