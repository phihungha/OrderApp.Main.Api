using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderApp.Main.Api.Infrastructure.Persistence;

namespace OrderApp.Main.Api.Infrastructure
{
    public static class Setup
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            var defaultConnectionString = builder.Configuration.GetConnectionString("Default");
            if (defaultConnectionString == null)
            {
                throw new InvalidOperationException("Default connection string is not configured.");
            }
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
            });
        }
    }
}
