namespace OrderApp.Main.Api.Domain.Entities.OrderEntities
{
    public enum OrderStatus
    {
        Pending,
        Fulfilling,
        WaitingForShipping,
        Shipping,
        Shipped,
        Completed,
        Canceled,
    }
}
