namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChanges();
    }
}
