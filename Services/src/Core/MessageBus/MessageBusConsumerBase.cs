using System.Text;

using KubeFood.Core.Options;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KubeFood.Core.MessageBus;

public abstract class MessageBusConsumerBase<T> : BackgroundService
{
    private const int MAX_RETRY_DELAY_SECONDS = 30;
    private const int INITIAL_RETRY_DELAY_SECONDS = 5;

    private TimeSpan _retryDelay = TimeSpan.FromSeconds(INITIAL_RETRY_DELAY_SECONDS);

    private readonly ConnectionFactory _connectionFactory;

    protected readonly ILogger _logger;
    protected readonly string _queue = typeof(T).Name;

    protected MessageBusConsumerBase(
        ILogger logger,
        IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
    {
        _logger = logger;
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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
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
                    try
                    {
                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                        _logger.LogInformation("Received message from queue {Queue}: {Message}", _queue, json);

                        var message = JsonConvert.DeserializeObject<T>(json);

                        if (message is null)
                        {
                            _logger.LogWarning("Received null message from queue {Queue}, skipping processing", _queue);
                            await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken: stoppingToken);
                            return;
                        }

                        await HandleMessageAsync(message, channel, ea, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message, will requeue");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                await channel.BasicConsumeAsync
                    (
                        queue: _queue,
                        autoAck: false,
                        consumer: consumer,
                        cancellationToken: stoppingToken
                    );

                _logger.LogInformation("RabbitMQ consumer started on queue {Queue}, waiting for messages...", _queue);

                _retryDelay = TimeSpan.FromSeconds(INITIAL_RETRY_DELAY_SECONDS);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection lost, retrying in {RetryDelay}s", _retryDelay.TotalSeconds);

                await Task.Delay(_retryDelay, stoppingToken);

                _retryDelay = TimeSpan.FromSeconds(Math.Min(_retryDelay.TotalSeconds * 2, MAX_RETRY_DELAY_SECONDS));
            }
        }
    }

    protected abstract Task HandleMessageAsync(T message, IChannel channel, BasicDeliverEventArgs ea, CancellationToken cancellationToken);
}