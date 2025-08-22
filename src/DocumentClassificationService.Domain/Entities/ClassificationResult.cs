namespace DocumentClassificationService.Domain.Entities;

public class ClassificationResult
{
    public DocumentId DocumentId { get; private set; }
    public DocumentType Classification { get; private set; }
    public ConfidenceScore Confidence { get; private set; }
    public ClassificationStatus Status { get; private set; }
    public string ClassifierModelUsed { get; private set; }
    public DateTime ClassifiedAt { get; private set; }

    private ClassificationResult() { }

    public ClassificationResult(
        DocumentId documentId, 
        DocumentType classification, 
        ConfidenceScore confidence, 
        string classifierModelUsed)
    {
        DocumentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
        Classification = classification ?? throw new ArgumentNullException(nameof(classification));
        Confidence = confidence ?? throw new ArgumentNullException(nameof(confidence));
        ClassifierModelUsed = !string.IsNullOrWhiteSpace(classifierModelUsed) 
            ? classifierModelUsed 
            : throw new ArgumentException("ClassifierModelUsed cannot be null or empty", nameof(classifierModelUsed));
        Status = ClassificationStatus.Classified;
        ClassifiedAt = DateTime.UtcNow;
    }
}
