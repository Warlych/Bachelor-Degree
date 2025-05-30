FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/Trains/Trains.Presentation/Trains.Presentation.csproj", "src/Microservices/Trains/Trains.Presentation/"]
COPY ["src/Microservices/Trains/Trains.Application/Trains.Application.csproj", "src/Microservices/Trains/Trains.Application/"]
COPY ["src/Microservices/Trains/Trains.Domain/Trains.Domain.csproj", "src/Microservices/Trains/Trains.Domain/"]
COPY ["src/Abstractions/Abstractions.Domain/Abstractions.Domain.csproj", "src/Abstractions/Abstractions.Domain/"]
COPY ["src/Abstractions/Abstractions.Persistence/Abstractions.Persistence.csproj", "src/Abstractions/Abstractions.Persistence/"]
COPY ["src/Microservices/Trains/Trains.Contracts/Trains.Contracts.csproj", "src/Microservices/Trains/Trains.Contracts/"]
COPY ["src/Microservices/Trains/Trains.Persistence/Trains.Persistence.csproj", "src/Microservices/Trains/Trains.Persistence/"]
RUN dotnet restore "src/Microservices/Trains/Trains.Presentation/Trains.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Trains/Trains.Presentation"
RUN dotnet build "Trains.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Trains.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trains.Presentation.dll"]
