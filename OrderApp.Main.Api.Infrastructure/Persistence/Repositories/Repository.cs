using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Application.Interfaces.Repositories;

namespace OrderApp.Main.Api.Infrastructure.Persistence.Repositories
{
    public abstract class Repository<T>(AppDbContext dbContext) : IRepository<T>
        where T : class
    {
        private readonly AppDbContext dbContext = dbContext;

        protected DbSet<T> Entities => dbContext.Set<T>();

        public void Add(T entity)
        {
            Entities.Add(entity);
        }

        public void Update(T entity)
        {
            Entities.Update(entity);
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }
    }
}
