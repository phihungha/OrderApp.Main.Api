using OrderApp.Main.Api.Domain.Entities.UserEntities;

namespace OrderApp.Main.Api.Application.Interfaces.ApplicationServiceInterfaces
{
    public interface IPaymentService
    {
        Task Pay(decimal amount, PaymentMethod method);
    }
}
