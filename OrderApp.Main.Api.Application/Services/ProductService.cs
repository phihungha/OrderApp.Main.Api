using FluentResults;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Domain.Entities.ProductEntities;
using OrderApp.Main.Api.Domain.Entities.StockItemEntities;

namespace OrderApp.Main.Api.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork) : IProductService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<ProductCatalogItemDto>> GetCatalog(
            string? searchQuery = null
        )
        {
            return await Task.FromResult(new List<ProductCatalogItemDto>());
        }

        public async Task<IReadOnlyList<ProductCatalogItemDto>> GetAdminCatalog(
            string? nameContains = null
        )
        {
            var entities = await unitOfWork.Products.GetAllAsync(nameContains);
            return entities.Select(ProductCatalogItemDto.FromEntity).ToList();
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
            var newEntity = new Product
            {
                Name = inputDto.Name,
                ShortDescription = inputDto.ShortDescription,
                Description = inputDto.Description,
                Price = inputDto.Price,
                StockItem = new StockItem { Quantity = 0 },
            };

            unitOfWork.Products.Add(newEntity);
            await unitOfWork.SaveChanges();

            return ProductDetailsDto.FromEntity(newEntity);
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

            return await GetDetailsById(id);
        }

        public async Task<Result> Delete(int id)
        {
            return await unitOfWork.Products.DeleteById(id);
        }
    }
}
