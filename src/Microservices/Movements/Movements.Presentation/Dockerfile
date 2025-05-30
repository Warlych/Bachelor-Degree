FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/Movements/Movements.Presentation/Movements.Presentation.csproj", "src/Microservices/Movements/Movements.Presentation/"]
COPY ["src/Microservices/Movements/Movements.Application/Movements.Application.csproj", "src/Microservices/Movements/Movements.Application/"]
COPY ["src/Abstractions/Abstractions.Persistence/Abstractions.Persistence.csproj", "src/Abstractions/Abstractions.Persistence/"]
COPY ["src/Abstractions/Abstractions.Domain/Abstractions.Domain.csproj", "src/Abstractions/Abstractions.Domain/"]
COPY ["src/Microservices/Movements/Movements.Domain/Movements.Domain.csproj", "src/Microservices/Movements/Movements.Domain/"]
COPY ["src/Microservices/Movements/Movements.Contracts/Movements.Contracts.csproj", "src/Microservices/Movements/Movements.Contracts/"]
COPY ["src/Microservices/Movements/Movements.Persistence/Movements.Persistence.csproj", "src/Microservices/Movements/Movements.Persistence/"]
COPY ["src/Messages/Messages.Broker/Messages.Broker.csproj", "src/Messages/Messages.Broker/"]
RUN dotnet restore "src/Microservices/Movements/Movements.Presentation/Movements.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Movements/Movements.Presentation"
RUN dotnet build "Movements.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Movements.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Movements.Presentation.dll"]
