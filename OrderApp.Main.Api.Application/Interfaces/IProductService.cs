using OrderApp.Main.Api.Application.DTOs.ProductDTOs;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductCatalogItemDto>> GetAdminCatalog(string? nameContains = null);
        Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalog(string? searchQuery = null);
        Task<ProductDetailsDto?> GetDetailsById(int id);
        Task<ProductDetailsDto> Create(ProductInputDto productInputDto);
        Task<ProductDetailsDto> Update(int id, ProductInputDto productInputDto);
        Task Delete(int id);
    }
}
