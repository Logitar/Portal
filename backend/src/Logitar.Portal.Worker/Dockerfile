#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Logitar.Portal.Worker/Logitar.Portal.Worker.csproj", "src/Logitar.Portal.Worker/"]
RUN dotnet restore "./src/Logitar.Portal.Worker/Logitar.Portal.Worker.csproj"
COPY . .
WORKDIR "/src/src/Logitar.Portal.Worker"
RUN dotnet build "./Logitar.Portal.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Logitar.Portal.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logitar.Portal.Worker.dll"]