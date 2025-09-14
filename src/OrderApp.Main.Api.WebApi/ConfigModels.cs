using OrderApp.Main.Api.Infrastructure;

namespace OrderApp.Main.Api.Jobs
{
    public record AppConfig
    {
        public required AwsSnsConfig AwsSns { get; set; }
        public required AwsSqsConfig AwsSqs { get; set; }
        public required EmailConfig Email { get; set; }
        public required OpenSearchApiClientConfig OpenSearchApiClient { get; set; }
        public required VisaApiClientConfig VisaApiClient { get; set; }
    }
}
