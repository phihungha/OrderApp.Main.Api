using FluentResults;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;
using OrderApp.Main.Api.Application.Errors;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Exceptions;

namespace OrderApp.Main.Api.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork) : IOrderService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<OrderListItemDto>> GetAll(
            IEnumerable<OrderStatus>? statuses = null
        )
        {
            var orders = await unitOfWork.Orders.GetAllAsync(statuses);
            return orders.Select(OrderListItemDto.FromEntity).ToList();
        }

        public async Task<Result<OrderDetailsDto>> GetDetailsById(int id)
        {
            var result = await unitOfWork.Orders.GetDetailsById(id);

            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            return OrderDetailsDto.FromEntity(result.Value);
        }

        public async Task<Result<OrderDetailsDto>> Create(OrderCreateDto dto)
        {
            var order = new Order() { ShippingAddress = dto.ShippingAddress };

            var productIds = dto.Lines.Select(line => line.ProductId).ToList();
            var products = await unitOfWork.Products.GetManyByIdsAsync(productIds);
            var productsById = products.ToDictionary(p => p.Id, p => p);

            List<OrderLine> orderLines = [];
            foreach (var line in dto.Lines)
            {
                if (!productsById.ContainsKey(line.ProductId))
                {
                    return new BusinessError($"Product with ID {line.ProductId} not found.");
                }

                var product = productsById[line.ProductId];
                orderLines.Add(
                    new OrderLine
                    {
                        ProductId = product.Id,
                        Quantity = line.Quantity,
                        UnitPrice = product.Price,
                    }
                );
            }

            order.SetOrderLines(orderLines);
            order.BeginFulfill();

            unitOfWork.Orders.Add(order);
            await unitOfWork.SaveChanges();

            return (await GetDetailsById(order.Id)).Value;
        }

        public async Task<Result<OrderDetailsDto>> Update(int id, OrderUpdateDto dto)
        {
            var result = await unitOfWork.Orders.GetDetailsById(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var order = result.Value;

            if (dto.Status == OrderUpdateDto.StatusOptions.Completed)
            {
                order.Complete();
            }
            else if (dto.Status == OrderUpdateDto.StatusOptions.Canceled)
            {
                order.Cancel();
            }

            await unitOfWork.SaveChanges();
            return OrderDetailsDto.FromEntity(order);
        }

        public async Task<Result> FinishFulfill(int id) =>
            await UpdateStatus(id, order => order.FinishFulfill());

        public async Task<Result> BeginShipping(int id) =>
            await UpdateStatus(id, order => order.BeginShipping());

        public async Task<Result> FinishShipping(int id) =>
            await UpdateStatus(id, order => order.FinishShipping());

        public async Task<Result> Complete(int id) =>
            await UpdateStatus(id, order => order.Complete());

        private async Task<Result> UpdateStatus(int id, Action<Order> statusUpdateAction)
        {
            var result = await unitOfWork.Orders.GetById(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            statusUpdateAction(result.Value);
            return await unitOfWork.SaveChanges();
        }
    }
}
