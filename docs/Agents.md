# Project Context and State

## Repository Overview
- **Name**: HackerNewsAggregator
- **Structure**: .NET 10 backend Web API (`backend/`) plus Angular frontend (`frontend/`). Solution file `HackerNewsAggregator.slnx` includes backend, unit tests, and integration tests.
- **Status**: Active development on `main`. Backend and frontend are both scaffolded and customized.

## Current Branching and Git Status
- **Branch**: `main` (remote `origin/main` exists).
- **Remote**: configured; local branch tracks `origin/main`.
- **Working tree**: may be dirty depending on in-flight changes.

## Ignore List Baseline
- Root `.gitignore` covers .NET artifacts and IDE files.
- `frontend/.gitignore` covers Angular output, cache, `node_modules`, and Playwright `test-results/`.

## Backend API Design
- **Controller**: `backend/Controllers/HackerNewsController.cs` exposes `GET /api/hackernews/newest?count=20`.
- **Services**:
  - `IHackerNewsClient`/`HackerNewsClient` for typed HTTP calls to the HN API.
  - `IHackerNewsService`/`HackerNewsService` for cached newest stories.
- **Models**: `HackerNewsItem` maps HN API JSON; `HackerNewsStory` is the API response model.
- **Caching**: `HackerNewsOptions` configured via `appsettings.json` and `appsettings.Development.json`.
- **OpenAPI & Scalar**: OpenAPI is mapped in Development; Scalar UI available at `/scalar`.
- **CORS**: Allowed origin `http://localhost:4200` for frontend dev.

## Frontend Application
- **Angular**: standalone app under `frontend/`, with a single route rendering `NewestStoriesPage`.
- **Page features**: search input, paginated list, loading/error states, hacker-themed styling.
- **API calls**: `HackerNewsApiService` fetches `GET /api/hackernews/newest?count=100` and client-side filters/paginates.
- **Proxy**: `frontend/proxy.conf.json` proxies `/api` to `http://localhost:5129` for local dev.

## Tests
- **Backend unit tests**: `backend.Tests/` includes service and client tests (xUnit + Moq).
- **Backend integration tests**: `backend.IntegrationTests/` uses real HTTP against a running API (defaults to `http://localhost:5129`, override with `HN_BACKEND_BASE_URL`).
- **Frontend unit tests**: Angular tests in `frontend/src/app/*.spec.ts` (uses HTTP testing utilities).
- **Frontend e2e**: Playwright tests in `frontend/e2e/` with `frontend/playwright.config.ts` (spins up backend and frontend).

## Running Locally
- **Backend**: `dotnet run --project backend` (Development enables OpenAPI + Scalar at `/scalar`).
- **Frontend**: `npm install`, then `npm start` in `frontend/`.
- **Proxy**: ensure backend on `http://localhost:5129` for the frontend proxy.

## Testing Notes
- **Dotnet dependency**: backend tests require `dotnet` on PATH.
- **Playwright**: browsers installed with `npx playwright install`. OS deps may be required (`npx playwright install-deps`).
- **Integration tests**: require the backend running and internet access to Hacker News.

## Future Agent Guidance
1. Confirm `dotnet` and Node versions (Angular 21 requires Node v20.19+ or v22.12+).
2. Run backend unit/integration tests and frontend unit/e2e tests before major changes.
3. Keep the frontend proxy and backend CORS settings aligned for local dev.
4. Update `docs/Agents.md` after structural changes or new test infrastructure.
