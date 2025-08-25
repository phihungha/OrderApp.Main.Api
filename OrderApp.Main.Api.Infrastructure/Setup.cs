using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Infrastructure.Persistence;
using OrderApp.Main.Api.Infrastructure.Services.VisaPaymentService;
using OrderApp.Main.Api.Infrastructure.SqsMessageHandler;
using Refit;

namespace OrderApp.Main.Api.Infrastructure
{
    public static class Setup
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;

            var jobsConnectionString =
                builder.Configuration.GetConnectionString("Jobs")
                ?? throw new InvalidOperationException("ConnectionStrings:Jobs is not configured.");
            services.AddHangfire(provider =>
                provider.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(jobsConnectionString))
            );
            services.AddHangfireServer();

            var defaultConnectionString =
                builder.Configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:Default is not configured."
                );
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(defaultConnectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var visaApiHostUrl = builder.Configuration.GetValue<string>("VisaApi:HostUrl");
            if (string.IsNullOrEmpty(visaApiHostUrl))
            {
                throw new Exception("VisaApi:HostUrl configuration is missing or empty.");
            }
            builder
                .Services.AddRefitClient<IVisaApi>(
                    new RefitSettings
                    {
                        ContentSerializer = new NewtonsoftJsonContentSerializer(
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            }
                        ),
                    }
                )
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(visaApiHostUrl));

            builder.Services.AddScoped<IVisaPaymentService, VisaPaymentService>();

            var awsSqsUrls =
                config.GetSection("AwsSqs:Urls").Get<IDictionary<string, string>>()
                ?? throw new InvalidOperationException("AwsSqs:Urls is not configured.");

            var orderUpdatesSqsUrl =
                awsSqsUrls["OrderUpdates"]
                ?? throw new InvalidOperationException(
                    "AwsSqs:Urls:OrderUpdates is not configured."
                );
            services.AddAWSMessageBus(builder =>
            {
                builder.AddSQSPoller(
                    orderUpdatesSqsUrl,
                    options =>
                    {
                        options.MaxNumberOfConcurrentMessages = 10;
                        options.WaitTimeSeconds = 20;
                    }
                );

                builder.AddMessageHandler<SqsOrderUpdateHandler, OrderStatusUpdateMessage>(
                    OrderStatusUpdateMessage.MessageType
                );
            });
        }
    }
}
