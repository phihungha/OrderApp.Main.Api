using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Infrastructure.Persistence.Repositories
{
    public class ProductRepository(AppDbContext dbContext)
        : Repository<Product>(dbContext),
            IProductRepository
    {
        public async Task<IReadOnlyList<Product>> GetAllAsync(string? nameContains = null)
        {
            var query = Entities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nameContains))
            {
                query = query.Where(e => e.Name.Contains(nameContains));
            }

            return await query.OrderBy(e => e.Id).ToListAsync();
        }

        public async Task<Product?> GetDetailsbyId(int id)
        {
            return await Entities.Include(e => e.StockItem).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task DeleteById(int id)
        {
            await Entities.Where(e => e.Id == id).ExecuteDeleteAsync();
        }
    }
}
