using Newtonsoft.Json;

namespace DocumentClassificationService.Application.DTOs;

public class ClassifyDocumentMessage
{
    [JsonProperty("documentId")]
    public string DocumentId { get; set; } = string.Empty;

    [JsonProperty("blobUrl")]
    public string BlobUrl { get; set; } = string.Empty;

    [JsonProperty("tenantId")]
    public string TenantId { get; set; } = string.Empty;
}
