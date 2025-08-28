using AWS.Messaging.Publishers.SNS;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Infrastructure.Notify.MessageDTOs;

namespace OrderApp.Main.Api.Infrastructure.Notify
{
    public class OrderNotifyService(ISNSPublisher snsPublisher) : IOrderNotifyService
    {
        private readonly ISNSPublisher snsPublisher = snsPublisher;

        public async Task NotifyEvent(OrderEvent orderEvent)
        {
            await snsPublisher.PublishAsync(
                new OrderEventMessageDto
                {
                    OrderId = orderEvent.OrderId,
                    Status = orderEvent.Status,
                    Time = orderEvent.Time,
                },
                new SNSOptions { MessageGroupId = orderEvent.Id.ToString() }
            );
        }
    }
}
