namespace Jevstafjev.Anecdotes.Notification.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string recipientEmail, string subject, string plainTextContent, string htmlContent);
}
