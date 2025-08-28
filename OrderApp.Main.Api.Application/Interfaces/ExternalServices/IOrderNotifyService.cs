using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public interface IOrderNotifyService
    {
        Task NotifyStatusUpdate(int orderId, OrderStatus status, DateTime time);
    }
}
