namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public record ProductSearchIndexDoc
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
    }

    public interface IProductSearchService
    {
        Task<IReadOnlyList<ProductSearchIndexDoc>> Search(string? query = null);
        Task IndexDocument(ProductSearchIndexDoc document);
        Task DeleteDocument(int productId);
    }
}
