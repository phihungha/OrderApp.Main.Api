using FluentResults;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetAllAsync(string? nameContains = null);
        Task<Result<Product>> GetDetailsbyId(int id);
        Task<Result> DeleteById(int id);
    }
}
