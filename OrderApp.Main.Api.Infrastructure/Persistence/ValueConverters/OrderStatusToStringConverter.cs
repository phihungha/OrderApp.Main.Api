using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Infrastructure.Persistence.ValueConverters
{
    public class OrderStatusToStringConverter : ValueConverter<OrderStatus, string>
    {
        public OrderStatusToStringConverter()
            : base(v => v.ToString(), v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)) { }
    }
}
