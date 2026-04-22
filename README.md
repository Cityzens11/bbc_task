# Currency Converter Platform

Full-stack currency conversion platform with:
- Backend API: ASP.NET Core (.NET 10)
- Frontend: React + TypeScript (Vite)
- External rate provider: Frankfurter API

## Architecture Overview

## Backend (Clean layered design)
- CurrencyConverter.Domain
  - Core models and business constraints
- CurrencyConverter.Application
  - Service abstractions and currency use-cases
- CurrencyConverter.Infrastructure
  - Frankfurter provider, provider factory, caching, retry/circuit breaker, HTTP correlation
- CurrencyConverter.Api
  - Controllers, middleware, auth, RBAC, rate limiting, versioning, observability

### Implemented API Endpoints (v1)
- `GET /api/v1/rates/latest?baseCurrency=EUR`
- `GET /api/v1/rates/convert?amount=100&fromCurrency=EUR&toCurrency=USD`
- `GET /api/v1/rates/historical?baseCurrency=EUR&startDate=2020-01-01&endDate=2020-01-31&page=1&pageSize=20`
- `POST /api/v1/auth/token` (dev/test token bootstrap)

### Business Rule
The conversion endpoint rejects TRY, PLN, THB, MXN for source/target with HTTP 400 and a clear message.

### Resilience and Performance
- In-memory caching for latest and historical rates
- Retry policy with exponential backoff (Polly)
- Circuit breaker for upstream failures (Polly)
- HttpClientFactory for managed outbound calls

### Provider Extensibility
- `IExchangeRateProvider` abstraction
- `IExchangeRateProviderFactory` to select provider from config
- `FrankfurterExchangeRateProvider` implemented with RestEase

### Security
- JWT Bearer authentication
- RBAC policies:
  - `RatesReaderPolicy`
  - `ConverterPolicy`
- API fixed-window rate limiting
- Configurable CORS policy

### Logging, Monitoring, Observability
- Structured logging with Serilog
- Per-request observability logs include:
  - Client IP
  - Client ID (JWT `client_id` claim)
  - HTTP method + endpoint
  - Response status code
  - Response time
  - Correlation ID
- Correlation ID is propagated to outbound Frankfurter API calls via `X-Correlation-ID`.


## Frontend
- React + TypeScript + React Query
- Main features:
  - Conversion form (amount, source, target)
  - Latest rates table by base currency
  - Historical rates view with date range + pagination
- UX quality:
  - Validation for excluded currencies
  - Loading states
  - Friendly API error messages
  - Responsive layout

## Project Structure
- `backend/src` - API and backend libraries
- `backend/tests` - unit and integration tests
- `frontend` - React application

## Setup Instructions

## Prerequisites
- .NET SDK 10.0.x
- Node.js 20+
- npm 10+

## Run Backend
1. From repository root:
   - `dotnet restore CurrencyConverterPlatform.slnx`
   - `dotnet build CurrencyConverterPlatform.slnx`
2. Run API:
   - `dotnet run --project backend/src/CurrencyConverter.Api/CurrencyConverter.Api.csproj`

By default the API uses `http://localhost:5050` (check launch profile output).

## Run Frontend
1. `cd frontend`
2. Install dependencies:
   - `npm install`
3. Start dev server:
   - `npm run dev`

## Testing

## Backend
- Run all tests with coverage collector:
  - `dotnet test CurrencyConverterPlatform.slnx`

## Frontend
- Run tests:
  - `cd frontend`
  - `npm run test`

## API Versioning
Routes are versioned with URL segment (`/api/v1/...`).

## Multi-environment Support
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Test.json`
- `appsettings.Production.json`
P.S some of the configurations can be moved to docker env config in future

Environment can be selected via `ASPNETCORE_ENVIRONMENT`.

## Horizontal Scalability Notes
- API is stateless
- Uses DI and provider abstraction
- Caching currently uses in-memory cache; can be replaced with distributed cache (Redis) for multi-instance deployments
- Rate limiting is currently in-process; can be moved to centralized gateway or distributed limiter for cluster-wide enforcement

## CI/CD Readiness
Suggested pipeline stages:
1. Restore
2. Build backend + frontend
3. Run backend tests with coverage
4. Run frontend tests
5. Publish artifacts
6. Containerize/deploy

Minimal checks recommended:
- `dotnet build`
- `dotnet test`
- `npm run build`
- `npm run test`

## AI Usage
AI tools were used to accelerate delivery and quality:
- Initial architecture decomposition (layering + provider abstraction)
- Middleware and API wiring boilerplate generation
- Test skeleton generation for unit/integration scenarios
- React feature scaffolding and typed API client setup

What was manually validated and adjusted:
- Enforced RestEase specifically for external provider calls
- Corrected middleware behavior to log authenticated `client_id` after auth flow
- Validated endpoint contracts, auth policies, and exclusion rules
- Fixed build/test issues (config typing, project references, package gaps)

What was not accepted blindly from AI:
- Generic template code and defaults were removed
- Security-sensitive values and policy decisions were reviewed manually
- Error handling and business rule wording were adjusted for clarity
- Build and tests were executed and corrected iteratively

## Assumptions and Trade-offs
- Added `POST /api/v1/auth/token` for local/demo token bootstrap
- In-memory cache was chosen for simplicity; production should use distributed cache
- Integration tests call real app pipeline; one test path reaches external provider
- Frontend auto-fetches a dev token for demo usability

## Potential Future Improvements
1. Replace in-memory caching and rate limit state with distributed infrastructure.
2. Add OpenTelemetry traces/metrics and dashboard integration.
3. Add stronger input validation with FluentValidation.
4. Add richer integration tests with mocked upstream server and outage simulations.
5. Add containerization (`Dockerfile`, `docker-compose`) and CI workflow files.
