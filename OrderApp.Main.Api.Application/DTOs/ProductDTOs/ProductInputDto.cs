namespace OrderApp.Main.Api.Application.DTOs.ProductDTOs
{
    public class ProductInputDto
    {
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
    }
}
