using Newtonsoft.Json;

namespace DocumentClassificationService.Application.DTOs;

public class DocumentClassificationResultMessage
{
    [JsonProperty("documentId")]
    public Guid DocumentId { get; set; }

    [JsonProperty("tenantId")]
    public string TenantId { get; set; } = string.Empty;

    [JsonProperty("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonProperty("blobUrl")]
    public string BlobUrl { get; set; } = string.Empty;

    [JsonProperty("confidenceScore")]
    public double ConfidenceScore { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
}
