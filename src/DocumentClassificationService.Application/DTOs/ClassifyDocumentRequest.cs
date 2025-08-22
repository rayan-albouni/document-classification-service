using Newtonsoft.Json;

namespace DocumentClassificationService.Application.DTOs;

public class ClassifyDocumentRequest
{
    [JsonProperty("blobUrl")]
    public string BlobUrl { get; set; } = string.Empty;

    [JsonProperty("tenantId")]
    public string TenantId { get; set; } = string.Empty;
}
