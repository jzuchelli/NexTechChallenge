# HackerNewsAggregator

## High-Level Architecture
- **Solution layout**: `HackerNewsAggregator.slnx` ties together a backend ASP.NET Core Web API (`backend/`) targeting .NET 10 and an Angular-based frontend (`frontend/`).
- **Backend**: `backend/` is a minimal Web API scaffold that currently exposes the standard template controllers and OpenAPI support. It is meant to aggregate Hacker News data through future endpoints and serves JSON payloads to the frontend.
- **Frontend**: `frontend/` will host the Angular SPA. At minimum it needs `package.json`/CLI configuration to interact with the backend APIs and present a curated Hacker News experience. Keep UI logic segregated here so it can ship independently of the API.

## Running the Solution Locally
1. **Prerequisites**
   - [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (matches `net10.0` in `backend/HackerNewsAggregator.csproj`).
   - [Node.js (18+)](https://nodejs.org/) with npm or an alternative package manager (yarn/pnpm) for the Angular app.

2. **Backend**
   ```bash
   cd backend
   dotnet restore
   dotnet build
   dotnet run
   ```
   The API listens on the ports defined by `launchSettings.json` (default HTTP/HTTPS). Confirm it by visiting the automatically generated Swagger UI when running in Development mode.

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

## Supporting Information
- `.gitignore` already filters .NET artifacts, IDE folders, and Angular/node build outputs to keep commits clean.
- `docs/Agents.md` records the current repository state and guidance for future agents. Always consult it before making large changes.
- Since no git remote exists yet, initialize one when you are ready (`git remote add origin ...`), push `main`, then configure the remote’s default branch accordingly.

## Suggestions for Successors
- Run `dotnet run` and `npm run start` together to smoke-test the interaction surface before adding features.
- Avoid committing generated `bin/`, `obj/`, `.angular/cache`, or `node_modules` directories; the `.gitignore` already covers them.
- If new services (e.g., crawlers or background workers) are required later, keep their configuration under dedicated folders and document registration in `docs/Agents.md` or a new `docs/README`.
