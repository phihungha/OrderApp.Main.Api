using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Entities.StockItemEntities;
using OrderApp.Main.Api.Domain.Entities.UserEntities;
using OrderApp.Main.Api.Infrastructure.Persistence.ValueConverters;

namespace OrderApp.Main.Api.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockItem> StockItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<OrderStatus>()
                .HaveConversion<OrderStatusToStringConverter>();
        }
    }
}
