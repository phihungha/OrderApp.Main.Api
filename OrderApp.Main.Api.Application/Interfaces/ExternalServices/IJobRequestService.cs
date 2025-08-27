namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public interface IJobRequestService
    {
        Task FulfillOrder(int orderId);
    }
}
