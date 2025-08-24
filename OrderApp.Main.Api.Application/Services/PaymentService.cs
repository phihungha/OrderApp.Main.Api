using FluentResults;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServiceInterfaces;
using OrderApp.Main.Api.Application.Interfaces.InfrastructureServices;
using OrderApp.Main.Api.Domain.Entities.UserEntities;

namespace OrderApp.Main.Api.Application.Services
{
    public class PaymentService(IVisaPaymentService visaPaymentService) : IPaymentService
    {
        private readonly IVisaPaymentService visaPaymentService = visaPaymentService;

        public async Task<Result> Pay(decimal amount, PaymentMethod method)
        {
            var paymentDetails = new PaymentDetails
            {
                CardNumber = method.CardNumber,
                CardHolderName = method.CardHolderName,
                CardExpiry = method.CardExpiry,
                CardCvv = method.CardCvv,
                Amount = amount,
            };
            return await visaPaymentService.Pay(paymentDetails);
        }
    }
}
