using Azure.Messaging.ServiceBus;

using KubeFood.Core.CloudIntegration.Azure.Common.Options;
using KubeFood.Core.MessageBus;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KubeFood.Core.CloudIntegration.Azure.ServiceBus;

public abstract class AzureServiceBusConsumerBase<T> : BackgroundService, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;

    protected readonly ILogger _logger;
    protected readonly string _queue;
    protected readonly MessageBusConsumerOptions? _consumerOptions;

    protected AzureServiceBusConsumerBase(
        ILogger logger,
        IOptions<AzureServiceBusOptions> serviceBusOptions,
        MessageBusConsumerOptions? consumerOptions = null)
    {
        _logger = logger;
        _consumerOptions = consumerOptions;

        var options = serviceBusOptions.Value;
        _client = new ServiceBusClient(options.ConnectionString);
        _queue = _consumerOptions?.QueueName ?? typeof(T).Name;
        _processor = _client.CreateProcessor(_queue, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        });

        _processor.ProcessMessageAsync += OnMessageReceivedAsync;
        _processor.ProcessErrorAsync += OnProcessErrorAsync;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ServiceBus consumer started on queue {Queue}, waiting for messages...", _queue);

        await _processor.StartProcessingAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ServiceBus consumer stopping to listen on queue {Queue}...", _queue);
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
        await _client.DisposeAsync();
    }

    protected abstract Task HandleMessageAsync(T message, ProcessMessageEventArgs args, CancellationToken cancellationToken);

    private async Task OnMessageReceivedAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var json = args.Message.Body.ToString();

            _logger.LogInformation("Received message {MessageId} from queue {Queue}: {Message}", args.Message.MessageId, _queue, json);

            var message = args.Message.Body.ToObjectFromJson<T>();

            if (message is null)
            {
                _logger.LogWarning("Received null message {MessageId} from queue {Queue}, skipping processing", args.Message.MessageId, _queue);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                return;
            }

            await HandleMessageAsync(message, args, args.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId}, will requeue", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
        }
    }

    private Task OnProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error on Azure Service Bus source: {Source}", args.ErrorSource);
        return Task.CompletedTask;
    }
}