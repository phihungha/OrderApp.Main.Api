using OrderApp.Main.Api.Infrastructure;

namespace OrderApp.Main.Api.Jobs
{
    public record AwsSqsConfig
    {
        public const string Key = "AwsSqs";

        public required AwsSqsHandlerConfig OrderFulfillRequests { get; set; }
        public required AwsSqsHandlerConfig OrderUpdates { get; set; }
    }

    public record AppConfig
    {
        public required AwsSnsConfig AwsSns { get; set; }
        public required AwsSqsConfig AwsSqs { get; set; }
        public required EmailConfig Email { get; set; }
        public required OpenSearchApiClientConfig OpenSearchApiClient { get; set; }
        public required VisaApiClientConfig VisaApiClient { get; set; }
    }
}
