using KubeFood.Core.Events;
using KubeFood.Core.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace KubeFood.Core.MessageBus;

public class MessageBusProducerRabbitMqService : IMessageBusProducerService
{
    private readonly ConnectionFactory _connectionFactory;

    public MessageBusProducerRabbitMqService(IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
    {
        var rabbitMqConfigurationOptionsValue = rabbitMqConfigurationOptions.Value;

        _connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMqConfigurationOptionsValue.HostName,
            Port = rabbitMqConfigurationOptionsValue.Port,
            UserName = rabbitMqConfigurationOptionsValue.UserName,
            Password = rabbitMqConfigurationOptionsValue.Password
        };
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string? queue = null, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var body = GetBodyMessage(@event);

        await channel.QueueDeclareAsync
            (
                queue: queue ?? typeof(TEvent).Name,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: null,
                cancellationToken: cancellationToken
            );

        var properties = new BasicProperties();

        await channel.BasicPublishAsync
            (
                exchange: "",
                routingKey: queue ?? typeof(TEvent).Name,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );
    }

    private static byte[] GetBodyMessage<TEvent>(TEvent @event)
    {
        var eventMessageJsonString = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(eventMessageJsonString);

        return body;
    }
}