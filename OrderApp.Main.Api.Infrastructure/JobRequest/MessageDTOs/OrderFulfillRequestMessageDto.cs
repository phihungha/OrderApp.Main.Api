namespace OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs
{
    public record OrderFulfillRequestMessageDto
    {
        public const string MessageType = "Order.FulfillRequest";

        public required int OrderId { get; set; }
    }
}
