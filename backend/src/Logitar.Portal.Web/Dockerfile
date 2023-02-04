#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Logitar.Portal.Web/Logitar.Portal.Web.csproj", "src/Logitar.Portal.Web/"]
RUN dotnet restore "src/Logitar.Portal.Web/Logitar.Portal.Web.csproj"
COPY . .
WORKDIR "/src/src/Logitar.Portal.Web"
RUN dotnet build "Logitar.Portal.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Logitar.Portal.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logitar.Portal.Web.dll"]