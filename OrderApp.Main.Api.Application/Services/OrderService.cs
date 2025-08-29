using FluentResults;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Domain.Entities.OrderEntities;
using OrderApp.Main.Api.Domain.Entities.UserEntities;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.Application.Services
{
    public class OrderService(
        IUnitOfWork unitOfWork,
        IJobStartService jobStartService,
        IOrderNotifier orderNotifier,
        IPaymentService paymentService
    ) : IOrderService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IJobStartService jobStartService = jobStartService;
        private readonly IOrderNotifier orderNotifier = orderNotifier;
        private readonly IPaymentService paymentService = paymentService;

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

        public async Task<Result> Complete(int id)
        {
            var getResult = await unitOfWork.Orders.GetById(id);
            if (getResult.IsFailed)
            {
                return Result.Fail(getResult.Errors);
            }

            var order = getResult.Value;

            var paymentMethod = new PaymentMethod
            {
                CardNumber = "4111111111111111",
                CardHolderName = "HA PHI HUNG",
                CardExpiry = "12/27",
                CardCvv = "123",
            };
            var paymentResult = await paymentService.Pay(order.TotalAmount, paymentMethod);

            if (paymentResult.IsFailed)
            {
                return paymentResult;
            }

            var completeResult = order.Complete();
            if (completeResult.IsFailed)
            {
                return completeResult;
            }

            await unitOfWork.SaveChanges();
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return Result.Ok();
        }

        private async Task<Result> UpdateStatus(int id, Func<Order, Result> updateStatusFunc)
        {
            var getResult = await unitOfWork.Orders.GetById(id);
            if (getResult.IsFailed)
            {
                return Result.Fail(getResult.Errors);
            }

            var order = getResult.Value;

            var statusUpdateResult = updateStatusFunc(order);
            if (statusUpdateResult.IsFailed)
            {
                return statusUpdateResult;
            }

            await unitOfWork.SaveChanges();
            await orderNotifier.NotifyEvent(order.CurrentEvent);

            return Result.Ok();
        }
    }
}
