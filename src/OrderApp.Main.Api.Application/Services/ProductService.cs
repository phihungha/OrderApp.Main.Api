using FluentResults;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Entities.StockItemEntities;

namespace OrderApp.Main.Api.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IProductSearchService productSearchService)
        : IProductService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IProductSearchService productSearchService = productSearchService;

        public async Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalog(
            string? searchQuery = null
        )
        {
            var productIndexItems = await productSearchService.Search(searchQuery);

            return productIndexItems
                .Select(i => new ProductCatalogItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    ShortDescription = i.ShortDescription,
                    Price = i.Price,
                })
                .ToList();
        }

        public async Task<IReadOnlyList<ProductCatalogItemDto>> GetAdminCatalog(
            string? nameContains = null
        )
        {
            var products = await unitOfWork.Products.GetAllAsync(nameContains);
            return products.Select(ProductCatalogItemDto.FromEntity).ToList();
        }

        public async Task<Result<ProductDetailsDto>> GetDetailsById(int id)
        {
            var result = await unitOfWork.Products.GetDetailsbyId(id);

            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            return ProductDetailsDto.FromEntity(result.Value);
        }

        public async Task<Result<ProductDetailsDto>> Create(ProductInputDto inputDto)
        {
            var product = new Product
            {
                Name = inputDto.Name,
                ShortDescription = inputDto.ShortDescription,
                Description = inputDto.Description,
                Price = inputDto.Price,
                StockItem = new StockItem { Quantity = inputDto.StockQuantity },
            };

            unitOfWork.Products.Add(product);
            await unitOfWork.SaveChanges();

            var productSearchIndexDoc = new ProductSearchIndexDoc
            {
                Id = product.Id,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
            };
            await productSearchService.IndexDocument(productSearchIndexDoc);

            return ProductDetailsDto.FromEntity(product);
        }

        public async Task<Result<ProductDetailsDto>> Update(int id, ProductInputDto inputDto)
        {
            var result = await unitOfWork.Products.GetDetailsbyId(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var product = result.Value;

            product.Name = inputDto.Name;
            product.ShortDescription = inputDto.ShortDescription;
            product.Description = inputDto.Description;
            product.Price = inputDto.Price;
            product.StockItem.Quantity = inputDto.StockQuantity;

            await unitOfWork.SaveChanges();

            var productSearchIndexDoc = new ProductSearchIndexDoc
            {
                Id = product.Id,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
            };
            await productSearchService.IndexDocument(productSearchIndexDoc);

            return await GetDetailsById(id);
        }

        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Products.DeleteById(id);

            if (result.IsSuccess)
            {
                await productSearchService.DeleteDocument(id);
            }

            return result;
        }
    }
}
