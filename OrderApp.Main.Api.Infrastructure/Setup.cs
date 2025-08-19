using Hangfire;
using Hangfire.PostgreSql;
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
            var services = builder.Services;
            var config = builder.Configuration;

            var defaultConnectionString = builder.Configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(defaultConnectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:Default is not configured.");
            }
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(defaultConnectionString);
            });

            var jobsConnectionString = builder.Configuration.GetConnectionString("Jobs");
            if (string.IsNullOrEmpty(jobsConnectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:Jobs is not configured.");
            }
            services.AddHangfire(provider =>
                provider.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(jobsConnectionString))
            );
            services.AddHangfireServer();
        }
    }
}
