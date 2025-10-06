using Azure.Messaging.ServiceBus;

using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.MessageBus;

using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Text.Json;

namespace KubeFood.Core.CloudIntegration.Azure.ServiceBus;

public class AzureServiceBusProducerService : IMessageBusProducerService, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = [];

    public AzureServiceBusProducerService(IOptions<AzureServiceBusOptions> serviceBusOptions)
    {
        var options = serviceBusOptions.Value;
        _client = new ServiceBusClient(options.ConnectionString);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string? queue = null, CancellationToken cancellationToken = default)
    {
        var queueName = queue ?? typeof(TEvent).Name;

        var sender = _senders.GetOrAdd(queueName, _client.CreateSender);

        var eventMessageJsonString = JsonSerializer.Serialize(@event);
        var message = new ServiceBusMessage(eventMessageJsonString);

        await sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var sender in _senders)
            await sender.Value.DisposeAsync();

        await _client.DisposeAsync();
    }
}