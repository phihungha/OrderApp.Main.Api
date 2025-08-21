using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService orderService = orderService;

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductCatalogItemDto>>> GetAll(
            [FromQuery] IEnumerable<OrderStatus>? statuses = null
        )
        {
            var dto = await orderService.GetAll(statuses);
            return Ok(dto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> GetById(int id)
        {
            return await orderService.GetDetailsById(id).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailsDto>> Create(OrderCreateDto inputDto)
        {
            var result = await orderService.Create(inputDto);

            if (result.IsFailed)
            {
                return result.ToActionResult();
            }

            var dto = result.Value;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<ProductDetailsDto>> Update(int id, OrderUpdateDto inputDto)
        {
            return await orderService.Update(id, inputDto).ToActionResult();
        }
    }
}
