using DocumentClassificationService.Application.Commands;
using DocumentClassificationService.Application.DTOs;
using DocumentClassificationService.Application.Handlers;
using DocumentClassificationService.Application.Validators;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace DocumentClassificationService.Functions.Functions;

public class ClassifyDocumentFunction
{
    private readonly IClassifyDocumentHandler _handler;
    private readonly IClassifyDocumentValidator _validator;
    private readonly ILogger<ClassifyDocumentFunction> _logger;

    public ClassifyDocumentFunction(
        IClassifyDocumentHandler handler,
        IClassifyDocumentValidator validator,
        ILogger<ClassifyDocumentFunction> logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Function("ClassifyDocument")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/documents/{documentId}/classify")] HttpRequestData req,
        string documentId)
    {
        try
        {
            _logger.LogInformation("Received document classification request for DocumentId: {DocumentId}", documentId);

            var request = await ParseRequestAsync(req);
            var validationResult = _validator.Validate(documentId, request);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for DocumentId: {DocumentId}. Errors: {Errors}", 
                    documentId, string.Join(", ", validationResult.Errors));
                return await CreateBadRequestResponseAsync(req, validationResult.Errors);
            }

            var command = ClassifyDocumentCommand.Create(documentId, request);
            var response = await _handler.HandleAsync(command);

            _logger.LogInformation("Document classification completed successfully for DocumentId: {DocumentId}", documentId);
            return await CreateSuccessResponseAsync(req, response);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON in request body for DocumentId: {DocumentId}", documentId);
            return await CreateBadRequestResponseAsync(req, new[] { "Invalid JSON format in request body" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing classification request for DocumentId: {DocumentId}", documentId);
            return await CreateInternalServerErrorResponseAsync(req);
        }
    }

    private static async Task<ClassifyDocumentRequest> ParseRequestAsync(HttpRequestData req)
    {
        var body = await req.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(body))
            throw new JsonException("Request body cannot be empty");

        return JsonConvert.DeserializeObject<ClassifyDocumentRequest>(body) 
            ?? throw new JsonException("Failed to deserialize request body");
    }

    private static async Task<HttpResponseData> CreateSuccessResponseAsync(HttpRequestData req, ClassifyDocumentResponse response)
    {
        var httpResponse = req.CreateResponse(HttpStatusCode.OK);
        httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var json = JsonConvert.SerializeObject(response, Formatting.Indented);
        await httpResponse.WriteStringAsync(json);
        
        return httpResponse;
    }

    private static async Task<HttpResponseData> CreateBadRequestResponseAsync(HttpRequestData req, IEnumerable<string> errors)
    {
        var httpResponse = req.CreateResponse(HttpStatusCode.BadRequest);
        httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var errorResponse = new { errors = errors.ToArray() };
        var json = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
        await httpResponse.WriteStringAsync(json);
        
        return httpResponse;
    }

    private static async Task<HttpResponseData> CreateInternalServerErrorResponseAsync(HttpRequestData req)
    {
        var httpResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        httpResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var errorResponse = new { error = "An internal server error occurred" };
        var json = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
        await httpResponse.WriteStringAsync(json);
        
        return httpResponse;
    }
}
