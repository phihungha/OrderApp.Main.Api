using FluentValidation;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;

namespace OrderApp.Main.Api.Application.Validators
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(d => d.ShippingAddress).NotEmpty();
            RuleFor(d => d.Lines)
                .Must(l => l.Count > 0)
                .WithMessage("Order must have at least one line.");
        }
    }
}
