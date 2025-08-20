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

        public async Task<ProductDetailsDto?> GetDetailsById(int id)
        {
            var entity = await unitOfWork.Products.GetDetailsbyId(id);
            return entity is null ? null : ProductDetailsDto.FromEntity(entity);
        }

        public async Task<ProductDetailsDto> Create(ProductInputDto productInputDto)
        {
            var newEntity = new Product
            {
                Name = productInputDto.Name,
                ShortDescription = productInputDto.ShortDescription,
                Description = productInputDto.Description,
                Price = productInputDto.Price,
                StockItem = new StockItem { Quantity = 0 },
            };

            unitOfWork.Products.Add(newEntity);
            await unitOfWork.SaveChanges();

            return ProductDetailsDto.FromEntity(newEntity);
        }

        public async Task<ProductDetailsDto> Update(int id, ProductInputDto productInputDto)
        {
            var updatedEntity = new Product
            {
                Id = id,
                Name = productInputDto.Name,
                ShortDescription = productInputDto.ShortDescription,
                Description = productInputDto.Description,
                Price = productInputDto.Price,
            };

            unitOfWork.Products.Update(updatedEntity);
            await unitOfWork.SaveChanges();

            return (await GetDetailsById(id))!;
        }

        public async Task Delete(int id)
        {
            await unitOfWork.Products.DeleteById(id);
        }
    }
}
