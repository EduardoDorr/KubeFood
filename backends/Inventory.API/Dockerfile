FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /https

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Inventory.API/KubeFood.Inventory.API.csproj", "Inventory.API/"]
COPY ["./Core/KubeFood.Core.csproj", "Core/"]
RUN dotnet restore "./Inventory.API/KubeFood.Inventory.API.csproj"
COPY . .
WORKDIR "/src/Inventory.API"
RUN dotnet build "./KubeFood.Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KubeFood.Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./Certificates/aspnetapp.pfx /https/aspnetapp.pfx
RUN chmod 644 /https/aspnetapp.pfx
ENTRYPOINT ["dotnet", "KubeFood.Inventory.API.dll"]