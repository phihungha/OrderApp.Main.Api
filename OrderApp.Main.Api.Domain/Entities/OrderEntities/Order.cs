using FluentResults;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.Domain.Entities.OrderEntities
{
    public class Order
    {
        public int Id { get; set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public bool IsFinished => Status == OrderStatus.Completed || Status == OrderStatus.Canceled;
        public IList<OrderEvent> Events { get; private set; } = [];
        public OrderEvent CurrentEvent => Events.Last();

        public required string ShippingAddress { get; set; }

        public ICollection<OrderLine> Lines { get; private set; } = [];
        public ICollection<Product> Products { get; } = [];

        public decimal TotalAmount
        {
            get => Lines.Sum(l => l.TotalPrice);
            set => _ = value;
        }

        public static Order CreateNew(string shippingAddress, IEnumerable<OrderLine> orderLines)
        {
            var order = new Order { ShippingAddress = shippingAddress };
            order.SetOrderLines(orderLines);
            order.RecordEvent();
            return order;
        }

        public Result SetOrderLines(IEnumerable<OrderLine> lines)
        {
            if (Status != OrderStatus.Pending)
            {
                return new BusinessError(
                    "Cannot set order lines of an order not in Pending state."
                );
            }

            Lines.Clear();
            foreach (var line in lines)
            {
                Lines.Add(line);
            }

            return Result.Ok();
        }

        public Result BeginFulfill()
        {
            if (Status != OrderStatus.Pending)
            {
                return new BusinessError("Cannot begin fulfilling a non-pending order.");
            }

            foreach (var line in Lines)
            {
                var product = line.Product;

                if (line.Quantity > product.StockItem.Quantity)
                {
                    return new BusinessError(
                        $"Product with ID {line.ProductId} doesn't have enough stock."
                    );
                }

                product.StockItem.Quantity -= line.Quantity;
            }

            Status = OrderStatus.Fulfilling;
            RecordEvent();

            return Result.Ok();
        }

        public Result FinishFulfill()
        {
            if (Status != OrderStatus.Fulfilling)
            {
                return new BusinessError(
                    "Cannot finish fulfilling an order that is not in Fulfilling state."
                );
            }

            Status = OrderStatus.WaitingForShipping;
            RecordEvent();

            return Result.Ok();
        }

        public Result BeginShipping()
        {
            if (Status != OrderStatus.WaitingForShipping)
            {
                return new BusinessError(
                    "Cannot begin shipping an order that is not in WaitingForShipping state."
                );
            }

            Status = OrderStatus.Shipping;
            RecordEvent();

            return Result.Ok();
        }

        public Result FinishShipping()
        {
            if (Status != OrderStatus.Shipping)
            {
                return new BusinessError(
                    "Cannot finish shipping an order that is not in Shipping state."
                );
            }

            Status = OrderStatus.Shipped;
            RecordEvent();

            return Result.Ok();
        }

        public Result Complete()
        {
            if (Status != OrderStatus.Shipped)
            {
                return new BusinessError("Cannot complete an order that is not in Shipped state.");
            }

            Status = OrderStatus.Completed;
            RecordEvent();

            return Result.Ok();
        }

        public Result Cancel()
        {
            if (IsFinished)
            {
                return new BusinessError(
                    "Cannot cancel a finished order. (in Completed or Canceled state)"
                );
            }

            Status = OrderStatus.Canceled;
            RecordEvent();

            return Result.Ok();
        }

        private void RecordEvent()
        {
            var eventEntity = new OrderEvent { OrderId = Id, Status = Status };
            Events.Add(eventEntity);
        }
    }
}
