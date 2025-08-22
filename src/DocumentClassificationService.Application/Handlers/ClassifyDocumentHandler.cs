using DocumentClassificationService.Application.Commands;
using DocumentClassificationService.Application.DTOs;
using DocumentClassificationService.Domain.Entities;
using DocumentClassificationService.Domain.Interfaces;
using DocumentClassificationService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DocumentClassificationService.Application.Handlers;

public interface IClassifyDocumentHandler
{
    Task<ClassifyDocumentResponse> HandleAsync(ClassifyDocumentCommand command, CancellationToken cancellationToken = default);
}

public class ClassifyDocumentHandler : IClassifyDocumentHandler
{
    private readonly IDocumentClassificationService _classificationService;
    private readonly ILogger<ClassifyDocumentHandler> _logger;

    public ClassifyDocumentHandler(
        IDocumentClassificationService classificationService,
        ILogger<ClassifyDocumentHandler> logger)
    {
        _classificationService = classificationService ?? throw new ArgumentNullException(nameof(classificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ClassifyDocumentResponse> HandleAsync(ClassifyDocumentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting document classification for DocumentId: {DocumentId}", command.DocumentId);

            var document = CreateDocument(command);
            var result = await _classificationService.ClassifyDocumentAsync(document, cancellationToken);
            var response = MapToResponse(result);

            _logger.LogInformation("Document classification completed successfully for DocumentId: {DocumentId}, Classification: {Classification}, Confidence: {Confidence}", 
                command.DocumentId, result.Classification, result.Confidence);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while classifying document with DocumentId: {DocumentId}", command.DocumentId);
            throw;
        }
    }

    private static Document CreateDocument(ClassifyDocumentCommand command)
    {
        return new Document(
            new DocumentId(command.DocumentId),
            command.BlobUrl,
            new TenantId(command.TenantId));
    }

    private static ClassifyDocumentResponse MapToResponse(ClassificationResult result)
    {
        return new ClassifyDocumentResponse
        {
            DocumentId = result.DocumentId.Value,
            Classification = result.Classification.Value,
            Confidence = result.Confidence.Value,
            Status = result.Status.ToString().ToUpperInvariant(),
            ClassifierModelUsed = result.ClassifierModelUsed
        };
    }
}
