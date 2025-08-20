using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Infrastructure.Persistence.Repositories;

namespace OrderApp.Main.Api.Infrastructure.Persistence
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        private readonly AppDbContext dbContext = dbContext;

        public IProductRepository Products { get; } = new ProductRepository(dbContext);

        public async Task SaveChanges()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
