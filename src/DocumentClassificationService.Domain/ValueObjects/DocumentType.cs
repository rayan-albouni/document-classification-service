namespace DocumentClassificationService.Domain.ValueObjects;

public record DocumentType
{
    public string Value { get; init; }

    public DocumentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("DocumentType cannot be null or empty", nameof(value));
        
        Value = value;
    }

    public static DocumentType Invoice => new("Invoice");
    public static DocumentType Receipt => new("Receipt");
    public static DocumentType Contract => new("Contract");

    public static implicit operator string(DocumentType documentType) => documentType.Value;
    public static implicit operator DocumentType(string value) => new(value);

    public override string ToString() => Value;
}
