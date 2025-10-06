using Azure.Messaging.ServiceBus;

using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.Events;
using KubeFood.Core.MessageBus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.CloudIntegration.Azure.ServiceBus;

public class AzureServiceBusConsumerEventService<T> : AzureServiceBusConsumerBase<T>
    where T : IEvent
{
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusConsumerEventService(
        IServiceProvider serviceProvider,
        ILogger<AzureServiceBusConsumerEventService<T>> logger,
        IOptions<AzureServiceBusOptions> serviceBusOptions,
        MessageBusConsumerOptions? consumerOptions = null)
        : base(logger, serviceBusOptions, consumerOptions)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleMessageAsync(T message, ProcessMessageEventArgs args, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<IMessageBusConsumerEventHandler<T>>();

        var result = await handler.ConsumeAsync(message, cancellationToken);

        if (result == MessageBusConsumerEventResult.Ack)
            await args.CompleteMessageAsync(args.Message, cancellationToken);
        else
        {
            _logger.LogWarning("Message processing return Nack for {MessageId}, requeuing", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message, cancellationToken: cancellationToken);
        }
    }
}