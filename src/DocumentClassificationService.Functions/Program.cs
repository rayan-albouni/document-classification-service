using DocumentClassificationService.Application;
using DocumentClassificationService.Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Logging.AddApplicationInsights();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

await builder.Build().RunAsync();
