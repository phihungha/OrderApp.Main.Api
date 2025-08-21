using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Infrastructure.Persistence.EntityConfigs
{
    public class OrderEntityConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasMany(o => o.Products).WithMany(p => p.Orders).UsingEntity<OrderLine>();
        }
    }
}
