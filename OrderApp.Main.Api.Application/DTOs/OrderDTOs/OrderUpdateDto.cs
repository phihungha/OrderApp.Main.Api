namespace OrderApp.Main.Api.Application.DTOs.OrderDTOs
{
    public record OrderUpdateDto
    {
        public enum StatusOptions
        {
            Canceled,
            Completed,
        }

        public StatusOptions? Status { get; set; }
    }
}
