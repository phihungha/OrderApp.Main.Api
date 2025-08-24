namespace OrderApp.Main.Api.Application.Interfaces.InfrastructureServices
{
    public record PaymentDetails
    {
        public required string CardNumber { get; set; }
        public required string CardHolderName { get; set; }
        public required string CardExpiry { get; set; }
        public required string CardCvv { get; set; }
        public required decimal Amount { get; set; }
    }

    public interface IVisaPaymentService
    {
        Task Pay(PaymentDetails details);
    }
}
