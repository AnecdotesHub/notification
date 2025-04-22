namespace Jevstafjev.Anecdotes.Notification.Application.Messaging.DeliveryMessages.ViewModels;

public class TagViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int AnecdotesCount { get; set; }
}
