namespace DocumentClassificationService.Domain.Interfaces;

public interface IMessageBusService
{
    Task SendMessageAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;
    Task SendMessageAsync(string queueName, string messageBody, string? messageId = null, string? subject = null, CancellationToken cancellationToken = default);
}
