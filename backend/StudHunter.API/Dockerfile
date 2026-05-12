# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

USER root
RUN apt-get update && apt-get install -y fontconfig && rm -rf /var/lib/apt/lists/*
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StudHunter.API/StudHunter.API.csproj", "StudHunter.API/"]
COPY ["StudHunter.DB.Postgres/StudHunter.DB.Postgres.csproj", "StudHunter.DB.Postgres/"]
RUN dotnet restore "./StudHunter.API/StudHunter.API.csproj"
COPY . .
WORKDIR "/src/StudHunter.API"
RUN dotnet build "./StudHunter.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StudHunter.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/wwwroot/uploads/avatars
RUN chown -R $APP_UID:$APP_UID /app/wwwroot
USER $APP_UID

ENTRYPOINT ["dotnet", "StudHunter.API.dll"]