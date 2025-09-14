namespace OrderApp.Main.Api.Domain.Entities.UserEntities
{
    public record PaymentMethod
    {
        public required string CardNumber { get; set; }
        public required string CardHolderName { get; set; }
        public required string CardExpiry { get; set; }
        public required string CardCvv { get; set; }
    }
}
