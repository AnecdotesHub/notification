using Jevstafjev.Anecdotes.Notification.Definitions.Base;
using MediatR;

namespace Jevstafjev.Anecdotes.Notification.Definitions.Mediator;

public class MediatorDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());
    }
}
