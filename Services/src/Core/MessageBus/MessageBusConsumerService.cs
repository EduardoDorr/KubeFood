using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using KubeFood.Core.Options;

namespace KubeFood.Core.MessageBus;

public class MessageBusConsumerService<T> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queue = typeof(T).Name;

    public MessageBusConsumerService(IServiceProvider serviceProvider, IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
    {
        _serviceProvider = serviceProvider;
        var rabbitMqConfigurationOptionsValue = rabbitMqConfigurationOptions.Value;

        _connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMqConfigurationOptionsValue.HostName,
            Port = rabbitMqConfigurationOptionsValue.Port,
            UserName = rabbitMqConfigurationOptionsValue.UserName,
            Password = rabbitMqConfigurationOptionsValue.Password
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync
            (
                queue: _queue,
                durable: false,
                exclusive: false,
                autoDelete: true,
                arguments: null,
                cancellationToken: stoppingToken
            );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonSerializer.Deserialize<T>(json);

            using var scope = _serviceProvider.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageBusConsumerServiceHandler<T>>();

            var result = await handler.ConsumeAsync(message, stoppingToken);

            if (result == MessageBusConsumerResult.Ack)
                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            else
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, stoppingToken);
        };

        await channel.BasicConsumeAsync
            (
                queue: _queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}