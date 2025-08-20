using FluentResults;
using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Errors;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Infrastructure.Persistence.Repositories;

namespace OrderApp.Main.Api.Infrastructure.Persistence
{
    public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        private readonly AppDbContext dbContext = dbContext;

        public IProductRepository Products { get; } = new ProductRepository(dbContext);

        public async Task<Result> SaveChanges()
        {
            try
            {
                await dbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new NotFoundError();
            }
        }
    }
}
