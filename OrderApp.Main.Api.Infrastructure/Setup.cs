using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderApp.Main.Api.Application.Interfaces;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;
using OrderApp.Main.Api.Infrastructure.JobRequest;
using OrderApp.Main.Api.Infrastructure.JobRequest.MessageDTOs;
using OrderApp.Main.Api.Infrastructure.Persistence;
using OrderApp.Main.Api.Infrastructure.VisaPayment;
using Refit;

namespace OrderApp.Main.Api.Infrastructure
{
    public static class Setup
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var config = builder.Configuration;

            SetupUnitOfWork(config, services);
            SetupVisaPaymentService(config, services);
            SetupSqsPublishers(config, services);

            builder.Services.AddScoped<IJobRequestService, JobRequestService>();
        }

        private static void SetupUnitOfWork(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var defaultConnectionString =
                configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:Default is not configured."
                );
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(defaultConnectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void SetupVisaPaymentService(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var visaApiHostUrl = configuration.GetValue<string>("VisaApi:HostUrl");
            if (string.IsNullOrEmpty(visaApiHostUrl))
            {
                throw new Exception("VisaApi:HostUrl configuration is missing or empty.");
            }
            services
                .AddRefitClient<IVisaApi>(
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

            services.AddScoped<IVisaPaymentService, VisaPaymentService>();
        }

        private static void SetupSqsPublishers(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var orderFulfillReqsSqsUrl =
                configuration.GetValue<string>("AwsSqs:OrderFulfillRequests:Url")
                ?? throw new InvalidOperationException(
                    "AwsSqs:OrderFulfillRequests:Url is not configured."
                );

            services.AddAWSMessageBus(bus =>
            {
                bus.AddSQSPublisher<OrderFulfillRequestMessageDto>(
                    orderFulfillReqsSqsUrl,
                    OrderFulfillRequestMessageDto.MessageType
                );
            });
        }
    }
}
