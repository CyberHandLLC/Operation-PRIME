# Operation Prime Consolidated Implementation Plan

This guide summarizes the core implementation approach for Operation Prime and removes duplicate guidance from existing documents. It clarifies abstractions and outlines a phased strategy for building the WinUI 3 desktop application using Clean Architecture, MVVM, EF Core with SQLCipher, and HTTP integration.

## 1. Review Findings
- **Duplication**: Multiple docs repeat the same status tables and phase descriptions. Consolidate milestones into this single plan and keep status trackers separate.
- **Missing Abstractions**: `INOIService` referenced in documentation is not defined. The HTTP token provider and workflow services exist only in outline form. These interfaces should be added under `Application/Interfaces`.
- **Inconsistencies**: `OperationPrimeDbContextFactory` constructor arguments don't match `OperationPrimeDbContext`. Adjust the factory to supply a single `IOptions<DatabaseOptions>` instance.

## 2. Clean Architecture Folder Layout
```
Domain/
  Entities/
  ValueObjects/
  Enums/
  Interfaces/
Application/
  Interfaces/
  Services/
  DTOs/
  Validators/
Infrastructure/
  Data/
    Context/
    Repositories/
    Migrations/
  Services/
  Options/
Presentation/
  ViewModels/
  Views/
  Services/
  Constants/
```
- Tests reside under `tests/` with `Application.Tests` and `Architecture.Tests` projects.

## 3. Shared Base Classes and Utilities
- **BaseViewModel** (`Presentation/ViewModels`): Inherits from `ObservableValidator` and contains common properties (`IsBusy`, `Title`).
- **BaseEntity** (`Domain/Entities`): Provides audit trail fields and `UpdateAuditFields` method.
- **Validation**: Start with data annotations in ViewModels; later migrate to FluentValidation in the Application layer.
- **Logging**: Inject `ILogger<T>` into all services, repositories, and ViewModels. Logging is now fully implemented and consistent across all core Application services, repositories, ViewModels, and infrastructure services. Use debug-level logs for workflow steps and information-level logs for important actions.

## 4. Database and Encryption Setup
- Use `OperationPrimeDbContext` with SQLCipher via `Microsoft.Data.Sqlite` connection string format.
- Configuration is provided by `DatabaseOptions` bound from `appsettings.json`.
- Register context in DI as scoped:
  ```csharp
  services.AddDbContext<OperationPrimeDbContext>((sp, options) =>
  {
      var db = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      options.UseSqlite($"{db.ConnectionString};Password={db.EncryptionKey}");
  });
  ```

## 5. Service and Interface Overview
- **Repositories** (`Domain/Interfaces` → `Infrastructure/Data/Repositories`)
  - `IIncidentRepository`, `IPreIncidentRepository`, `IMajorIncidentRepository`
- **Application Services** (`Application/Services`)
  - `IIncidentService`, `IPriorityService`, `IValidationService`, `IAuditService`
  - *New*: `INOIService`, `IPreIncidentWorkflowService`, `IMajorIncidentWorkflowService`, `ITokenProvider`
- **HTTP Wrapper**
  - `TokenProvider` and `NeuronsService` implementations use `HttpClient` with optional circuit breaker logic.

## 6. Debug Logging Strategy
Insert structured logs at key points using `ILogger<T>`:
- Repository methods log CRUD operations.
- Services log workflow stages (creation, update, validation).
- ViewModels log navigation and command execution.
- Use scopes for high-level operations:
  ```csharp
  using (_logger.BeginScope("Incident {IncidentId}", incident.Id))
  {
      _logger.LogDebug("Creating incident");
  }
  ```
Logs should be verbose in debug builds and minimal in release builds. Persist logs to `%APPDATA%/OperationPrime/logs` using the built-in logging providers.

## 7. Phased Execution Plan
### Phase 1 – Foundation
- Establish project structure and DI container.
- Implement BaseEntity, BaseViewModel, and enums.
- Configure `OperationPrimeDbContext` with SQLCipher.
- Add repository and service skeletons with logging.

### Phase 2 – Feature Build
- Implement incident workflows and DTOs.
- Flesh out `INOIService` and workflow services.
- Integrate Neurons HTTP calls via `TokenProvider` and `NeuronsService`.
- Build WinUI pages and ViewModels that consume application services.

## 8. Summary Checklist
- [x] Add missing interfaces (`INOIService`, workflow services).
- [x] Correct `OperationPrimeDbContextFactory` constructor usage.
- [x] Ensure all services, repositories, and ViewModels use `ILogger<T>` (logging is fully implemented and consistent across all core layers).
- [x] Implement DI registration for new services.
- [x] Migration files present in `/Migrations` (initial migration created).
- [x] Data persistence fully verified (EF Core migrations ran successfully, 2025-07-25).
- [ ] Expand unit tests and architecture checks in `tests/` *(deferred until tooling is available)*.
- [ ] Implement `PriorityMatrix` value object (still missing; currently handled as a private matrix in `PriorityService`).
- [ ] Scaffold wizard/navigation logic and views (e.g., `IncidentWizardView`, `IncidentDetailView`) for incident workflows.

This consolidated plan streamlines the documentation and clarifies next actions for continuing Operation Prime development.

