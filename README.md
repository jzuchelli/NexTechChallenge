# HackerNewsAggregator

## High-Level Architecture
- **Solution layout**: `HackerNewsAggregator.slnx` ties together a backend ASP.NET Core Web API (`backend/`) targeting .NET 10 and an Angular-based frontend (`frontend/`).
- **Backend**: `backend/` is an ASP.NET Core Web API with a controller-based Hacker News newest-stories endpoint (`GET /api/hackernews/newest`). It uses a typed `HttpClient` and `IMemoryCache` for low-latency fetches from the Hacker News API.
- **Frontend**: `frontend/` will host the Angular SPA. At minimum it needs `package.json`/CLI configuration to interact with the backend APIs and present a curated Hacker News experience. Keep UI logic segregated here so it can ship independently of the API.

## Running the Solution Locally
1. **Prerequisites**
   - [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (matches `net10.0` in `backend/HackerNewsAggregator.csproj`).
   - [Node.js (20.19+ or 22.12+)](https://nodejs.org/) with npm or an alternative package manager (yarn/pnpm) for the Angular app.

2. **Backend**
   ```bash
   cd backend
   dotnet restore
   dotnet build
   dotnet run
   ```
   The API listens on the ports defined by `backend/Properties/launchSettings.json` (default HTTP/HTTPS). In Development you can view the OpenAPI JSON and the Scalar UI at `/scalar`.

3. **Frontend (Angular)**
   ```bash
   cd frontend
   npm install          # or yarn/pnpm if preferred
   npm run start        # adjust to your CLI (e.g., `ng serve`)
   ```
   Configure the Angular dev server to proxy API calls to the backend if you keep the default ports separated.

4. **Full-stack workflow**
   - Run backend and frontend concurrently. The frontend should consume backend endpoints via relative paths (through a proxy or shared base URL) so that you can develop both sides without repeated restarts.
   - Use the `.http` file in `backend/` for quick HTTP checks when Swagger is unavailable.

## Tests
```bash
dotnet test HackerNewsAggregator.slnx
```
Unit tests live under `backend.Tests/` and cover caching behavior and input validation for the Hacker News service.

## Known Limitations (Interview Context)
- **No server-side search**: the app fetches the newest stories and performs filtering/paging on the client, so search is limited to the fetched set (`count=100`).
- **Upstream dependency**: the backend depends on the public Hacker News API with no retry/backoff strategy; transient failures surface to the client.
- **Integration tests require internet**: `backend.IntegrationTests` exercise real HTTP against the Hacker News API and may fail offline.

## Supporting Information
- `.gitignore` already filters .NET artifacts, IDE folders, and Angular/node build outputs to keep commits clean.
- `docs/Agents.md` records the current repository state and guidance for future agents. Always consult it before making large changes.

## Suggestions for Successors
- Run `dotnet run` and `npm run start` together to smoke-test the interaction surface before adding features.
- Avoid committing generated `bin/`, `obj/`, `.angular/cache`, or `node_modules` directories; the `.gitignore` already covers them.
- If new services (e.g., crawlers or background workers) are required later, keep their configuration under dedicated folders and document registration in `docs/Agents.md` or a new `docs/README`.
