using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetAllAsync(string? nameContains = null);
        Task<Product?> GetDetailsbyId(int id);
        Task DeleteById(int id);
    }
}
