namespace OrderApp.Main.Api.Infrastructure
{
    public record EmailConfig
    {
        public const string Email = "Email";

        public required string FromName { get; set; }
        public required string FromEmail { get; set; }
        public required string AppPassword { get; set; }
        public required string SmtpServerUrl { get; set; }
        public required int SmtpServerPort { get; set; }
    }
}
