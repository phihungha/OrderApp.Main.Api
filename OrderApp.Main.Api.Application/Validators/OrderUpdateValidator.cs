using FluentValidation;
using OrderApp.Main.Api.Application.DTOs.OrderDTOs;

namespace OrderApp.Main.Api.Application.Validators
{
    public class OrderUpdateValidator : AbstractValidator<OrderUpdateDto>
    {
        public OrderUpdateValidator()
        {
            RuleFor(d => d.Status).IsInEnum();
        }
    }
}
