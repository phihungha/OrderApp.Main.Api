using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderApp.Main.Api.Application
{
    public static class Setup
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddFluentValidationAutoValidation();

            services.AddScoped<IProductService, ProductService>();
        }
    }
}
