using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentClassificationService.Domain.Interfaces;
using DocumentClassificationService.Infrastructure.Configuration;
using DocumentClassificationService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentClassificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentIntelligenceSettings>(
            configuration.GetSection(DocumentIntelligenceSettings.SectionName));

        services.AddSingleton<DocumentAnalysisClient>(provider =>
        {
            var settings = configuration.GetSection(DocumentIntelligenceSettings.SectionName)
                .Get<DocumentIntelligenceSettings>() ?? throw new InvalidOperationException("DocumentIntelligence configuration is missing");

            return new DocumentAnalysisClient(
                new Uri(settings.Endpoint),
                new AzureKeyCredential(settings.ApiKey));
        });

        services.AddScoped<IDocumentClassificationService, AzureDocumentIntelligenceService>();

        return services;
    }
}
