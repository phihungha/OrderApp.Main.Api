using OrderApp.Main.Api.Domain.Entities.ProductEntities;

namespace OrderApp.Main.Api.Application.DTOs.ProductDTOs
{
    public class ProductCatalogItemDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public required decimal Price { get; set; }

        public static ProductCatalogItemDto FromEntity(Product product)
        {
            return new ProductCatalogItemDto
            {
                Id = product.Id,
                Name = product.Name,
                ShortDescription = product.Description,
                Price = product.Price,
            };
        }
    }
}
