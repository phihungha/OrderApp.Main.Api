using FluentValidation;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;

namespace OrderApp.Main.Api.Application.Validators
{
    public class OrderLineCreateValidator : AbstractValidator<OrderLineCreateDto>
    {
        public OrderLineCreateValidator()
        {
            RuleFor(d => d.Quantity).GreaterThan(0);
        }
    }
}
