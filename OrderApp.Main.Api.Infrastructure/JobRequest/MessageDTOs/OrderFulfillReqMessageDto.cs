namespace OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs
{
    public record OrderFulfillReqMessageDto
    {
        public const string MessageType = "Order.FulfillRequest";

        public required int OrderId { get; set; }
    }
}
