using FluentResults;
using OrderApp.Main.Api.Application.Interfaces.Repositories;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }

        Task<Result> SaveChanges();
    }
}
