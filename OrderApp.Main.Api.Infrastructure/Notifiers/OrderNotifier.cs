using AWS.Messaging.Publishers.SNS;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Infrastructure.Notifiers.MessageDTOs;

namespace OrderApp.Main.Api.Infrastructure.Notifiers
{
    public class OrderNotifier(IEmailService emailService, ISNSPublisher snsPublisher)
        : IOrderNotifier
    {
        private static readonly HashSet<OrderStatus> OrderStatusesWithEmailNotice =
        [
            OrderStatus.WaitingForShipping,
            OrderStatus.Shipping,
            OrderStatus.Shipped,
            OrderStatus.Completed,
            OrderStatus.Canceled,
        ];

        private readonly IEmailService emailService = emailService;
        private readonly ISNSPublisher snsPublisher = snsPublisher;

        public async Task NotifyEvent(OrderEvent orderEvent)
        {
            await NotifyEventViaSns(orderEvent);

            if (OrderStatusesWithEmailNotice.Contains(orderEvent.Status))
            {
                await NotifyEventViaEmail(orderEvent);
            }
        }

        private async Task NotifyEventViaSns(OrderEvent orderEvent)
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

        private async Task NotifyEventViaEmail(OrderEvent orderEvent)
        {
            var orderId = orderEvent.OrderId;
            var status = orderEvent.Status;
            var time = orderEvent.Time;

            var subjectStatusDescription = status switch
            {
                OrderStatus.WaitingForShipping => "is Waiting for Shipping",
                OrderStatus.Shipping => "is being Shipped",
                OrderStatus.Shipped => "has been Shipped",
                OrderStatus.Completed => "is Completed",
                OrderStatus.Canceled => "has been Canceled",
                _ => "has been updated",
            };
            var emailSubject = $"Order #{orderId} {subjectStatusDescription}";

            await emailService.SendMail(
                "Ha Phi Hung",
                "haphihung55@gmail.com",
                emailSubject,
                "OrderEventNoticeToCustomer",
                new
                {
                    OrderId = orderId,
                    Status = status.ToString(),
                    Time = time,
                }
            );
        }
    }
}
