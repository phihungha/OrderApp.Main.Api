using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderApp.Main.Api.Application.Interfaces.ApplicationServices;
using OrderApp.Main.Api.Application.Services;
using OrderApp.Main.Api.Application.Validators;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderApp.Main.Api.Application
{
    public static class Setup
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<ProductInputValidator>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}
