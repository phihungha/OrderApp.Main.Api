using FluentResults;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.Infrastructure.VisaPayment
{
    public class VisaPaymentService(IVisaApi visaApiClient) : IVisaPaymentService
    {
        private readonly IVisaApi visaApiClient = visaApiClient;

        public async Task<Result> Pay(PaymentDetails details)
        {
            var reqDto = new VisaPayReqDto
            {
                CardNumber = details.CardNumber,
                CardHolderName = details.CardHolderName,
                CardExpiry = details.CardExpiry,
                CardCvv = details.CardCvv,
                Amount = details.Amount,
            };

            var respDto = await visaApiClient.Pay(reqDto);

            if (respDto.Status == "success")
            {
                return Result.Ok();
            }

            return new BusinessError(respDto.Message);
        }
    }
}
