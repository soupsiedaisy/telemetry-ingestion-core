# Telemetry Ingestion Core

### Description

A small vertical slice of a telemetry ingestion pipeline, using ASP.NET Core (.NET 8), SQLite via EF Core, Typescript React and GitHub Actions.

#### Author: Willa Charlotte Lyle

#### Email: <dev@wildflowersoup.dev>

## Summary

A simple, tightly scoped telemetry pipeline that:

- Accepts single telemetry readings into a backend service
- Persists them to a lightweight SQL store
- Exposes a simple query interface
- Builds and tests via a single CI pipeline
- (Optional) Builds backend service into a docker image
- (Optional) Displays readings via a frontend service

Example telemetry payload:

```json
{
  "tenantId": "acme",
  "deviceId": "dev-123",
  "type": "water_level",
  "value": 1.23,
  "unit": "m",
  "battery": 62,
  "signal": -85,
  "recordedAt": "2025-01-10T10:15:00Z",
  "externalId": "r-789"
}
```

## Requirements and Constraints

### Backend

- ASP.NET Core (.NET 8).
- Single-reading ingest only (one reading per POST request).
- Validation for required fields with sensible ranges.
- Duplicate prevention for when externalId is repeated for the same tenant.
- Filtering and pagination for queries.
- Structured logging.
- Simple health endpoint for basic readiness/liveness.
- One or two environment driven configurations.
- Simple domain rules.
- Computation of a status object (e.g. batteryLow when below configured threshold).
- Query endpoint with filtering by deviceId, type, and time range, plus pagination.
- Automated tests focused on core domain logic and at least one happypath API flow.
- Minimal APIs and controllers.
- Evolvable model with clear API shape without overengineering.
- (Optional) Correlation ID propagation if provided by the client.

### DB

- SQLite persistence of readings (using EF core).
- Reasonable schema and mapping.

### Frontend (Optional)
- Readonly table with recent readings (last N or fixed recent).
- Single loading and error states.
- Typed API models.
- Environment driven API base url.
- (Optional) Device and type filters.
- (Optional) Simple submit form to POST a single reading.

### CI/CD

- GitHub Actions.
- Builds and tests project.
- Published test results/artifacts.
- (Optional) Docker image for the backend.
- (Optional) Basic caching for package restores.

## Architecture

## Backend

### Project Layout

### Data Models

### API Layout

### Validation and Business Rules

### Tests

## CI/CD

## Frontend (Optional)

## Operational Readiness / Configuration

## Out of Scope Functionality / Future Considerations

## Delivery Checklist / TODOs

- [ ] Finish writing this SOLUTION.md document.
- [ ] Write README.md.
- [ ] Implement TelemetryIngestionCore.Api with controllers, services, DbContext, models and configuration.
- [ ] Add EF Core migrations and DB initialization.
- [ ] Add structured JSON logging and correlation ID propagation.
- [ ] Implement health endpoints.
- [ ] Implement unit and integration tests.
- [ ] Add GitHub Actions to build project and publish test results.
- [ ] (Optional) Add Docker compose file and integrate it into CI/CD.
- [ ] (Optional) Minimal React Typescript frontend.
