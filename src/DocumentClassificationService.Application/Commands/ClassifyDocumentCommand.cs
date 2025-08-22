using DocumentClassificationService.Application.DTOs;

namespace DocumentClassificationService.Application.Commands;

public record ClassifyDocumentCommand(
    string DocumentId,
    string BlobUrl,
    string TenantId) 
{
    public static ClassifyDocumentCommand Create(string documentId, ClassifyDocumentRequest request)
    {
        return new ClassifyDocumentCommand(documentId, request.BlobUrl, request.TenantId);
    }
}
