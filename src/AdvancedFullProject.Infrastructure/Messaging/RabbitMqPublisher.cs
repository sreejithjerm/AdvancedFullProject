using System.Text;
using System.Text.Json;
using AdvancedFullProject.Application.Abstractions.Messaging;
using RabbitMQ.Client;

namespace AdvancedFullProject.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IRabbitMqPublisher
{
    public Task PublishAsync(string routingKey, object payload)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("events", ExchangeType.Topic, durable: true);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        channel.BasicPublish("events", routingKey, null, body);
        return Task.CompletedTask;
    }
}
