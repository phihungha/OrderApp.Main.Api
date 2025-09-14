using Refit;

namespace OrderApp.Main.Api.Infrastructure.VisaPayment
{
    public interface IVisaApi
    {
        [Post("/payments")]
        Task<VisaPayResDto> Pay(VisaPayReqDto dto);
    }

    public record VisaPayReqDto
    {
        public required string CardNumber { get; set; }
        public required string CardHolderName { get; set; }
        public required string CardExpiry { get; set; }
        public required string CardCvv { get; set; }
        public required decimal Amount { get; set; }
    }

    public record VisaPayResDto
    {
        public required string TransactionId { get; set; }
        public required string Status { get; set; }
        public required string Message { get; set; }
        public required string Amount { get; set; }
    }
}
