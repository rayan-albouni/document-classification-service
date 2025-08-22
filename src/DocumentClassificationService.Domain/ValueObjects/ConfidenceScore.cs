namespace DocumentClassificationService.Domain.ValueObjects;

public record ConfidenceScore
{
    public double Value { get; init; }

    public ConfidenceScore(double value)
    {
        if (value < 0.0 || value > 1.0)
            throw new ArgumentException("ConfidenceScore must be between 0.0 and 1.0", nameof(value));
        
        Value = value;
    }

    public static implicit operator double(ConfidenceScore confidence) => confidence.Value;
    public static implicit operator ConfidenceScore(double value) => new(value);

    public override string ToString() => Value.ToString("F2");
}
