# CLAUDE.md

## Project

Lidarr is a music library manager (.NET 8 backend, React/TypeScript frontend). It targets Windows, Linux, macOS, and FreeBSD. The canonical upstream is [github.com/Lidarr/Lidarr](https://github.com/Lidarr/Lidarr). PRs target the `develop` branch, never `master`.

---

## Building

```bash
# Backend (Posix/Linux)
dotnet msbuild -restore src/Lidarr.sln -p:Configuration=Debug -p:Platform=Posix -t:PublishAllRids

# Frontend
yarn install
yarn start   # webpack dev watch
```

---

## Running Tests

Backend tests require a prior build (output goes to `_tests/`):

```bash
./test.sh Linux Unit Test          # unit tests
./test.sh Linux Integration Test   # integration tests

# Or directly against already-built assemblies:
dotnet test _tests/*.dll --filter:Category!=ManualTest
```

Frontend has no test suite — only linting:

```bash
yarn lint --fix
yarn stylelint-linux --fix
```

---

## Guardrails

**Do not lightly edit code you cannot run tests against.**

Before changing any non-trivial backend logic:
1. Locate the matching test class in the corresponding `*.Test/` project.
2. Run that test class to confirm it passes before your change.
3. Run it again after. Add or update tests if behavior changes.
4. 80% coverage is required on new code in PRs.

Frontend / GUI changes are currently exempt (no automated test suite).

---

## Architecture

### Backend (`src/`)

| Project | Purpose |
|---|---|
| `NzbDrone.Core/` | All business logic — music, indexers, downloads, notifications, health |
| `Lidarr.Api.V1/` | Thin REST controllers; delegates to Core services |
| `NzbDrone.Common/` | Shared utilities — HTTP, disk, environment, extensions |
| `Lidarr.Http/` | ASP.NET Core hosting, middleware, auth |
| `NzbDrone.*.Test/` | NUnit test projects mirroring the above |

**Patterns:**

- **Repository**: `BasicRepository<T>` base class; feature repos in `NzbDrone.Core/<Feature>/Repositories/`
- **Service**: `IFooService` / `FooService` pairs own business logic; DI via constructor
- **Event-driven messaging**: `IEvent` + `IHandle<T>` for loose coupling; `ICommand` + `IExecute<T>` for commands; pub/sub via `IEventAggregator`
- **ORM**: Dapper with custom `SqlBuilder`; supports SQLite (default) and PostgreSQL
- **Migrations**: FluentMigrator in `NzbDrone.Core/Datastore/Migration/` — every schema change needs a migration file
- **DI container**: DryIoc; convention-based registration; no service locator

### Frontend (`frontend/src/`)

- React 18, Redux (thunks), React Router v5
- Feature-based directories: `Artist/`, `Album/`, `Track/`, `Activity/`, `Settings/`, etc.
- CSS Modules + PostCSS
- Localization: `translate()` from `Utilities/String/translate`; English keys in `src/NzbDrone.Core/Localization/en.json`

---

## Code Style

### C# (enforced by `.editorconfig` and StyleCop)

- 4-space indent, UTF-8, no trailing whitespace, final newline
- `var` everywhere — explicit types are a build error (IDE0007)
- Instance fields: `_camelCase` (underscore prefix required, SX1309=warning)
- No `this.` qualification
- Inline `out` variable declarations
- `System.*` usings first; remove unused usings (IDE0005=error)
- No XML doc comments on internal code

### TypeScript / CSS

- 2-space indent
- Prettier handles formatting — always run `yarn lint --fix` before committing frontend changes
- Run `yarn stylelint-linux --fix` for CSS changes

### Comments

Only comment when the **why** is non-obvious — a hidden constraint, a non-obvious invariant, or a workaround for a specific external bug. Never describe what the code does. No docstrings on internal code. No emojis.

---

## Adding Features

- **New UI strings**: add a key to `src/NzbDrone.Core/Localization/en.json` (English only; other languages via Weblate)
- **New DB columns/tables**: add a FluentMigrator migration in `NzbDrone.Core/Datastore/Migration/`
- **API changes**: `Lidarr.Api.V1/` — keep controllers thin, logic stays in Core
- **Commit prefixes**: `New:` for features, `Fixed:` for bugs (non-maintenance only)
