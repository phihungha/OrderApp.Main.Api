namespace OrderApp.Main.Api.Infrastructure
{
    public record AwsSnsConfig
    {
        public const string Key = "AwsSns";

        public required AwsSnsPublisherConfig OrderEvents { get; set; }
    }

    public record AwsSnsPublisherConfig
    {
        public required string TopicUrl { get; set; }
    }

    public record AwsSqsConfig
    {
        public const string Key = "AwsSqs";

        public required AwsSqsHandlerConfig OrderFulfillRequests { get; set; }
    }

    public record AwsSqsHandlerConfig
    {
        public required string QueueUrl { get; set; }
    }

    public record EmailConfig
    {
        public const string Key = "Email";

        public required string FromName { get; set; }
        public required string FromEmail { get; set; }
        public required string AppPassword { get; set; }
        public required string SmtpServerUrl { get; set; }
        public required int SmtpServerPort { get; set; }
    }

    public record OpenSearchApiClientConfig
    {
        public const string Key = "OpenSearchApiClient";

        public required string Url { get; set; }
    }

    public record VisaApiClientConfig
    {
        public const string Key = "VisaApiClient";

        public required string Url { get; set; }
    }

    public record InfrastructureConfig
    {
        public required AwsSnsConfig AwsSns { get; set; }
        public required AwsSqsConfig AwsSqs { get; set; }
        public required EmailConfig Email { get; set; }
        public required OpenSearchApiClientConfig OpenSearchApiClient { get; set; }
        public required VisaApiClientConfig VisaApiClient { get; set; }
    }
}
