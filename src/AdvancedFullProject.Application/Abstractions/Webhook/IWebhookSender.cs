namespace AdvancedFullProject.Application.Abstractions.Webhook;

public interface IWebhookSender
{
    Task SendAsync(string eventName, object data, CancellationToken cancellationToken);
}
