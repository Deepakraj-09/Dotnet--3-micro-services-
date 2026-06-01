# RoboShop Shipping Service — Java → .NET 8 Conversion

## Overview

The `shipping` microservice was originally written in **Java (Spring Boot 2.3)**
and has been fully converted to **ASP.NET Core / .NET 8 (C#)**.
All other services (cart, catalogue, user — Node.js) and the frontend (HTML/CSS/JS)
are unchanged.

---

## File-by-File Conversion Map

| Java file | .NET equivalent | Notes |
|---|---|---|
| `ShippingServiceApplication.java` | `Program.cs` | App entry-point, DI registration, middleware pipeline |
| `JpaConfig.java` | `Program.cs` (AddDbContext) | MySQL connection string from `DB_HOST` env var |
| `Controller.java` | `Controllers/ShippingController.cs` | `@RestController` → `[ApiController]` |
| `City.java` | `Models/City.cs` | `@Entity @Table` → EF Core `[Table]` |
| `Code.java` | `Models/Code.cs` | `@Entity @Table` → EF Core `[Table]` |
| `Ship.java` | `Models/Ship.cs` | Plain DTO — direct C# port |
| `CityRepository.java` | `Repositories/CityRepository.cs` | `CrudRepository` + JPQL → EF Core LINQ |
| `CodeRepository.java` | `Repositories/CodeRepository.cs` | `PagingAndSortingRepository` → EF Core + `OrderBy` |
| `Calculator.java` | `Services/Calculator.cs` | Haversine formula — direct port |
| `CartHelper.java` | `Services/CartHelper.cs` | Apache `HttpClient` → `System.Net.Http.HttpClient` |
| `RetryableDataSource.java` | `Program.cs` (EF retry) | `@Retryable` → EF Core `EnableRetryOnFailure` + Polly |
| `InstanaDatacenterTagInterceptor` | `Infrastructure/DatacenterMiddleware.cs` | `HandlerInterceptorAdapter` → ASP.NET Core middleware |
| `pom.xml` | `Shipping.csproj` | Maven → MSBuild/NuGet |
| `Dockerfile` | `Dockerfile` | `eclipse-temurin:17-jre` → `mcr.microsoft.com/dotnet/aspnet:8.0` |

---

## Dependency Mapping

| Java / Spring | .NET 8 |
|---|---|
| Spring Boot | ASP.NET Core |
| Spring Data JPA + Hibernate | Entity Framework Core 8 + Pomelo MySQL |
| Spring Retry `@Retryable` | Polly (EF retry + `IHttpClientFactory` policy) |
| Apache HttpClient | `System.Net.Http.HttpClient` (IHttpClientFactory) |
| SLF4J + Logback | Serilog |
| Maven (`pom.xml`) | MSBuild (`Shipping.csproj`) |
| `javax.servlet` | `Microsoft.AspNetCore.Http` |
| `@SpringBootApplication` | `WebApplication.CreateBuilder` + `app.Run()` |
| `@RestController` / `@GetMapping` | `[ApiController]` / `[HttpGet]` |
| `@Autowired` | Constructor injection (standard .NET DI) |
| `HandlerInterceptorAdapter` | `IMiddleware` / `RequestDelegate` |
| Instana SDK `SpanSupport.annotate` | `System.Diagnostics.Activity.SetTag` (OTel compatible) |

---

## Building

```bash
cd Shipping
dotnet restore
dotnet build
dotnet run
```

## Docker

```bash
docker build -t roboshop/shipping-dotnet .
docker run -e DB_HOST=mysql -e CART_ENDPOINT=cart -p 8080:8080 roboshop/shipping-dotnet
```

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `DB_HOST` | `mysql` | MySQL hostname (same as original Java service) |
| `CART_ENDPOINT` | `cart` | Cart service hostname |
| `ASPNETCORE_URLS` | `http://+:8080` | Listening address |

## API Endpoints (unchanged)

| Method | Path | Description |
|---|---|---|
| GET | `/health` | Health check |
| GET | `/count` | City count |
| GET | `/codes` | All country codes |
| GET | `/cities/{code}` | Cities for a country |
| GET | `/match/{code}/{text}` | Autocomplete city search |
| GET | `/calc/{id}` | Calculate shipping cost |
| POST | `/confirm/{id}` | Add shipping to cart |
| GET | `/memory` | Allocate 25MB (chaos) |
| GET | `/free` | Free held memory (chaos) |
