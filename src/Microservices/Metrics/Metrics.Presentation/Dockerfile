FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/Metrics/Metrics.Presentation/Metrics.Presentation.csproj", "src/Microservices/Metrics/Metrics.Presentation/"]
COPY ["src/Microservices/Metrics/Metrics.Application/Metrics.Application.csproj", "src/Microservices/Metrics/Metrics.Application/"]
COPY ["src/Microservices/Metrics/Metrics.Domain/Metrics.Domain.csproj", "src/Microservices/Metrics/Metrics.Domain/"]
COPY ["src/Abstractions/Abstractions.Domain/Abstractions.Domain.csproj", "src/Abstractions/Abstractions.Domain/"]
COPY ["src/Abstractions/Abstractions.Persistence/Abstractions.Persistence.csproj", "src/Abstractions/Abstractions.Persistence/"]
COPY ["src/Microservices/Movements/Movements.Contracts/Movements.Contracts.csproj", "src/Microservices/Movements/Movements.Contracts/"]
COPY ["src/Microservices/RailwaySections/RailwaySections.Contracts/RailwaySections.Contracts.csproj", "src/Microservices/RailwaySections/RailwaySections.Contracts/"]
COPY ["src/Microservices/Trains/Trains.Contracts/Trains.Contracts.csproj", "src/Microservices/Trains/Trains.Contracts/"]
COPY ["src/Microservices/Metrics/Metrics.Contracts/Metrics.Contracts.csproj", "src/Microservices/Metrics/Metrics.Contracts/"]
COPY ["src/Microservices/Metrics/Metrics.Persistence/Metrics.Persistence.csproj", "src/Microservices/Metrics/Metrics.Persistence/"]
RUN dotnet restore "src/Microservices/Metrics/Metrics.Presentation/Metrics.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Metrics/Metrics.Presentation"
RUN dotnet build "Metrics.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Metrics.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metrics.Presentation.dll"]
