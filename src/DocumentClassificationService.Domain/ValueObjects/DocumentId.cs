namespace DocumentClassificationService.Domain.ValueObjects;

public record DocumentId
{
    public string Value { get; init; }

    public DocumentId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("DocumentId cannot be null or empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(DocumentId documentId) => documentId.Value;
    public static implicit operator DocumentId(string value) => new(value);

    public override string ToString() => Value;
}
