using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using KubeFood.Core.Options;

namespace KubeFood.Core.MessageBus;

public class MessageBusProducerService : IMessageBusProducerService
{
    private readonly ConnectionFactory _connectionFactory;

    public MessageBusProducerService(IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
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

    public async Task PublishAsync<T>(string queue, T @event, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var body = GetBodyMessage(@event);

        await channel.QueueDeclareAsync
            (
                queue: queue,
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
                routingKey: queue,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken
            );
    }

    private static byte[] GetBodyMessage<T>(T @event)
    {
        var eventMessageJsonString = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(eventMessageJsonString);

        return body;
    }
}