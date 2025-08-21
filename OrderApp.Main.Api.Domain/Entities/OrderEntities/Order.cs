using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Exceptions;

namespace OrderApp.Main.Api.Domain.Entities.OrderEntities
{
    public class Order
    {
        public int Id { get; set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public bool IsFinished => Status == OrderStatus.Completed || Status == OrderStatus.Canceled;
        public IList<OrderEvent> Events { get; private set; } = [];

        public required string ShippingAddress { get; set; }

        public ICollection<OrderLine> Lines { get; private set; } = [];
        public ICollection<Product> Products { get; } = [];

        public decimal TotalAmount
        {
            get => Lines.Sum(l => l.TotalPrice);
            set => _ = value;
        }

        public void SetOrderLines(IEnumerable<OrderLine> lines)
        {
            if (Status != OrderStatus.Pending)
            {
                throw new BusinessException(
                    "Cannot change order lines of an order that isn't pending."
                );
            }

            Lines.Clear();
            foreach (var line in lines)
            {
                Lines.Add(line);
            }
        }

        public void BeginFulfill()
        {
            if (IsFinished)
            {
                throw new BusinessException(
                    "Cannot begin fulfilling a finished (completed or canceled) order."
                );
            }
            Status = OrderStatus.Fulfilling;
        }

        public void FinishFulfill()
        {
            if (Status != OrderStatus.Fulfilling)
            {
                throw new BusinessException(
                    "Cannot finish fulfilling an order that is not in fulfillment."
                );
            }
            Status = OrderStatus.WaitingForShipping;
        }

        public void BeginShipping()
        {
            if (Status != OrderStatus.Fulfilling)
            {
                throw new BusinessException(
                    "Cannot begin shipping an order that is not fulfilled."
                );
            }
            Status = OrderStatus.Shipping;
        }

        public void FinishShipping()
        {
            if (Status != OrderStatus.Shipping)
            {
                throw new BusinessException(
                    "Cannot finish shipping an order that is not in shipping."
                );
            }
            Status = OrderStatus.Shipped;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Shipped)
            {
                throw new BusinessException(
                    "Cannot complete a finished (completed or canceled) order."
                );
            }
            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (IsFinished)
            {
                throw new BusinessException(
                    "Cannot cancel a finished (completed or canceled) order."
                );
            }
            Status = OrderStatus.Canceled;
        }
    }
}
