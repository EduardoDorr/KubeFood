using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KubeFood.Core.RabbitMq;

public class RabbitMqMessageBusConsumerInboxService<T, TId, TDbContext> : MessageBusConsumerBase<T>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqMessageBusConsumerInboxService(
        IServiceProvider serviceProvider,
        ILogger<RabbitMqMessageBusConsumerInboxService<T, TId, TDbContext>> logger,
        IOptions<RabbitMqOptions> rabbitMqConfigurationOptions,
        MessageBusConsumerOptions? consumerOptions = null)
        : base(logger, rabbitMqConfigurationOptions, consumerOptions)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleMessageAsync(T message, IChannel channel, BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var internalEventName = _consumerOptions?.InternalEventName ?? typeof(T).Name;

        var inboxMessage = new InboxMessage<TId>
        {
            CreatedAt = DateTime.Now,
            Type = internalEventName,
            Content = JsonConvert.SerializeObject(
                message,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                })
        };

        await context.Set<InboxMessage<TId>>().AddAsync(inboxMessage, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
    }
}