using Jevstafjev.Anecdotes.Notification.Application.Services;
using Jevstafjev.Anecdotes.Notification.Definitions.Base;
using SendGrid;

namespace Jevstafjev.Anecdotes.Notification.Definitions.DependencyContainer;

public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            return new SendGridClient(configuration["SendGrid:ApiKey"]);
        });
    }
}
