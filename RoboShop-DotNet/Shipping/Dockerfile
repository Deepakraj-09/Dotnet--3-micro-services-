# ─── Build stage ──────────────────────────────────────────────────────────────
# Replaces: mvn package  (Java pom.xml build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Shipping.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# ─── Runtime stage ────────────────────────────────────────────────────────────
# Replaces: FROM eclipse-temurin:17-jre-jammy  (Java runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Expose port 8080 (ASP.NET Core default; matches Java server.port=8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Shipping.dll"]
