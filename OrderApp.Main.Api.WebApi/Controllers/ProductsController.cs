using Microsoft.AspNetCore.Mvc;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;
using OrderApp.Main.Api.Application.Interfaces;

namespace OrderApp.Main.Api.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService productService = productService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductCatalogItemDto>>> GetAll(
            bool? admin = false,
            string? searchQuery = null
        )
        {
            if (admin == true)
            {
                var adminDto = await productService.GetAdminCatalog(searchQuery);
                return Ok(adminDto);
            }

            var dto = await productService.GetCatalog(searchQuery);
            return Ok(dto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> GetById(int id)
        {
            var dto = await productService.GetDetailsById(id);

            if (dto == null)
            {
                return NotFound();
            }

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailsDto>> Create(ProductInputDto inputDto)
        {
            var createdProduct = await productService.Create(inputDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> Update(int id, ProductInputDto inputDto)
        {
            var updatedProduct = await productService.Update(id, inputDto);
            return Ok(updatedProduct);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await productService.Delete(id);
            return NoContent();
        }
    }
}
