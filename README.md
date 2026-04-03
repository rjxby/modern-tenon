# ModernTenon API

ModernTenon is a small products API built with ASP.NET Core minimal APIs, Entity Framework Core, and SQLite. The current codebase targets the latest project upgrades in this repository: `.NET 10` and `EF Core 10`.

## What Is In The Repo

- `Backend/Api/Host`: HTTP host, endpoint mapping, Swagger, and app configuration.
- `Backend/Api/Services`: application service layer and mapping between API/domain models.
- `Backend/Api/Repositories`: EF Core data access, SQLite context, and migrations.
- `Backend/Tests`: unit tests and integration tests.

## Current Stack

- .NET 10
- ASP.NET Core minimal APIs
- Entity Framework Core 10
- SQLite
- Swagger / Swashbuckle
- xUnit, FluentAssertions, Moq, Bogus

Central package versions are managed in [`Backend/Directory.Packages.props`](/Backend/Directory.Packages.props).

## API Surface

The API exposes product endpoints under `/api/products`.

- `GET /api/products?page=1&limit=10`: list products with pagination metadata.
- `GET /api/products/{id}`: fetch a single product.
- `POST /api/products`: create a product.
- `PUT /api/products/{id}`: update a product.

### Request Models

`POST /api/products`

```json
{
  "name": "Keyboard"
}
```

`PUT /api/products/{id}`

```json
{
  "name": "Mechanical Keyboard",
  "price": 149.99
}
```

Validation currently requires:

- `name` to be present and at least 3 characters long.
- `price` on update to be present and greater than `0`.

## Local Development

### Prerequisites

- .NET 10 SDK
- `dotnet-ef` CLI tool if you want to apply or create migrations through the `Makefile`

### Run The API

From the repository root:

```bash
dotnet restore Backend/ModernTenon.sln
dotnet run --project Backend/Api/Host
```

The API applies pending EF Core migrations automatically during startup. For local `dotnet run`, the default SQLite file is configured in [`Backend/Api/Host/appsettings.json`](/Backend/Api/Host/appsettings.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Backend/Api/Host/data/modern-tenon.db"
  }
}
```

This is now explicit: the configured path is used as-is. Override it in any environment with `ConnectionStrings__DefaultConnection`.

Default local URLs from [`launchSettings.json`](/Backend/Api/Host/Properties/launchSettings.json):

- `http://localhost:5108`
- `https://localhost:7001`

Swagger UI is enabled in Development at:

- `https://localhost:7001/swagger`
- `http://localhost:5108/swagger`

## Makefile Shortcuts

The repository includes a [`Makefile`](/Makefile) with common EF Core commands:

```bash
make add-migration name=YourMigrationName
make remove-migration
make apply-migrations
```

For a one-command container run with persisted SQLite storage:

```bash
make dev-run
```

That builds the image with Podman and runs it with a named volume mounted at `/data`. The image sets `ConnectionStrings__DefaultConnection=Data Source=/data/modern-tenon.db`, so the container owns the DB path and applies migrations on startup. The same contract works with Docker:

```bash
docker build -t modern-tenon-api-dev -f Backend/Api/Host/Dockerfile ./Backend
docker run --mount type=volume,src=moderntenon-data,dst=/data -p 8080:8080 modern-tenon-api-dev
```

## Tests

Run all tests:

```bash
dotnet test Backend/ModernTenon.sln
```

The integration test suite boots the API through `WebApplicationFactory` and replaces the configured database with an in-memory SQLite database.

For a quick end-to-end smoke test against the real HTTP server:

```bash
make smoke-test
```

To target a specific host:

```bash
make smoke-test HOST=http://127.0.0.1:8080
```

`make smoke-test` assumes your API is already running and only sends HTTP requests. It does not start the app or touch the database.

That script will:

- call `GET /api/products`
- create a product with `POST /api/products`
- fetch it with `GET /api/products/{id}`
- update it with `PUT /api/products/{id}`
