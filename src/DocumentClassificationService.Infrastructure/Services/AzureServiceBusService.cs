using Azure.Messaging.ServiceBus;
using DocumentClassificationService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DocumentClassificationService.Infrastructure.Services;

public class AzureServiceBusService : IMessageBusService, IDisposable
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<AzureServiceBusService> _logger;
    private readonly Dictionary<string, ServiceBusSender> _senders;

    public AzureServiceBusService(ServiceBusClient serviceBusClient, ILogger<AzureServiceBusService> logger)
    {
        _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _senders = new Dictionary<string, ServiceBusSender>();
    }

    public async Task SendMessageAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var messageBody = JsonConvert.SerializeObject(message);
            await SendMessageAsync(queueName, messageBody, Guid.NewGuid().ToString(), typeof(T).Name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message of type {MessageType} to queue {QueueName}", typeof(T).Name, queueName);
            throw;
        }
    }

    public async Task SendMessageAsync(string queueName, string messageBody, string? messageId = null, string? subject = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var sender = GetOrCreateSender(queueName);
            
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                MessageId = messageId ?? Guid.NewGuid().ToString(),
                Subject = subject
            };

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
            
            _logger.LogInformation("Successfully sent message to queue {QueueName} with MessageId: {MessageId}", queueName, serviceBusMessage.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to queue {QueueName}", queueName);
            throw;
        }
    }

    private ServiceBusSender GetOrCreateSender(string queueName)
    {
        if (!_senders.TryGetValue(queueName, out var sender))
        {
            sender = _serviceBusClient.CreateSender(queueName);
            _senders[queueName] = sender;
        }
        return sender;
    }

    public void Dispose()
    {
        foreach (var sender in _senders.Values)
        {
            sender.DisposeAsync().GetAwaiter().GetResult();
        }
        _senders.Clear();
        
        _serviceBusClient?.DisposeAsync().GetAwaiter().GetResult();
    }
}
