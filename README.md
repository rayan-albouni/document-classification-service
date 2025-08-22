# Document Classification Service

A high-performance Azure Functions-based microservice that classifies documents using Azure Document Intelligence. The service follows Domain-Driven Design (DDD) principles and implements SOLID design patterns.

## Architecture

The solution is structured using Clean Architecture with the following layers:

- **Domain**: Core business logic, entities, value objects, and interfaces
- **Application**: Use cases, commands, handlers, and DTOs
- **Infrastructure**: External service implementations (Azure Document Intelligence)
- **Functions**: Azure Functions API layer

## API Endpoints

### POST /api/v1/documents/{documentId}/classify

Classifies a document stored in Azure Blob Storage.

**Request:**
```json
{
  "blobUrl": "https://storage.blob.core.windows.net/container/document.pdf",
  "tenantId": "tenant-123"
}
```

**Response:**
```json
{
  "documentId": "doc_uuid_12345",
  "classification": "Invoice",
  "confidence": 0.99,
  "status": "CLASSIFIED",
  "classifierModelUsed": "custom-classifier-v1"
}
```

## Configuration

Update the `local.settings.json` file with your Azure services configuration:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=...",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "your-app-insights-key",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=your-key"
  },
  "DocumentIntelligence": {
    "Endpoint": "https://your-endpoint.cognitiveservices.azure.com/",
    "ApiKey": "your-api-key",
    "ClassifierModelId": "your-custom-model-id"
  }
}
```

## Prerequisites

- .NET 8.0 SDK
- Azure Functions Core Tools
- Azure Document Intelligence resource
- Azure Application Insights resource

## Running Locally

1. Clone the repository
2. Update `local.settings.json` with your Azure configuration
3. Run the following commands:

```bash
cd src
dotnet restore
cd DocumentClassificationService.Functions
func start
```

## Project Structure

```
src/
├── DocumentClassificationService.sln
├── DocumentClassificationService.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   └── Interfaces/
├── DocumentClassificationService.Application/
│   ├── Commands/
│   ├── DTOs/
│   ├── Handlers/
│   └── Validators/
├── DocumentClassificationService.Infrastructure/
│   ├── Services/
│   └── Configuration/
└── DocumentClassificationService.Functions/
    └── Functions/
```

## Features

- **Clean Architecture**: Follows DDD principles with clear separation of concerns
- **SOLID Principles**: Implements dependency inversion, single responsibility, and other SOLID principles
- **High Performance**: Optimized for Azure Functions with minimal cold start times
- **Comprehensive Logging**: Integrated with Application Insights for monitoring and diagnostics
- **Validation**: Input validation with detailed error responses
- **Error Handling**: Robust error handling with appropriate HTTP status codes

## Testing

The API can be tested using the following curl command:

```bash
curl -X POST "http://localhost:7071/api/v1/documents/test-doc-123/classify" \
  -H "Content-Type: application/json" \
  -d '{
    "blobUrl": "https://your-storage.blob.core.windows.net/documents/sample.pdf",
    "tenantId": "tenant-123"
  }'
```

## Deployment

The service can be deployed to Azure using:

1. Azure CLI
2. Visual Studio
3. GitHub Actions
4. Azure DevOps

Ensure to configure the application settings in Azure to match your `local.settings.json` values.
