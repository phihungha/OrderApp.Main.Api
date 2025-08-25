using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Entities.StockItemEntities;

namespace OrderApp.Main.Api.Domain.Entities.ProductEntities
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public StockItem StockItem { get; set; } = null!;

        public ICollection<Order> Orders { get; } = [];
    }
}
