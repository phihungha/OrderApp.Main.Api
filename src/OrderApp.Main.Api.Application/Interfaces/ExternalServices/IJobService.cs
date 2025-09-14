namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public interface IJobService
    {
        Task FulfillOrder(int orderId);
    }
}
