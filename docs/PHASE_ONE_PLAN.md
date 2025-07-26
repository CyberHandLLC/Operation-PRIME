# OPERATION PRIME — Phase One Implementation Plan

**Purpose:** Build a robust, maintainable, and testable foundation for the NOC PRIME application using MVVM, WinUI 3, and best practices from project documentation.

---

## Table of Contents
1. [Clean Architecture Implementation Plan](#clean-architecture-implementation-plan)
2. [Four Core Layers Setup](#four-core-layers-setup)
3. [Dependency Injection & Service Registration](#dependency-injection--service-registration)
4. [Configuration & Options Pattern](#configuration--options-pattern)
5. [Testing & Architecture Validation](#testing--architecture-validation)
6. [Logging & Error Handling](#logging--error-handling)
7. [Web Request Wrapper Service](#web-request-wrapper-service)
8. [Validation & DTOs](#validation--dtos)
9. [Implementation Roadmap](#implementation-roadmap)
10. [Step-by-Step Checklist](#step-by-step-checklist)

---

## 1. Documentation-Driven Requirements
- **Architecture:** Strict MVVM, DI, circuit breaker for integrations
- **Coding:** SOLID, clean code, explicit validation, testable, component isolation
- **UI:** WinUI 3, NavigationView, wizard dialogs, responsive & accessible
- **Workflows:** Unified incident wizard with conditional fields, .docx NOI templates, team-based autocomplete
- **Data:** EF Core with SQLCipher, offline-first, audit logging, priority matrix, validation rules

## 2. Solution Structure
```
OperationPrime/
├── Models/
│   ├── Incident.cs
│   ├── PreIncident.cs
│   ├── MajorIncident.cs
│   └── Enums.cs
├── ViewModels/
│   ├── BaseViewModel.cs
│   ├── IncidentViewModel.cs
│   ├── PreIncidentViewModel.cs
│   └── MajorIncidentViewModel.cs
├── Views/
│   ├── DashboardView.xaml
│   ├── IncidentWizardView.xaml
│   └── IncidentDetailView.xaml
├── Services/
│   ├── PriorityService.cs
│   ├── NOIService.cs
│   ├── AuditService.cs
│   └── NeuronsService.cs
├── Data/
│   └── AppDbContext.cs
├── Utils/
│   └── Helpers.cs
├── App.xaml
├── App.xaml.cs
└── ...
```

---

## 3. Implementation Roadmap

### A. MVVM Foundation
1. Create `BaseViewModel` implementing `INotifyPropertyChanged`
2. Set up dependency injection (DI) container
3. Establish navigation (NavigationView, content host)
4. Scaffold MainWindow with navigation and status bar

### B. Core Data Models
1. Implement enums: `IncidentStatus`, `Priority`, `Urgency`, `IncidentSource`
2. Create `Incident` base class (per REFERENCE.md)
3. Create `PreIncident` and `MajorIncident` classes (inherit from Incident)
4. Add validation attributes and logic

### C. DbContext & Persistence
1. Implement `AppDbContext` with EF Core and SQLCipher encryption
2. Add `DbSet<>` for all models
3. Set up initial migration

### D. ViewModels & Views
1. Create `IncidentViewModel` (observable properties, commands)
2. Create `PreIncidentViewModel` and `MajorIncidentViewModel`
3. Scaffold `DashboardView`, `IncidentWizardView`, `IncidentDetailView` (basic XAML)

### E. Services
1. Implement skeletons for `PriorityService`, `NOIService`, `AuditService`, `NeuronsService`
2. Register all services with DI

### F. Workflows & UI
1. Implement unified incident creation wizard with conditional field logic
2. Add autocomplete behaviors for Applications, SME names, and Support teams
3. Implement .docx NOI template processing and auto-generation
4. Add team-based email distribution with manual override capability
5. Implement keyboard shortcuts, validation, and navigation logic

### G. Testing & Verification
1. Add unit tests for models, services, and validation logic (xUnit)
2. Document evidence of implementation (per Coding Standards)

---

## 4. Step-by-Step Checklist

### 0. Clean Architecture & Foundation
- [x] Create solution folders/projects for:
  - Domain (entities, value objects, business rules)
  - Application (use-cases, workflow logic, interfaces)
  - Infrastructure (EF Core, SQLCipher, repository/service implementations)
  - Presentation (WinUI 3 UI, ViewModels, XAML, Services, Utils)
- [x] Define repository/service interfaces in Domain/Application, implementations in Infrastructure
- [x] Set up DI/service registration for all interfaces with correct lifetimes
 - [x] Scaffold basic audit logging and error handling (IAuditService, ILogger<T> usage)
- [x] Enforce Clean Architecture, DI, and logging/audit/error handling patterns for all future development

### 1. MVVM Foundation
- [x] Create `BaseViewModel` in ViewModels/ inheriting from `ObservableValidator` (CommunityToolkit.Mvvm v8.4+)
- [x] Scaffold MainWindow with NavigationView and status bar
- [x] Create UI Framework Foundation (DashboardView, PlaceholderView)
- [x] Set up DI container and service registration
- [x] Implement NavigationService abstraction

### 2. Core Data Models (CURRENT FOCUS)
- [x] Create enums: `IncidentStatus`, `Priority`, `UrgencyLevel`, `ImpactLevel`, `IncidentType`
- [x] Create `BaseEntity` with audit trail properties
- [x] Implement `Incident` base class
- [x] Implement `PreIncident` and `MajorIncident` classes
- [x] Add validation attributes (use data annotations for validation)
- [x] Implement `PriorityMatrix` value object (replaces private matrix in `PriorityService`)

### 3. DbContext & Persistence
- [x] Implement `AppDbContext` with encryption
- [x] Add `DbSet<>` properties for all models
- [ ] Create and apply initial migration

### 4. ViewModels & Views
- [x] Implement `IncidentViewModel`, `PreIncidentViewModel`, `MajorIncidentViewModel` (inherit from `BaseViewModel : ObservableValidator`, use `[ObservableProperty]` partials)
- [x] Scaffold `DashboardView`, `IncidentWizardView`, `IncidentDetailView`
- [x] Create and apply initial migration (migration files exist in `/Migrations`)
- [x] Data persistence fully verified (EF Core migrations ran successfully, 2025-07-25)

### 4. ViewModels & Views
- [x] Implement `IncidentViewModel`, `PreIncidentViewModel`, `MajorIncidentViewModel` (inherit from `BaseViewModel : ObservableValidator`, use `[ObservableProperty]` partials)
- [x] Scaffold `DashboardView`
- [ ] Scaffold `IncidentWizardView`, `IncidentDetailView` (not yet implemented)

### 5. Services
- [x] Create skeletons for all services
- [x] Register services in DI (dependency injection setup is complete and covers all core layers)

### 6. Workflows & UI
- [x] Scaffold unified incident wizard with conditional fields
- [ ] Implement validation and navigation logic
- [ ] Scaffold wizard flows for Pre/Major Incidents (wizard/navigation logic not yet implemented)
- [ ] Implement validation and navigation logic (partial; some validation present, navigation logic for wizard missing)
- [ ] Add keyboard shortcuts

### 7. Testing & Verification
- [ ] Add unit tests for core logic *(deferred - environment lacks .NET 9 tooling)*
- [ ] Document implementation evidence *(pending test completion)*

---

## Modern MVVM & Validation Standards (2025+)
- All ViewModels must inherit from `BaseViewModel : ObservableValidator` (CommunityToolkit.Mvvm v8.4+)
- Use `[ObservableProperty]` partials for observable properties (supports protected set, required, null-annotations, etc.)
- Uses `Directory.Build.props` with `<LangVersion>preview</LangVersion>` for partial property support and future-proofing across all projects
- MVVM Toolkit analyzers must be enabled to enforce correct usage and surface errors/warnings
- Use `[RelayCommand]` and `IAsyncRelayCommand` for commands; use `IMessenger` for decoupled messaging between ViewModels
- Validation is handled via data annotations and toolkit support—no need to manually implement `INotifyDataErrorInfo`
- **Logging is now fully implemented and consistent across all core Application services, repositories, ViewModels, and infrastructure services.**

**Refer to this plan as you build out Phase One. Each step is designed to ensure maintainability, testability, and alignment with project standards and requirements.**
