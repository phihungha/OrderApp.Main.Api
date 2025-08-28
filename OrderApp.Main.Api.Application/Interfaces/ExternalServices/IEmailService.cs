namespace OrderApp.Main.Api.Application.Interfaces.ExternalServices
{
    public interface IEmailService
    {
        Task SendMail<T>(
            string toName,
            string toEmail,
            string subject,
            string templateName,
            T viewModel
        );
    }
}
