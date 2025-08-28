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
using OrderApp.Main.Api.Infrastructure.Notify;
using OrderApp.Main.Api.Infrastructure.Notify.MessageDTOs;
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

            SetupDbContext(configuration, services);
            SetupOpenSearch(configuration, services);
            SetupVisaApiClient(configuration, services);
            SetupMessagePublishers(configuration, services);

            services.AddScoped<IOrderNotifyService, NotifyService>();
            services.AddScoped<IJobRequestService, JobRequestService>();
            services.AddSingleton<IProductSearchService, ProductSearchService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVisaPaymentService, VisaPaymentService>();
        }

        private static void SetupDbContext(
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
        }

        private static void SetupOpenSearch(
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
        }

        private static void SetupVisaApiClient(
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
        }

        private static void SetupMessagePublishers(
            IConfiguration configuration,
            IServiceCollection services
        )
        {
            var orderFulfillRequestsSqsUrl =
                configuration.GetValue<string>("AwsSqs:OrderFulfillRequests:Url")
                ?? throw new InvalidOperationException(
                    "AwsSqs:OrderFulfillRequests:Url is not configured."
                );

            var OrderEventsSqsUrl =
                configuration.GetValue<string>("AwsSns:OrderEvents:Url")
                ?? throw new InvalidOperationException("AwsSns:OrderEvents:Url is not configured.");

            services.AddAWSMessageBus(bus =>
            {
                bus.AddSQSPublisher<OrderFulfillRequestMessageDto>(
                    orderFulfillRequestsSqsUrl,
                    OrderFulfillRequestMessageDto.MessageType
                );

                bus.AddSQSPublisher<OrderEventMessageDto>(
                    OrderEventsSqsUrl,
                    OrderEventMessageDto.MessageType
                );
            });
        }
    }
}
