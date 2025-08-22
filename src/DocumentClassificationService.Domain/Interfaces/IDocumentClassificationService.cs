using DocumentClassificationService.Domain.Entities;

namespace DocumentClassificationService.Domain.Interfaces;

public interface IDocumentClassificationService
{
    Task<ClassificationResult> ClassifyDocumentAsync(Document document, CancellationToken cancellationToken = default);
}
