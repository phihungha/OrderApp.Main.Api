namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public interface IJobStartService
    {
        Task FulfillOrder(int orderId);
    }
}
