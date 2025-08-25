using FluentResults;
using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Interfaces.Repositories;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.Infrastructure.Persistence.Repositories
{
    public class OrderRepository(AppDbContext dbContext)
        : Repository<Order>(dbContext),
            IOrderRepository
    {
        public async Task<IReadOnlyList<Order>> GetAllAsync(
            IEnumerable<OrderStatus>? statuses = null
        )
        {
            var query = Entities.AsNoTracking();

            if (statuses != null)
            {
                query = query.Where(e => statuses.Contains(e.Status));
            }

            return await query.OrderBy(e => e.Id).ToListAsync();
        }

        public async Task<Result<Order>> GetById(int id)
        {
            var entity = await Entities.FirstOrDefaultAsync(e => e.Id == id);
            return entity is null ? new NotFoundError() : entity;
        }

        public async Task<Result<Order>> GetDetailsById(int id)
        {
            var entity = await Entities
                .Include(e => e.Lines)
                .ThenInclude(l => l.Product)
                .Include(e => e.Events)
                .FirstOrDefaultAsync(e => e.Id == id);
            return entity is null ? new NotFoundError() : entity;
        }
    }
}
