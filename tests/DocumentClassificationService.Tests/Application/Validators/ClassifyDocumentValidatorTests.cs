using DocumentClassificationService.Application.DTOs;
using DocumentClassificationService.Application.Validators;
using FluentAssertions;
using Xunit;

namespace DocumentClassificationService.Tests.Application.Validators;

public class ClassifyDocumentValidatorTests
{
    private readonly ClassifyDocumentValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var documentId = "test-doc-123";
        var request = new ClassifyDocumentRequest
        {
            BlobUrl = "https://example.com/document.pdf",
            TenantId = "tenant-123"
        };

        // Act
        var result = _validator.Validate(documentId, request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithInvalidDocumentId_ShouldReturnFailure(string documentId)
    {
        // Arrange
        var request = new ClassifyDocumentRequest
        {
            BlobUrl = "https://example.com/document.pdf",
            TenantId = "tenant-123"
        };

        // Act
        var result = _validator.Validate(documentId, request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DocumentId is required");
    }

    [Fact]
    public void Validate_WithNullRequest_ShouldReturnFailure()
    {
        // Arrange
        var documentId = "test-doc-123";

        // Act
        var result = _validator.Validate(documentId, null!);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Request body is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithInvalidBlobUrl_ShouldReturnFailure(string blobUrl)
    {
        // Arrange
        var documentId = "test-doc-123";
        var request = new ClassifyDocumentRequest
        {
            BlobUrl = blobUrl,
            TenantId = "tenant-123"
        };

        // Act
        var result = _validator.Validate(documentId, request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("BlobUrl is required");
    }

    [Fact]
    public void Validate_WithInvalidUrlFormat_ShouldReturnFailure()
    {
        // Arrange
        var documentId = "test-doc-123";
        var request = new ClassifyDocumentRequest
        {
            BlobUrl = "not-a-valid-url",
            TenantId = "tenant-123"
        };

        // Act
        var result = _validator.Validate(documentId, request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("BlobUrl must be a valid URL");
    }
}
