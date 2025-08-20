using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Application.DTOs.ProductDTOs
{
    public class ProductDetailsDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required int StockQuantity { get; set; }

        public static ProductDetailsDto FromEntity(Product product)
        {
            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockItem.Quantity,
            };
        }
    }
}
