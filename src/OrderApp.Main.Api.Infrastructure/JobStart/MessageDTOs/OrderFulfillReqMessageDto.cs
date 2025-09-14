namespace OrderApp.Main.Api.Infrastructure.JobStart.MessageDTOs
{
    public record OrderFulfillReqMessageDto
    {
        public const string MessageType = "Order.FulfillRequest";

        public required int OrderId { get; set; }
    }
}
