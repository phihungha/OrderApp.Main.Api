using OrderApp.Main.Api.Application.Interfaces;

namespace OrderApp.Main.Api.Infrastructure.Persistence
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        private readonly AppDbContext dbContext = dbContext;

        public async Task SaveChanges()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
