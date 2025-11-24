using Azure.Messaging.ServiceBus;

using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.MessageBus;
using KubeFood.Core.Persistence.BoxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace KubeFood.Core.CloudIntegration.Azure.ServiceBus;

public class AzureServiceBusConsumerInboxService<T, TId, TDbContext> : AzureServiceBusConsumerBase<T>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusConsumerInboxService(
        IServiceProvider serviceProvider,
        ILogger<AzureServiceBusConsumerInboxService<T, TId, TDbContext>> logger,
        IOptions<AzureServiceBusOptions> serviceBusOptions,
        MessageBusConsumerOptions? consumerOptions = null)
        : base(logger, serviceBusOptions, consumerOptions)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleMessageAsync(T message, ProcessMessageEventArgs args, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();

        var internalEventName = _consumerOptions?.InternalEventName ?? typeof(T).Name;

        var inboxMessage = new InboxMessage<TId>
        {
            IdempotencyId = args.Message.MessageId,
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

        await args.CompleteMessageAsync(args.Message, cancellationToken);
    }
}