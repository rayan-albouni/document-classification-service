using DocumentClassificationService.Application.DTOs;

namespace DocumentClassificationService.Application.Validators;

public interface IClassifyDocumentValidator
{
    ValidationResult Validate(string documentId, ClassifyDocumentRequest request);
}

public class ClassifyDocumentValidator : IClassifyDocumentValidator
{
    public ValidationResult Validate(string documentId, ClassifyDocumentRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(documentId))
            errors.Add("DocumentId is required");

        if (request == null)
        {
            errors.Add("Request body is required");
            return new ValidationResult(false, errors);
        }

        if (string.IsNullOrWhiteSpace(request.BlobUrl))
            errors.Add("BlobUrl is required");

        if (string.IsNullOrWhiteSpace(request.TenantId))
            errors.Add("TenantId is required");

        if (!string.IsNullOrWhiteSpace(request.BlobUrl) && !IsValidUrl(request.BlobUrl))
            errors.Add("BlobUrl must be a valid URL");

        return new ValidationResult(errors.Count == 0, errors);
    }

    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

public record ValidationResult(bool IsValid, IReadOnlyList<string> Errors);
