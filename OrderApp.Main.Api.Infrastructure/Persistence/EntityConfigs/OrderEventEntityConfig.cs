using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Infrastructure.Persistence.EntityConfigs
{
    internal class OrderEventEntityConfig : IEntityTypeConfiguration<OrderEvent>
    {
        public void Configure(EntityTypeBuilder<OrderEvent> builder)
        {
            builder.Property(e => e.Time).HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
