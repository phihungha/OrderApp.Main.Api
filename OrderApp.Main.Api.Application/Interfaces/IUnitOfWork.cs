using FluentResults;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }

        Task<Result> SaveChanges();
    }
}
