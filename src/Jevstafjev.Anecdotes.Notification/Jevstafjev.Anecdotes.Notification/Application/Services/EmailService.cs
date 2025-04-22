using SendGrid;
using SendGrid.Helpers.Mail;

namespace Jevstafjev.Anecdotes.Notification.Application.Services;

public class EmailService(SendGridClient client, IConfiguration configuration) : IEmailService
{
    public async Task SendEmailAsync(string recipientEmail, string subject, string plainTextContent, string htmlContent)
    {
        var from = new EmailAddress(configuration["SendGrid:FromEmail"]);
        var to = new EmailAddress(recipientEmail);

        var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error: {response.StatusCode}, {error}");
        }
    }
}
