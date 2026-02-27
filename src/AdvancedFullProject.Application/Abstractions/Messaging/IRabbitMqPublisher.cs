namespace AdvancedFullProject.Application.Abstractions.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync(string routingKey, object payload);
}
