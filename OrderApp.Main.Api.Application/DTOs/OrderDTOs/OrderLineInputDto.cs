namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderLineInputDto
    {
        public required int ProductId { get; init; }
        public required int Quantity { get; init; }
    }
}
