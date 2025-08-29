using DocumentClassificationService.Application.Commands;
using DocumentClassificationService.Application.DTOs;
using DocumentClassificationService.Application.Handlers;
using DocumentClassificationService.Application.Validators;
using DocumentClassificationService.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

namespace DocumentClassificationService.Functions.Functions;

public class ClassifyDocumentFunction
{
    private readonly IClassifyDocumentHandler _handler;
    private readonly IClassifyDocumentValidator _validator;
    private readonly ILogger<ClassifyDocumentFunction> _logger;
    private readonly IMessageBusService _messageBusService;

    public ClassifyDocumentFunction(
        IClassifyDocumentHandler handler,
        IClassifyDocumentValidator validator,
        ILogger<ClassifyDocumentFunction> logger,
        IMessageBusService messageBusService)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageBusService = messageBusService ?? throw new ArgumentNullException(nameof(messageBusService));
    }

    [Function("ClassifyDocument")]
    public async Task RunAsync(
        [ServiceBusTrigger("document-classification-queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        string? documentId = null;
        try
        {
            _logger.LogInformation("Received document classification message. MessageId: {MessageId}", message.MessageId);

            var messageData = ParseMessage(message);
            documentId = messageData.DocumentId;

            _logger.LogInformation("Processing document classification for DocumentId: {DocumentId}", documentId);

            var request = new ClassifyDocumentRequest
            {
                BlobUrl = messageData.BlobUrl,
                TenantId = messageData.TenantId
            };

            var validationResult = _validator.Validate(documentId, request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for DocumentId: {DocumentId}. Errors: {Errors}", 
                    documentId, string.Join(", ", validationResult.Errors));
                
                await SendResultToQueueAsync(documentId, messageData.TenantId, messageData.BlobUrl, null, 0.0, "FAILED");
                await messageActions.CompleteMessageAsync(message);
                return;
            }

            var command = ClassifyDocumentCommand.Create(documentId, request);
            var response = await _handler.HandleAsync(command);

            await SendResultToQueueAsync(
                response.DocumentId,
                messageData.TenantId,
                messageData.BlobUrl,
                response.Classification,
                response.Confidence,
                response.Status);

            _logger.LogInformation("Document classification completed successfully for DocumentId: {DocumentId}", documentId);
            await messageActions.CompleteMessageAsync(message);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON in message body for DocumentId: {DocumentId}", documentId);
            await SendResultToQueueAsync(documentId ?? "unknown", "", "", null, 0.0, "FAILED");
            await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object> { { "Reason", "InvalidJSON" }, { "Error", ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing classification request for DocumentId: {DocumentId}", documentId);
            await SendResultToQueueAsync(documentId ?? "unknown", "", "", null, 0.0, "FAILED");
            await messageActions.DeadLetterMessageAsync(message, new Dictionary<string, object> { { "Reason", "ProcessingError" }, { "Error", ex.Message } });
        }
    }

    private static ClassifyDocumentMessage ParseMessage(ServiceBusReceivedMessage message)
    {
        var body = message.Body.ToString();
        
        if (string.IsNullOrWhiteSpace(body))
            throw new JsonException("Message body cannot be empty");

        return JsonConvert.DeserializeObject<ClassifyDocumentMessage>(body) 
            ?? throw new JsonException("Failed to deserialize message body");
    }

    private async Task SendResultToQueueAsync(string documentId, string tenantId, string blobUrl, string? documentType, double confidenceScore, string status)
    {
        try
        {
            var result = new DocumentClassificationResultMessage
            {
                DocumentId = Guid.Parse(documentId),
                TenantId = tenantId,
                DocumentType = documentType ?? "Unknown",
                BlobUrl = blobUrl,
                ConfidenceScore = confidenceScore,
                Status = status
            };

            await _messageBusService.SendMessageAsync("document-classification-results", result);
            _logger.LogInformation("Sent classification result to queue for DocumentId: {DocumentId}", documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send result to queue for DocumentId: {DocumentId}", documentId);
            // Don't rethrow - we don't want to fail the message processing just because we couldn't send the result
        }
    }
}
