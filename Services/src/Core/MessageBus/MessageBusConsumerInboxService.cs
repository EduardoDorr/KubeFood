using KubeFood.Core.Options;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KubeFood.Core.MessageBus;

public class MessageBusConsumerInboxService<T, TId, TDbContext> : MessageBusConsumerBase<T>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public MessageBusConsumerInboxService(
        IServiceProvider serviceProvider,
        ILogger<MessageBusConsumerInboxService<T, TId, TDbContext>> logger,
        IOptions<RabbitMqConfigurationOptions> rabbitMqConfigurationOptions)
        : base(logger, rabbitMqConfigurationOptions)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleMessageAsync(T message, IChannel channel, BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var inboxMessage = new InboxMessage<TId>
        {
            CreatedAt = DateTime.Now,
            Type = typeof(T).Name,
            Content = JsonConvert.SerializeObject(
                message,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                })
        };

        await context.Set<InboxMessage<TId>>().AddAsync(inboxMessage, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken: cancellationToken);
    }
}