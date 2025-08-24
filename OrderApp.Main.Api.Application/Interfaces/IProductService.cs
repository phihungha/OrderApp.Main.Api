using FluentResults;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;

namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductCatalogItemDto>> GetAdminCatalog(string? nameContains = null);
        Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalog(string? searchQuery = null);
        Task<Result<ProductDetailsDto>> GetDetailsById(int id);
        Task<Result<ProductDetailsDto>> Create(ProductInputDto productInputDto);
        Task<Result<ProductDetailsDto>> Update(int id, ProductInputDto productInputDto);
        Task<Result> Delete(int id);
    }
}
