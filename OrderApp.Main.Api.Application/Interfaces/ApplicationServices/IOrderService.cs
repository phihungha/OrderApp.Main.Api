using FluentResults;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.Interfaces.ApplicationServices
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderListItemDto>> GetAll(IEnumerable<OrderStatus>? statuses = null);
        Task<Result<OrderDetailsDto>> GetDetailsById(int id);
        Task<Result<OrderDetailsDto>> Create(OrderCreateDto dto);
        Task<Result<OrderDetailsDto>> Update(int id, OrderUpdateDto dto);
        Task<Result> Fulfill(int id);
        Task<Result> BeginShipping(int id);
        Task<Result> FinishShipping(int id);
        Task<Result> Complete(int id);
    }
}
