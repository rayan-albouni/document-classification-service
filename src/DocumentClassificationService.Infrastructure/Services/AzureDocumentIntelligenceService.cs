using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentClassificationService.Domain.Entities;
using DocumentClassificationService.Domain.Interfaces;
using DocumentClassificationService.Domain.ValueObjects;
using DocumentClassificationService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocumentClassificationService.Infrastructure.Services;

public class AzureDocumentIntelligenceService : IDocumentClassificationService
{
    private readonly DocumentAnalysisClient _client;
    private readonly DocumentIntelligenceSettings _settings;
    private readonly ILogger<AzureDocumentIntelligenceService> _logger;

    public AzureDocumentIntelligenceService(
        DocumentAnalysisClient client,
        IOptions<DocumentIntelligenceSettings> settings,
        ILogger<AzureDocumentIntelligenceService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ClassificationResult> ClassifyDocumentAsync(Document document, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting document classification for DocumentId: {DocumentId} using model: {ModelId}", 
                document.Id, _settings.ClassifierModelId);

            var operation = await _client.ClassifyDocumentFromUriAsync(
                WaitUntil.Completed,
                _settings.ClassifierModelId,
                new Uri(document.BlobUrl),
                cancellationToken: cancellationToken);

            var result = operation.Value;
            var topClassification = GetTopClassification(result);

            _logger.LogInformation("Document classification completed for DocumentId: {DocumentId}, Classification: {Classification}, Confidence: {Confidence}", 
                document.Id, topClassification.DocumentType, topClassification.Confidence);

            return new ClassificationResult(
                document.Id,
                new DocumentType(topClassification.DocumentType),
                new ConfidenceScore(topClassification.Confidence),
                _settings.ClassifierModelId);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure Document Intelligence API error for DocumentId: {DocumentId}. Status: {Status}, Error: {Error}", 
                document.Id, ex.Status, ex.Message);
            throw new InvalidOperationException($"Document classification failed: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during document classification for DocumentId: {DocumentId}", document.Id);
            throw;
        }
    }

    private static (string DocumentType, double Confidence) GetTopClassification(AnalyzeResult result)
    {
        if (result.Documents?.FirstOrDefault() is not { } document)
            throw new InvalidOperationException("No classification results returned from Azure Document Intelligence");

        if (document.DocumentType is null)
            throw new InvalidOperationException("Document type is null in classification result");

        return (document.DocumentType, (double)document.Confidence);
    }
}
