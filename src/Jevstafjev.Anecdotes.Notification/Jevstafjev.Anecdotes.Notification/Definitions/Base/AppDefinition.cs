namespace Jevstafjev.Anecdotes.Notification.Definitions.Base;

public abstract class AppDefinition : IAppDefinition
{
    public virtual void ConfigureServices(WebApplicationBuilder builder) { }

    public virtual void ConfigureApplication(WebApplication app) { }
}
