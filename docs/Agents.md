# Project Context and State

## Repository Overview
- **Name**: HackerNewsAggregator
- **Structure**: Mixed .NET backend and Angular frontend. Top-level solution file `HackerNewsAggregator.slnx`. Directories: `backend/` (.NET 10 Web API), `frontend/` (Angular).
- **Status**: Initial commit has yet to be created; working tree contains generated .NET/Angular scaffold plus editor metadata.

## Current Branching and Git Status
- **Branch**: `main` (renamed from `master` per latest request).
- **Remote**: none configured locally.
- **Staged Changes**: None yet; all generated files listed under `git status` remain untracked or modified.

## Ignore List Baseline
- `.gitignore` includes .NET artifacts (`bin/`, `obj/`, `TestResults/`, ReSharper caches, IDE files) and Angular/Node files (`node_modules/`, `dist/`, `.angular/cache`, log files, env files) to avoid clutter.

## Backend API Design
- **Controller**: `backend/Controllers/HackerNewsController.cs` exposes `GET /api/hackernews/newest?count=20`.
- **Services**: `backend/Services/` holds a typed `HttpClient` wrapper for Hacker News (`IHackerNewsClient`/`HackerNewsClient`) and a caching orchestration layer (`IHackerNewsService`/`HackerNewsService`) using `IMemoryCache`.
- **Models**: `HackerNewsItem` maps HN API JSON; `HackerNewsStory` is the trimmed response contract.
- **Caching**: `HackerNewsOptions` configured via `appsettings.json` to control TTLs.

## Tests
- **Unit tests**: `backend.Tests/` contains `HackerNewsServiceTests.cs` (xUnit + Moq) that validate caching behavior, filtering, and input validation.
- **Solution**: `HackerNewsAggregator.slnx` includes both the backend and test project.

## Future Agent Guidance
1. **Initialize commits**: create an initial commit capturing backend/front-end scaffolding once review is done.
2. **Explore directories**: inspect `backend/` and `frontend/` to understand current scaffolded code before adding features. Use `rg`/`fd` for targeted searches.
3. **Follow conventions**: respect naming/formatting used by .NET 10 and Angular CLI; avoid reformatting large generated files unnecessarily.
4. **Document changes**: record rationale for major changes in `docs/Agents.md` to help successors pick up the context quickly.
5. **Tests & builds**: run `dotnet build` from `backend/` and `npm run build` or equivalent from `frontend/` when modifying core functionality.
