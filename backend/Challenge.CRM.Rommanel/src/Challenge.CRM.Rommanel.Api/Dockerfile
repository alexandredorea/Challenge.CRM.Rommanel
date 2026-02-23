FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
#EXPOSE 8081

# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia apenas os csproj primeiro (cache de restore)
COPY ["src/Challenge.CRM.Rommanel.Api/Challenge.CRM.Rommanel.Api.csproj", "src/Challenge.CRM.Rommanel.Api/"]
COPY ["src/Challenge.CRM.Rommanel.Application/Challenge.CRM.Rommanel.Application.csproj", "src/Challenge.CRM.Rommanel.Application/"]
COPY ["src/Challenge.CRM.Rommanel.Domain/Challenge.CRM.Rommanel.Domain.csproj", "src/Challenge.CRM.Rommanel.Domain/"]
COPY ["src/Challenge.CRM.Rommanel.Infrastructure/Challenge.CRM.Rommanel.Infrastructure.csproj", "src/Challenge.CRM.Rommanel.Infrastructure/"]

RUN dotnet restore "./src/Challenge.CRM.Rommanel.Api/Challenge.CRM.Rommanel.Api.csproj"

COPY . .
WORKDIR "/src/src/Challenge.CRM.Rommanel.Api"
RUN dotnet build "./Challenge.CRM.Rommanel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: publish e runtime
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Challenge.CRM.Rommanel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Challenge.CRM.Rommanel.Api.dll"]