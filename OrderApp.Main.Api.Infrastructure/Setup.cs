using System.Configuration;
using FluentEmail.MailKitSmtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            var infraConfig =
                builder.Configuration.Get<InfrastructureConfig>()
                ?? throw new ConfigurationErrorsException("App config is null.");

            SetupDefaultDb(builder.Configuration, services);
            SetupFluentEmail(infraConfig, services);
            SetupMessagePublishers(infraConfig, services);
            SetupOpenSearch(infraConfig, services);
            SetupVisaApiClient(infraConfig, services);

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJobRequestService, JobRequestService>();
            services.AddScoped<IOrderNotifyService, OrderNotifyService>();
            services.AddSingleton<IProductSearchService, ProductSearchService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVisaPaymentService, VisaPaymentService>();
        }

        private static void SetupFluentEmail(
            InfrastructureConfig infraConfig,
            IServiceCollection services
        )
        {
            var fileProvider = new PhysicalFileProvider(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates")
            );

            var config = infraConfig.Email;

            services
                .AddFluentEmail(config.FromEmail, config.FromName)
                .AddLiquidRenderer(c =>
                {
                    c.FileProvider = fileProvider;
                })
                .AddMailKitSender(
                    new SmtpClientOptions
                    {
                        Server = config.SmtpServerUrl,
                        Port = config.SmtpServerPort,
                        RequiresAuthentication = true,
                        User = config.FromEmail,
                        Password = config.AppPassword,
                    }
                );
        }

        private static void SetupDefaultDb(IConfiguration allConfig, IServiceCollection services)
        {
            var connectionString =
                allConfig.GetConnectionString("Default")
                ?? throw new ConfigurationErrorsException("ConnectionStrings:Default is not set");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        private static void SetupMessagePublishers(
            InfrastructureConfig infraConfig,
            IServiceCollection services
        )
        {
            var snsConfig = infraConfig.AwsSns;
            var sqsConfig = infraConfig.AwsSqs;

            services.AddAWSMessageBus(bus =>
            {
                bus.AddSQSPublisher<OrderFulfillReqMessageDto>(
                    sqsConfig.OrderFulfillRequests.QueueUrl,
                    OrderFulfillReqMessageDto.MessageType
                );

                bus.AddSNSPublisher<OrderEventMessageDto>(
                    snsConfig.OrderEvents.TopicUrl,
                    OrderEventMessageDto.MessageType
                );
            });
        }

        private static void SetupOpenSearch(
            InfrastructureConfig infraConfig,
            IServiceCollection services
        )
        {
            var config = infraConfig.OpenSearchApiClient;
            var connetionSettings = new ConnectionSettings(new Uri(config.Url));
            services.AddSingleton<IOpenSearchClient>(new OpenSearchClient(connetionSettings));
        }

        private static void SetupVisaApiClient(
            InfrastructureConfig infraConfig,
            IServiceCollection services
        )
        {
            var config = infraConfig.VisaApiClient;

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
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(config.Url));
        }
    }
}
