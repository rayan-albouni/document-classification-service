FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DocumentClassificationService.Functions/DocumentClassificationService.Functions.csproj", "src/DocumentClassificationService.Functions/"]
COPY ["src/DocumentClassificationService.Application/DocumentClassificationService.Application.csproj", "src/DocumentClassificationService.Application/"]
COPY ["src/DocumentClassificationService.Domain/DocumentClassificationService.Domain.csproj", "src/DocumentClassificationService.Domain/"]
COPY ["src/DocumentClassificationService.Infrastructure/DocumentClassificationService.Infrastructure.csproj", "src/DocumentClassificationService.Infrastructure/"]
RUN dotnet restore "src/DocumentClassificationService.Functions/DocumentClassificationService.Functions.csproj"
COPY . .
WORKDIR "/src/src/DocumentClassificationService.Functions"
RUN dotnet build "DocumentClassificationService.Functions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentClassificationService.Functions.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
