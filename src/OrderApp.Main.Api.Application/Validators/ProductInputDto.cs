using FluentValidation;
using OrderApp.Main.Api.Application.DTOs.ProductDTOs;

namespace OrderApp.Main.Api.Application.Validators
{
    public class ProductInputValidator : AbstractValidator<ProductInputDto>
    {
        public ProductInputValidator()
        {
            RuleFor(d => d.Name).NotEmpty();
            RuleFor(d => d.Price).GreaterThan(0);
            RuleFor(d => d.StockQuantity).GreaterThanOrEqualTo(0);
        }
    }
}
