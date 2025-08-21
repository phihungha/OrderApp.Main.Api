using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Entities.StockItemEntities;

namespace OrderApp.Main.Api.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockItem> StockItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
