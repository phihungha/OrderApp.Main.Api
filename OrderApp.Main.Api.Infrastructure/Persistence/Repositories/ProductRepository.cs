using FluentResults;
using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Interfaces.Repositories;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Errors;

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

        public async Task<IReadOnlyList<Product>> GetManyByIdsAsync(IEnumerable<int> ids)
        {
            return await Entities
                .AsNoTracking()
                .Include(e => e.StockItem)
                .Where(e => ids.Contains(e.Id))
                .ToListAsync();
        }

        public async Task<Result<Product>> GetDetailsbyId(int id)
        {
            var entity = await Entities
                .Include(e => e.StockItem)
                .FirstOrDefaultAsync(e => e.Id == id);
            return entity is null ? new NotFoundError() : entity;
        }

        public async Task<Result> DeleteById(int id)
        {
            var rowsDeleted = await Entities.Where(e => e.Id == id).ExecuteDeleteAsync();
            return Result.FailIf(rowsDeleted == 0, new NotFoundError());
        }
    }
}
