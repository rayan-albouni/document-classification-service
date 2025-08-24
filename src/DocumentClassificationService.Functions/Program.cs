using DocumentClassificationService.Application;
using DocumentClassificationService.Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddFunctionsWorkerDefaults();
builder.Services.AddApplicationInsightsTelemetryWorkerService();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

await builder.Build().RunAsync();
