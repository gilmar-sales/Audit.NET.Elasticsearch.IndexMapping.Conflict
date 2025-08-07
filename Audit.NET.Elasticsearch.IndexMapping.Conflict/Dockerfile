FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Audit.NET.Elasticsearch.IndexMapping.Conflict/Audit.NET.Elasticsearch.IndexMapping.Conflict.csproj", "Audit.NET.Elasticsearch.IndexMapping.Conflict/"]
RUN dotnet restore "Audit.NET.Elasticsearch.IndexMapping.Conflict/Audit.NET.Elasticsearch.IndexMapping.Conflict.csproj"
COPY . .
WORKDIR "/src/Audit.NET.Elasticsearch.IndexMapping.Conflict"
RUN dotnet build "./Audit.NET.Elasticsearch.IndexMapping.Conflict.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Audit.NET.Elasticsearch.IndexMapping.Conflict.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Audit.NET.Elasticsearch.IndexMapping.Conflict.dll"]
