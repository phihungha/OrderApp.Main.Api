using FluentEmail.Core;
using OrderApp.Main.Api.Application.Interfaces.ExternalServices;

namespace OrderApp.Main.Api.Infrastructure
{
    public class EmailService(IFluentEmail fluentEmail) : IEmailService
    {
        private readonly IFluentEmail fluentEmail = fluentEmail;

        public async Task SendMail<T>(
            string toName,
            string toEmail,
            string subject,
            string templateName,
            T viewModel
        )
        {
            var templateFilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "EmailTemplates",
                $"{templateName}.liquid"
            );

            var response = await fluentEmail
                .To(toEmail, toName)
                .Subject(subject)
                .UsingTemplateFromFile(templateFilePath, viewModel)
                .SendAsync();

            if (!response.Successful)
            {
                throw new ApplicationException(
                    $"Failed to send email: {string.Join(", ", response.ErrorMessages)}"
                );
            }
        }
    }
}
