using DocumentClassificationService.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DocumentClassificationService.Tests.Domain.ValueObjects;

public class DocumentIdTests
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateInstance()
    {
        // Arrange
        var value = "test-doc-123";

        // Act
        var documentId = new DocumentId(value);

        // Assert
        documentId.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidValue_ShouldThrowArgumentException(string value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new DocumentId(value))
            .Should().Throw<ArgumentException>()
            .WithMessage("DocumentId cannot be null or empty*");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var documentId = new DocumentId("test-doc-123");

        // Act
        string value = documentId;

        // Assert
        value.Should().Be("test-doc-123");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldCreateInstance()
    {
        // Arrange
        var value = "test-doc-123";

        // Act
        DocumentId documentId = value;

        // Assert
        documentId.Value.Should().Be(value);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var documentId = new DocumentId("test-doc-123");

        // Act
        var result = documentId.ToString();

        // Assert
        result.Should().Be("test-doc-123");
    }
}
