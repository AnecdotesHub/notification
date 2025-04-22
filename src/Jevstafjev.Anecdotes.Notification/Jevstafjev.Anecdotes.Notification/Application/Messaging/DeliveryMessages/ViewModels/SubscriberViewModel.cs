namespace Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.ViewModels;

public class SubscriberViewModel
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
}
