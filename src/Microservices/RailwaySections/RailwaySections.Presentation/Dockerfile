FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Microservices/RailwaySections/RailwaySections.Presentation/RailwaySections.Presentation.csproj", "src/Microservices/RailwaySections/RailwaySections.Presentation/"]
COPY ["src/Microservices/RailwaySections/RailwaySections.Domain/RailwaySections.Domain.csproj", "src/Microservices/RailwaySections/RailwaySections.Domain/"]
COPY ["src/Abstractions/Abstractions.Domain/Abstractions.Domain.csproj", "src/Abstractions/Abstractions.Domain/"]
COPY ["src/Microservices/RailwaySections/RailwaySections.Persistence/RailwaySections.Persistence.csproj", "src/Microservices/RailwaySections/RailwaySections.Persistence/"]
COPY ["src/Abstractions/Abstractions.Persistence/Abstractions.Persistence.csproj", "src/Abstractions/Abstractions.Persistence/"]
COPY ["src/Microservices/RailwaySections/RailwaySections.Application/RailwaySections.Application.csproj", "src/Microservices/RailwaySections/RailwaySections.Application/"]
COPY ["src/Microservices/RailwaySections/RailwaySections.Contracts/RailwaySections.Contracts.csproj", "src/Microservices/RailwaySections/RailwaySections.Contracts/"]
RUN dotnet restore "src/Microservices/RailwaySections/RailwaySections.Presentation/RailwaySections.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Microservices/RailwaySections/RailwaySections.Presentation"
RUN dotnet build "RailwaySections.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RailwaySections.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RailwaySections.Presentation.dll"]
