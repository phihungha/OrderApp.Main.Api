using OpenSearch.Client;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;

namespace OrderApp.Main.Api.Infrastructure
{
    public class ProductSearchService(IOpenSearchClient openSearchClient) : IProductSearchService
    {
        private const string IndexName = "products";
        private const int MaxSearchResultSize = 50;

        private readonly IOpenSearchClient openSearchClient = openSearchClient;

        public async Task<IReadOnlyList<ProductSearchIndexDoc>> Search(string? searchTerm = null)
        {
            var response = await openSearchClient.SearchAsync<ProductSearchIndexDoc>(i =>
                i.Index(IndexName)
                    .Query(q =>
                        q.MultiMatch(m =>
                            m.Fields(fs =>
                                    fs.Field(f => f.Name)
                                        .Field(f => f.ShortDescription)
                                        .Field(f => f.Description)
                                )
                                .Query(searchTerm)
                        )
                    )
                    .Size(MaxSearchResultSize)
            );

            return response.Documents.ToList();
        }

        public async Task IndexDocument(ProductSearchIndexDoc document)
        {
            await openSearchClient.IndexAsync(document, i => i.Index(IndexName).Id(document.Id));
        }

        public async Task DeleteDocument(int productId)
        {
            await openSearchClient.DeleteAsync<ProductSearchIndexDoc>(
                productId,
                i => i.Index(IndexName)
            );
        }
    }
}
