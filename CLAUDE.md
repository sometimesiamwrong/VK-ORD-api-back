# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **VK ORD API Wrapper** (AdLawyerApi) - a .NET 8.0 Web API that wraps and manages interactions with the VK ORD (Оператор Рекламных Данных - Advertising Data Operator) API. The application provides a structured interface for managing advertising contracts, creatives, counterparties, invoices, and statistics in the VK advertising ecosystem.

## Development Commands

### Build and Run
```bash
# Build the project
dotnet build

# Run the application (DO NOT run unless explicitly requested)
dotnet run --project src/WebApp/WebApp.csproj

# Build for production
dotnet publish -c Release -o ./publish

# Restore dependencies
dotnet restore
```

### Database Migrations
```bash
# Create a new migration (from Domain project)
dotnet ef migrations add <MigrationName> --project src/Domain/Domain.csproj --startup-project src/WebApp/WebApp.csproj

# Apply migrations to database
dotnet ef database update --project src/Domain/Domain.csproj --startup-project src/WebApp/WebApp.csproj

# Remove last migration
dotnet ef migrations remove --project src/Domain/Domain.csproj --startup-project src/WebApp/WebApp.csproj
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## Code Conventions (Cursor Rules)

**IMPORTANT**: Follow these rules from `.cursor/rules/rule.mdc`:

1. **Never use `#region` directives** - Code should be organized through proper file structure, not regions
2. **Avoid unnecessary try-catch blocks** - Global exception filters handle errors and validation. Only use try-catch when there's a specific reason
3. **Never run the application unless explicitly requested** - The user will ask when they want the app started
4. **Always run `dotnet build` after completing planned changes** - This ensures the code compiles correctly

## Architecture

### Project Structure

The solution follows a **Clean Architecture** pattern with two main projects:

- **Domain** (`src/Domain/`): Core business entities, data access, and VK ORD API contracts
  - `Data/`: EF Core DbContext and database configuration
  - `Entities/`: Domain entities (User, ApiCredential, VkOrdCounterparty, VkOrdContract, VkOrdCreative, etc.)
  - `VkOrdApi/`: Refit API client interfaces and DTOs for VK ORD integration
  - `BrokenRules/`: Domain validation and error handling
  - `Migrations/`: EF Core database migrations

- **WebApp** (`src/WebApp/`): ASP.NET Core Web API and application logic
  - `Controllers/`: API endpoints
  - `Services/`: Business logic implementation
  - `Repositories/`: Data access layer (Repository pattern)
  - `Handlers/`: MediatR command/query handlers
  - `Features/`: CQRS-style features with MediatR
  - `Middleware/`: Global exception handling and status code middleware
  - `Security/`: JWT token services and authentication
  - `Configuration/`: Strongly-typed configuration classes
  - `Behaviors/`: MediatR pipeline behaviors (VkOrdKeyBehavior for request enrichment)

### Key Architectural Patterns

1. **Repository Pattern**: All data access goes through repositories (`Repositories/Interfaces` and `Repositories/Implementations`)
2. **MediatR CQRS**: Commands and queries use MediatR with pipeline behaviors
3. **VK ORD Multi-tenancy**:
   - `VkOrdLogicalAccount` represents different VK ORD accounts
   - `ApiCredential` links users to logical accounts
   - All VK ORD entities inherit from `VkOrdBase` which includes `LogicalAccountId` for data isolation
4. **Refit HTTP Clients**: External APIs (VK ORD, DaData, OpenRouter) use Refit for type-safe HTTP calls
5. **Service Decoration**: Uses Scrutor for service decoration (e.g., caching decorators)

### Database

- **PostgreSQL** with Entity Framework Core
- Connection string in `appsettings.json` or environment-specific configs
- **Automatic script execution on startup**: SQL scripts in `Scripts/` folder are tracked in `DatabaseScript` table and executed once
- Main entities: Users, ApiCredentials, VkOrdCounterparties, VkOrdContracts, VkOrdCreatives, VkOrdMedia, VkOrdInvoices, VkOrdStatistics, FlowTemplates

### VK ORD Integration

The application wraps the VK ORD API with a simplified interface:

- **IVkOrdApiClient** (Refit): Low-level HTTP client for VK ORD API calls
- **IVkOrdService**: High-level service orchestrating VK ORD operations
- **IVkOrdDataService<TEntity, TResponse>**: Generic service for syncing VK ORD entities to local database
- **VkOrdKeyBehavior**: MediatR behavior that injects API credentials into requests based on JWT claims

Key VK ORD entities managed:
- Persons/Counterparties (контрагенты)
- Contracts (договоры)
- Creatives (рекламные креативы)
- Media (медиафайлы)
- Invoices (счета)
- Statistics (статистика)

### Authentication & Authorization

- **JWT Bearer authentication** with refresh tokens
- Configuration in `appsettings.json` under `JwtSettings`
- `ITokenService` handles token generation and validation
- `VkApiHeadersFilter`: Custom filter for VK ORD API key injection

### Caching

- **Dual caching strategy**: In-memory cache (MemoryCache) and optional Redis (StackExchange.Redis)
- `TimeoutDistributedCache`: Decorator that adds timeout protection to cache operations
- Cache configuration in `appsettings.json` under `Redis`

### External Services

1. **DaData API** (`IDaDataApiClient`): Russian company/address suggestions and validation
2. **OpenRouter API** (`IOpenRouterApiClient`): AI completions (Grok model by default)
3. **VK ORD API** (`IVkOrdApiClient`): Main integration point for advertising data

### Middleware Pipeline Order

1. Swagger (development)
2. CORS (`AllowFrontend` policy)
3. GlobalExceptionHandlingMiddleware (catches all exceptions, returns structured responses)
4. ResultStatusCodeMiddleware (translates domain results to HTTP status codes)
5. Authentication
6. Authorization
7. Controllers

### Flow Templates

`FlowTemplate` entity stores reusable workflows for VK ORD operations:
- Types: Wizard, Counterparty, Contract, Creative, Media, Invoice
- Each template has headers (JSON) and is linked to an `ApiCredential`
- Services: `IFlowTemplateService`, `IWizardFlowTemplateService`

### Global Exception Handling

- `GlobalExceptionHandlingMiddleware` catches all exceptions
- Returns structured error responses via `BrokenRuleResponse`
- No need for try-catch in most handlers/controllers

## Configuration

Configuration files are environment-specific:
- `appsettings.json` - Base configuration with production defaults
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

Key sections:
- `ConnectionStrings:DefaultConnection` - PostgreSQL connection
- `VkOrd` - VK ORD API settings (token, retry policy, concurrency limits)
- `JwtSettings` - JWT authentication settings
- `Redis` - Redis cache settings (optional)
- `DaDataSettings` - DaData API configuration
- `OpenRouterSettings` - AI API configuration
- `Serilog` - Logging configuration (console + file)

## Deployment

The project uses GitLab CI/CD for deployment:
- Pipeline defined in `.gitlab-ci.yml`
- Deploys to production server on `main` branch pushes
- Steps: SSH connection → git pull → dotnet publish → service restart
- Service scripts in `shs/` directory

## Important Notes

1. **Never start the application unless user explicitly asks** - This is a strict requirement
2. **Always run `dotnet build` after completing changes** - Ensures compilation succeeds
3. **Avoid regions and unnecessary try-catch** - Follow cursor rules for clean code
4. **Multi-tenant VK ORD data**: Always consider `LogicalAccountId` when querying VK ORD entities
5. **API Credential scoping**: VK ORD operations are scoped to `ApiCredential` which determines the logical account and API token
6. **Database scripts run on startup**: New SQL scripts are auto-executed from output directory
7/ **Text on task done**: minimal compressed text on the main points
