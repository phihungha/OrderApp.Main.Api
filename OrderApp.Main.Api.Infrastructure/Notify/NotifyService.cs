using AWS.Messaging.Publishers.SNS;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Infrastructure.Notify.MessageDTOs;

namespace OrderApp.Main.Api.Infrastructure.Notify
{
    public class NotifyService(ISNSPublisher snsPublisher) : IOrderNotifyService
    {
        private readonly ISNSPublisher snsPublisher = snsPublisher;

        public async Task NotifyStatusUpdate(int orderId, OrderStatus status, DateTime time)
        {
            await snsPublisher.PublishAsync(
                new OrderEventMessageDto
                {
                    OrderId = orderId,
                    Status = status,
                    Time = time,
                }
            );
        }
    }
}
