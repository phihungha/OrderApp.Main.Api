using OrderApp.Main.Api.Infrastructure;

namespace OrderApp.Main.Api.Jobs
{
    public record AuthConfig
    {
        public required JwtConfig Jwt { get; set; }
    }

    public record JwtConfig
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
    }

    public record AppConfig
    {
        public required AuthConfig Auth { get; set; }
        public required AwsSnsConfig AwsSns { get; set; }
        public required AwsSqsConfig AwsSqs { get; set; }
        public required EmailConfig Email { get; set; }
        public required OpenSearchApiClientConfig OpenSearchApiClient { get; set; }
        public required VisaApiClientConfig VisaApiClient { get; set; }
    }
}
