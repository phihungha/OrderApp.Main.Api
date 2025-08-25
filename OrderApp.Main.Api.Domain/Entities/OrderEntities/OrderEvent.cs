namespace OrderApp.Main.Api.Domain.Entities.OrderEntities
{
    public class OrderEvent
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
