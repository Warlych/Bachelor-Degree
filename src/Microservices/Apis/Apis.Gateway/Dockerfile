FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/Apis/Apis.Gateway/Apis.Gateway.csproj", "src/Microservices/Apis/Apis.Gateway/"]
COPY ["src/Microservices/Metrics/Metrics.Contracts/Metrics.Contracts.csproj", "src/Microservices/Metrics/Metrics.Contracts/"]
RUN dotnet restore "src/Microservices/Apis/Apis.Gateway/Apis.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Apis/Apis.Gateway"
RUN dotnet build "Apis.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Apis.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apis.Gateway.dll"]
