# --- Build (solution uniquement) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# On copie tout
COPY . .

# Restore/Build/Publish via la solution
RUN dotnet restore ./BEESHOP.AUTH.API.sln
RUN dotnet build   ./BEESHOP.AUTH.API.sln -c Release --no-restore
RUN dotnet publish ./BEESHOP.AUTH.API.sln -c Release --no-build /p:UseAppHost=false

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV DOTNET_EnableDiagnostics=0

# On récupère UNIQUEMENT les binaires publiés de l'API à lancer
# (le publish de la solution met chaque projet dans son propre dossier publish)
COPY --from=build /src/BEESHOP.AUTH.API/bin/Release/net8.0/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BEESHOP.AUTH.API.dll"]
