namespace DocumentClassificationService.Domain.Entities;

public class Document
{
    public DocumentId Id { get; private set; }
    public string BlobUrl { get; private set; }
    public TenantId TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Document(DocumentId id, string blobUrl, TenantId tenantId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        BlobUrl = !string.IsNullOrWhiteSpace(blobUrl) ? blobUrl : throw new ArgumentException("BlobUrl cannot be null or empty", nameof(blobUrl));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        CreatedAt = DateTime.UtcNow;
    }
}
