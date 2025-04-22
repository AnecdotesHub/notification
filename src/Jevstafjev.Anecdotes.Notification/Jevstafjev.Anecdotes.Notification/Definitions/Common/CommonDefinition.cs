using Jevstafjev.Anecdotes.Notification.Definitions.Base;

namespace Jevstafjev.Anecdotes.Notification.Definitions.Common
{
    public class CommonDefinition : AppDefinition
    {
        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddRazorPages();
        }

        public override void ConfigureApplication(WebApplication app)
        {
            app.UseHttpsRedirection();
            app.MapRazorPages();
        }
    }
}
