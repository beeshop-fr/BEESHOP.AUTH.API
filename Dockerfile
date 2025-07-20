# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ./BEESHOP.AUTH.API.sln
RUN dotnet publish ./BEESHOP.AUTH.API/BEESHOP.AUTH.API.csproj -c Release -o /app/publish

# Étape de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BEESHOP.AUTH.API.dll"]