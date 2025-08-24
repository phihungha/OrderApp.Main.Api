using FluentResults;
using OrderApp.Main.Api.Domain.Entities.UserEntities;

namespace OrderApp.Main.Api.Application.Interfaces.ApplicationServiceInterfaces
{
    public interface IPaymentService
    {
        Task<Result> Pay(decimal amount, PaymentMethod method);
    }
}
