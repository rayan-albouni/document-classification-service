using DocumentClassificationService.Application.Handlers;
using DocumentClassificationService.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentClassificationService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClassifyDocumentHandler, ClassifyDocumentHandler>();
        services.AddScoped<IClassifyDocumentValidator, ClassifyDocumentValidator>();

        return services;
    }
}
