using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenSearch.Client;
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
            var configuration = builder.Configuration;

            SetupUnitOfWork(configuration, services);
            SetupProductSearchService(configuration, services);
            SetupVisaPaymentService(configuration, services);
            SetupSqsPublishers(configuration, services);

            builder.Services.AddScoped<IJobRequestService, JobRequestService>();
        }

        private static void SetupUnitOfWork(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var connectionString =
                configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:Default is not configured."
                );

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void SetupProductSearchService(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var hostUrl =
                configuration.GetValue<string>("OpenSearchApiClient:Url")
                ?? throw new InvalidOperationException(
                    "OpenSearchClient:Url configuration is missing or empty."
                );
            var connetionSettings = new ConnectionSettings(new Uri(hostUrl));

            services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connetionSettings));
            services.AddSingleton<IProductSearchService, ProductSearchService>();
        }

        private static void SetupVisaPaymentService(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var hostUrl = configuration.GetValue<string>("VisaApiClient:Url");
            if (string.IsNullOrEmpty(hostUrl))
            {
                throw new Exception("VisaPaymentClient:Url configuration is missing or empty.");
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
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(hostUrl));

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
