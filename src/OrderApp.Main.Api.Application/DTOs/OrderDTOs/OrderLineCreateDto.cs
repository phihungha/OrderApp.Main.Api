namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderLineCreateDto
    {
        public required int ProductId { get; init; }
        public required int Quantity { get; init; }
    }
}
