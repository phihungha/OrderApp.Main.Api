using Microsoft.Extensions.Hosting;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderApp.Main.Api.Application
{
    public static class Setup
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            services.AddFluentValidationAutoValidation();
        }
    }
}
