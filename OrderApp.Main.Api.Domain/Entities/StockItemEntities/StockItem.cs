using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Domain.Entities.StockItemEntities
{
    public class StockItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
