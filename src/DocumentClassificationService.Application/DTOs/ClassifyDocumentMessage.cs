using Newtonsoft.Json;

namespace DocumentClassificationService.Application.DTOs;

public class ClassifyDocumentMessage
{
    public string DocumentId { get; set; } = string.Empty;

    public string BlobUrl { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;
}
