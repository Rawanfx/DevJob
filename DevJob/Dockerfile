FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DevJob/DevJob.API.csproj", "DevJob/"]
COPY ["DevJob.Core/DevJob.Application.csproj", "DevJob.Core/"]
COPY ["DevJob.Domain/DevJob.Domain.csproj", "DevJob.Domain/"]
COPY ["DevJob.Infrastructure/DevJob.Infrastructure.csproj", "DevJob.Infrastructure/"]
RUN dotnet restore "./DevJob/DevJob.API.csproj"
COPY . .
WORKDIR "/src/DevJob"
RUN dotnet build "./DevJob.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DevJob.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevJob.API.dll"]