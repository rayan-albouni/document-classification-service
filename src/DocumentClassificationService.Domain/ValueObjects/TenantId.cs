namespace DocumentClassificationService.Domain.ValueObjects;

public record TenantId
{
    public string Value { get; init; }

    public TenantId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("TenantId cannot be null or empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(TenantId tenantId) => tenantId.Value;
    public static implicit operator TenantId(string value) => new(value);

    public override string ToString() => Value;
}
