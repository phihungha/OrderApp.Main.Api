using FluentResults;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.Application.Services
{
    public class OrderService(
        IUnitOfWork unitOfWork,
        IJobStartService jobStartService,
        IOrderNotifier orderNotifier
    ) : IOrderService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IJobStartService jobStartService = jobStartService;
        private readonly IOrderNotifier orderNotifier = orderNotifier;

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
            var productIds = dto.Lines.Select(line => line.ProductId).ToList();
            var products = await unitOfWork.Products.GetManyByIdsAsync(productIds);
            var productsByIdDict = products.ToDictionary(p => p.Id, p => p);

            List<OrderLine> orderLines = [];
            foreach (var line in dto.Lines)
            {
                if (!productsByIdDict.ContainsKey(line.ProductId))
                {
                    return new BusinessError($"Product with ID {line.ProductId} not found.");
                }

                var product = productsByIdDict[line.ProductId];

                if (line.Quantity > product.StockItem.Quantity)
                {
                    return new BusinessError(
                        $"Product with ID {line.ProductId} doesn't have enough stock."
                    );
                }

                orderLines.Add(
                    new OrderLine
                    {
                        ProductId = product.Id,
                        Quantity = line.Quantity,
                        UnitPrice = product.Price,
                    }
                );
            }

            var order = Order.CreateNew(dto.ShippingAddress, orderLines);

            unitOfWork.Orders.Add(order);
            await unitOfWork.SaveChanges();

            await jobStartService.FulfillOrder(order.Id);
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return (await GetDetailsById(order.Id)).Value;
        }

        public async Task<Result> Fulfill(int id)
        {
            var result = await unitOfWork.Orders.GetDetailsById(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var order = result.Value;

            order.BeginFulfill();
            await unitOfWork.SaveChanges();
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            order.FinishFulfill();
            await unitOfWork.SaveChanges();
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return Result.Ok();
        }

        public async Task<Result<OrderDetailsDto>> Update(int id, OrderUpdateDto dto)
        {
            var result = await unitOfWork.Orders.GetDetailsById(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var order = result.Value;

            Result statusUpdateResult = Result.Ok();
            if (dto.Status == OrderUpdateDto.StatusOptions.Completed)
            {
                statusUpdateResult = order.Complete();
            }
            else if (dto.Status == OrderUpdateDto.StatusOptions.Canceled)
            {
                statusUpdateResult = order.Cancel();
            }

            if (statusUpdateResult.IsFailed)
            {
                return Result.Fail(statusUpdateResult.Errors);
            }

            await unitOfWork.SaveChanges();
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return OrderDetailsDto.FromEntity(order);
        }

        public async Task<Result> BeginShipping(int id) =>
            await UpdateStatus(id, order => order.BeginShipping());

        public async Task<Result> FinishShipping(int id) =>
            await UpdateStatus(id, order => order.FinishShipping());

        public async Task<Result> Complete(int id) =>
            await UpdateStatus(id, order => order.Complete());

        private async Task<Result> UpdateStatus(int id, Func<Order, Result> updateStatusFunc)
        {
            var result = await unitOfWork.Orders.GetById(id);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            var order = result.Value;

            updateStatusFunc(order);
            await unitOfWork.SaveChanges();

            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return Result.Ok();
        }
    }
}
