namespace DocumentClassificationService.Domain.Interfaces;

public interface IMessageBusService
{
    Task SendMessageAsync<T>(string queueName, T message);
}
