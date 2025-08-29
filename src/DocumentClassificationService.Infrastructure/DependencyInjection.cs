using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Messaging.ServiceBus;
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
                .Get<DocumentIntelligenceSettings>();
            
            if (settings == null)
                throw new InvalidOperationException("DocumentIntelligence configuration section is missing");
            
            if (string.IsNullOrWhiteSpace(settings.Endpoint))
                throw new InvalidOperationException("DocumentIntelligence:Endpoint is missing or empty");
            
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
                throw new InvalidOperationException("DocumentIntelligence:ApiKey is missing or empty");
            
            if (string.IsNullOrWhiteSpace(settings.ClassifierModelId))
                throw new InvalidOperationException("DocumentIntelligence:ClassifierModelId is missing or empty");

            return new DocumentAnalysisClient(
                new Uri(settings.Endpoint),
                new AzureKeyCredential(settings.ApiKey));
        });

        services.AddSingleton<ServiceBusClient>(provider =>
        {
            var connectionString = configuration["ServiceBusConnection"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ServiceBusConnection connection string is missing");
            
            return new ServiceBusClient(connectionString);
        });

        services.AddScoped<IDocumentClassificationService, AzureDocumentIntelligenceService>();
        services.AddScoped<IMessageBusService, AzureServiceBusService>();

        return services;
    }
}
