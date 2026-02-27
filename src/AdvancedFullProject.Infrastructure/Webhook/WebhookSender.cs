using System.Text;
using System.Text.Json;
using AdvancedFullProject.Application.Abstractions.Webhook;

namespace AdvancedFullProject.Infrastructure.Webhook;

public sealed class WebhookSender : IWebhookSender
{
    private readonly HttpClient _httpClient;
    public WebhookSender(HttpClient httpClient) => _httpClient = httpClient;

    public Task SendAsync(string eventName, object data, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new { eventName, data, occurredAt = DateTime.UtcNow });
        return _httpClient.PostAsync("https://example-webhook.local/events", new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken);
    }
}
