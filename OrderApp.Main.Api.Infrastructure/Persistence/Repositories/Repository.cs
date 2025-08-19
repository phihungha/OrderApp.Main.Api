using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Interfaces;

namespace OrderApp.Main.Api.Infrastructure.Persistence.Repositories
{
    public abstract class Repository<T>(AppDbContext dbContext) : IRepository<T>
        where T : class
    {
        private readonly AppDbContext dbContext = dbContext;

        private DbSet<T> Entities => dbContext.Set<T>();

        public void Add(T entity)
        {
            Entities.Add(entity);
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }
    }
}
