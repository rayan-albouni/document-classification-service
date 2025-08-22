namespace DocumentClassificationService.Infrastructure.Configuration;

public class DocumentIntelligenceSettings
{
    public const string SectionName = "DocumentIntelligence";

    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ClassifierModelId { get; set; } = string.Empty;
}
