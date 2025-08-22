using Newtonsoft.Json;

namespace DocumentClassificationService.Application.DTOs;

public class ClassifyDocumentResponse
{
    [JsonProperty("documentId")]
    public string DocumentId { get; set; } = string.Empty;

    [JsonProperty("classification")]
    public string Classification { get; set; } = string.Empty;

    [JsonProperty("confidence")]
    public double Confidence { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("classifierModelUsed")]
    public string ClassifierModelUsed { get; set; } = string.Empty;
}
