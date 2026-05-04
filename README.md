# Telemetry Ingestion Core

Small vertical slice of a telemetry pipeline using SQLite + .NET 8 + GitHub Actions (and potentially a React TypeScript frontend)

- **Filename**: README.md
- **Author**: Willa Charlotte Lyle
- **Date**: 29 April 2026
- **Stack**: .NET 8, ASP.NET Core, EF Core (SQLite), xUnit, GitHub Actions

## Requirements

- .Net 8 sdk and runtime
- ASP.Net 8 runtime and targeting pack
- dotnet cli tool
- dotnet-ef cli tool
- sqlite

## Features

- Single-reading telemetry ingestion at `POST /api/telemetry`
  - Duplicate prevention when externalId is repeated for the same tenant
- A query endpoint with filters and pagination at `GET /api/telemetry`
  - Contains a computed `batteryLow` status object
- Structured logging
- Simple health checks at `GET /health/live` and `GET /health/ready`
- Configurable default pagination values and battery low threshold
- CI/CD pipeline that builds project, runs tests and publishes test results

## Installation

Clone the repo and install dependencies:

```bash
# clone from codeberg
git clone https://codeberg.org/wildflowersoup/telemetry-ingestion-core.git
# or clone from github
git clone https://github.com/soupsiedaisy/telemetry-ingestion-core.git
cd telemetry-ingestion-core
cd server/src/TelemetryIngestionCore.Api
dotnet restore
```

## Usage

Build, update the DB and run the app:

```bash
dotnet build --no-restore 
dotnet ef database update
dotnet run
```

Create a reading:

```bash
curl -X 'POST' \
  'http://localhost:5555/api/telemetry' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "tenantId": "tenant-1",
  "externalId": "ext-1234",
  "deviceId": "device-1",
  "type": "level",
  "value": 5,
  "unit": "m",
  "battery": 50,
  "signal": -100,
  "recordedAt": "2026-04-29T16:48:03.231Z"
}'
```

Query readings with default pagination:

```bash
curl -X 'GET' \
  'http://localhost:5555/api/telemetry' \
  -H 'accept: application/json'
```

Query with custom page size and page number:

```bash
curl -X 'GET' \
  'http://localhost:5555/api/telemetry?page=1&pageSize=50' \
  -H 'accept: application/json'
```

Query readings with filters:

```bash
curl -X 'GET' \
  'http://localhost:5555/api/telemetry?tenantId=tenant-1&deviceId=device-1&type=level&from=2026-04-28&to=2026-04-30' \
  -H 'accept: application/json'
```

## Tests

To run the project tests:

```bash
cd path/to/telemetry-ingestion-core
cd server/src/TelemetryIngestionCore.Tests/
dotnet restore
dotnet test
```

## Configuration (env / appsettings)

- CONNECTIONSTRINGS__TelemetryDb — connection string (default: Data Source=./wwwroot/content/data/TelemetryDb.db)
- AppOptions:
  - BatteryLowThreshold - battery low threshold (default: 20)
  - MaxPageSize - max page size for pagination (default: 500)
  - DefaultPageSize - default page size for pagination (default: 50)

## Solution Overview

See [SOLUTION](/SOLUTION.md) for full breakdown of requirements, architecture and considerations.

## License

See `LICENSE` file for details.
