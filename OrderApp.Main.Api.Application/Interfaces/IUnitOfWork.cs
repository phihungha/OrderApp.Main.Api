namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }

        Task SaveChanges();
    }
}
