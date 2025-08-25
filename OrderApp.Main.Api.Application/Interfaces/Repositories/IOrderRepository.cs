using FluentResults;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;

namespace OrderApp.Main.Api.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetAllAsync(IEnumerable<OrderStatus>? statuses = null);
        Task<Result<Order>> GetById(int id);
        Task<Result<Order>> GetDetailsById(int id);
    }
}
