# OPERATION PRIME Development Guide

**Keywords**: development phases, project setup, MVVM implementation, testing framework, deployment process, code quality standards, build configuration

## Table of Contents
1. [Clean Architecture Development Approach](#clean-architecture-development-approach)
2. [Four Core Layers Implementation](#four-core-layers-implementation)
3. [Dependency Injection Setup](#dependency-injection-setup)
4. [Configuration & Options Implementation](#configuration--options-implementation)
5. [Testing & Architecture Validation Setup](#testing--architecture-validation-setup)
6. [Logging & Error Handling Implementation](#logging--error-handling-implementation)
7. [Web Request Wrapper Implementation](#web-request-wrapper-implementation)
8. [Validation & DTOs Implementation](#validation--dtos-implementation)
9. [Development Phases Overview](#development-phases-overview)
10. [Phase Implementation Details](#phase-implementation-details)
11. [Build Configuration](#build-configuration)
12. [Deployment Process](#deployment-process)

---

## Clean Architecture Development Approach

OPERATION PRIME development strictly follows Clean Architecture principles with the 8 core implementation areas:

1. **Four Core Layers**: Domain, Application, Infrastructure, Presentation
2. **Dependency Injection**: Microsoft.Extensions.DependencyInjection with proper lifetimes
3. **Configuration**: appsettings.json + IOptions<T> pattern
4. **Testing**: Domain.Tests, Application.Tests, NetArchTest validation
5. **Logging**: ILogger<T> integration and audit services
6. **Web Services**: Abstracted token/HTTP wrapper services
7. **Validation**: Toolkit validation evolving to FluentValidation
8. **DTOs**: Application layer DTOs for structured workflows

## Four Core Layers Implementation

### Domain Layer
- Pure C# entities, value objects, domain events
- Business rules and domain exceptions
- Repository/service interfaces
- No dependencies on infrastructure or UI

### Application Layer
- Use cases and workflow orchestration
- DTOs for data transfer
- Validation logic (FluentValidation)
- Service contracts and mappers

### Infrastructure Layer
- EF Core DbContext with SQLCipher
- Repository implementations
- External service integrations
- Data access and persistence

### Presentation Layer
- WinUI 3 views and ViewModels
- MVVM Toolkit implementation
- UI validation and messaging
- User interface logic

## Dependency Injection Setup

- Register all services with appropriate lifetimes
- Use interface-based contracts
- Configure DbContext as Scoped
- Wire up all repository and service implementations

## Configuration & Options Implementation

- Use appsettings.json for all configuration
- Implement IOptions<T> pattern
- Environment-specific settings
- Secure configuration management

## Testing & Architecture Validation Setup

- Create tests/ folder structure
- Implement Domain.Tests and Application.Tests
- Add NetArchTest for boundary enforcement
- Unit and integration testing strategies

## Logging & Error Handling Implementation

- Integrate ILogger<T> across all layers
- Implement audit service for compliance
- Domain-level error handling
- Structured logging patterns

## Web Request Wrapper Implementation

- Abstract external service calls
- Implement token provider service
- HTTP client configuration
- Offline fallback strategies

## Validation & DTOs Implementation

- Start with toolkit validation
- Plan migration to FluentValidation
- Implement DTOs for complex workflows
- Application layer validation logic

## Development Phases Overview

OPERATION PRIME development follows a structured 6-phase approach designed to deliver a production-ready incident management system within approximately 3 weeks.

## Phase 1: Foundation & Setup (2-3 days)
**Timeline**: Monday, July 21 to Wednesday, July 23

### Objectives
- Establish project structure and dependencies
- Configure development environment
- Set up basic MVVM architecture
- Implement core data models

### Key Deliverables
- **Project Structure**: Organized folder hierarchy (Models, Views, ViewModels, Services)
- **Dependencies**: NuGet packages (WinUI 3, EF Core, SQLCipher, MVVM Toolkit)
- **Base Models**: Incident base class with Pre/Major inheritance
- **Database Context**: EF Core setup with SQLite and encryption
- **MVVM Foundation**: Base ViewModel classes and property binding

### Technical Tasks
1. **Project Setup**:
   ```bash
   dotnet new winui3 -n OperationPrime
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package SQLitePCLRaw.bundle_e_sqlcipher
   dotnet add package CommunityToolkit.Mvvm
   ```

2. **Folder Structure**:
   ```
   OperationPrime/
   ├── Models/
   ├── Views/
   ├── ViewModels/
   ├── Services/
   ├── Data/
   └── Utils/
   ```

3. **Core Models Implementation**:
   - Base Incident class with shared properties (max 5,000 users)
   - PreIncident and MajorIncident derived classes
   - Enums for Status, Priority, Urgency, Source
   - Support for TextBox autocomplete fields (Applications, SME, Teams)
   - Conditional field logic for SME and bridge call data

### Milestone Criteria
- Project compiles without errors
- Basic window displays
- Database connection established
- Core models defined and tested

## Phase 2: UI Framework (3-4 days)
**Timeline**: Thursday, July 24 to Sunday, July 27

### Objectives
- Build main application shell
- Implement navigation structure
- Create wizard framework
- Establish responsive design patterns

### Key Deliverables
- **Main Shell**: NavigationView with sidebar and content area
- **Dashboard**: Home screen with status cards and quick actions
- **Wizard Framework**: Reusable ContentDialog with TabView stepper
- **Theme Support**: Dark/light mode implementation
- **Responsive Layout**: Adaptive design for different screen sizes

### Technical Tasks
1. **Main Window Structure**:
   - NavigationView with menu items
   - Content frame for page navigation
   - Status bar and notes panel

2. **Dashboard Components**:
   - Incident Assessment Wizard card
   - Active incidents status cards
   - Changes today summary
   - Persistent notes panel

3. **Wizard Framework**:
   - Generic wizard base class
   - Step navigation with validation
   - Progress indication
   - Save draft functionality

### Milestone Criteria
- Complete navigation between all main sections
- Wizard opens and displays all steps
- Responsive behavior on window resize
- Theme switching functional

## Phase 3: Core Functionality (4-5 days)
**Timeline**: Monday, July 28 to Friday, August 1

### Objectives
- Implement incident creation workflows
- Build priority calculation system
- Create data persistence layer
- Integrate Neurons connectivity

### Key Deliverables
- **Pre-Incident Workflow**: Complete creation and editing
- **Major Incident Workflow**: Enhanced workflow with NOI
- **Priority Service**: Automated calculation with matrix
- **Data Services**: CRUD operations with audit trail
- **Neurons Integration**: Neurons connectivity with offline fallback

### Technical Tasks
1. **Priority Calculation Service**:
   ```csharp
   public class PriorityService : IPriorityService
   {
       private readonly string[,] _priorityMatrix;
       
       public string CalculatePriority(IncidentUrgency urgency, int impactedUsers, string? overridePriority = null)
       {
           // Implementation with matrix lookup
       }
   }
   ```

2. **Incident Service**:
   - CRUD operations with EF Core
   - Audit trail generation
   - Data validation and business rules
   - Offline synchronization

3. **Neurons Integration**:
   - HttpClient configuration
   - login connectivity handling using token
   - Error handling and fallback
   - Data mapping and transformation

### Milestone Criteria
- Complete incident creation workflows functional
- Priority calculation working with real-time updates
- Data persists correctly with encryption
- Neurons integration working (with offline fallback)

## Phase 4: Advanced Features (3-4 days)
**Timeline**: Saturday, August 2 to Tuesday, August 5

### Objectives
- Implement NOI generation system
- Build comprehensive detail views
- Add timeline and audit functionality
- Create conversion workflow (Pre to Major)

### Key Deliverables
- **NOI Builder**: Template-based notice generation
- **Detail Views**: Tabbed interface for incident management
- **Timeline System**: Event tracking and manual updates
- **Conversion Workflow**: Pre-incident to Major escalation
- **Email Integration**: NOI distribution and notifications

### Technical Tasks
1. **NOI Generation**:
   ```csharp
   public class NOIBuilder
   {
       public NOIBuilder WithFields(IncidentViewModel vm) { /* ... */ }
       public string RenderTemplate() { /* ... */ }
       public string GeneratePreview() { /* ... */ }
       public void ExportDocx(string path) { /* ... */ }
   }
   ```

2. **Detail View Tabs**:
   - Overview with editable fields
   - Timeline with chronological events
   - Checklist for operational tracking
   - NOI tab for notice management
   - Audit tab for compliance

3. **Conversion Logic**:
   - Data preservation during conversion
   - Additional field completion
   - Audit trail maintenance
   - Stakeholder notification

### Milestone Criteria
- NOI generation produces properly formatted notices
- Detail views display and edit incidents correctly
- Timeline tracks all changes accurately
- Conversion preserves all data integrity

## Phase 5: Testing & Refinement (3-4 days)
**Timeline**: Wednesday, August 6 to Saturday, August 9

### Objectives
- Comprehensive testing across all workflows
- Performance optimization
- Security validation
- Code cleanup and documentation

### Key Deliverables
- **Unit Tests**: xUnit tests for all services and ViewModels
- **Integration Tests**: End-to-end workflow validation
- **Performance Tests**: Load testing with large datasets
- **Security Audit**: Encryption and input validation verification
- **Code Cleanup**: Optimization and redundancy removal

### Technical Tasks
1. **Testing Framework**:
   ```csharp
   [Test]
   public void PriorityService_CalculatesPriorityCorrectly()
   {
       // Arrange
       var service = new PriorityService();
       
       // Act
       var result = service.CalculatePriority(IncidentUrgency.High, 100);
       
       // Assert
       Assert.AreEqual("P1", result);
   }
   ```

2. **Performance Testing**:
   - Large incident lists (500+ items)
   - Concurrent user scenarios
   - Memory usage optimization
   - Database query performance

3. **Security Validation**:
   - Input sanitization testing
   - Encryption verification
   - Audit trail integrity

### Milestone Criteria
- All unit tests pass with >90% coverage
- No memory leaks or performance issues
- Security scan passes without critical issues
- Code meets quality standards

## Phase 6: Deployment & Distribution (1-2 days)
**Timeline**: Sunday, August 10 to Monday, August 11

### Objectives
- Package application for distribution
- Create installation package
- Validate deployment process
- Prepare production documentation

### Key Deliverables
- **MSIX Package**: Windows store-compatible installer
- **MSI Installer**: Traditional Windows installer
- **Deployment Guide**: Installation and configuration instructions
- **User Manual**: End-user documentation
- **Support Documentation**: Troubleshooting and maintenance

### Technical Tasks
1. **Build Configuration**:
   ```xml
   <PropertyGroup>
     <Configuration>Release</Configuration>
     <Platform>x64</Platform>
     <SelfContained>true</SelfContained>
     <RuntimeIdentifier>win10-x64</RuntimeIdentifier>
   </PropertyGroup>
   ```

2. **Package Creation**:
   - MSIX manifest configuration
   - Code signing certificate
   - Dependency bundling
   - Installation testing

3. **Documentation**:
   - Installation procedures
   - Configuration requirements
   - Troubleshooting guides
   - User training materials

### Milestone Criteria
- Installer creates functional application
- All dependencies included and working
- Documentation complete and accurate
- Ready for production deployment

## Development Standards

> **📋 For comprehensive coding guidelines, see [Coding Standards](./CODING_STANDARDS.md)**

### Code Quality
- **Naming Conventions**: PascalCase for public members, camelCase for private
- **Error Handling**: Comprehensive exception handling with logging
- **Performance**: Async/await for all I/O operations
- **Architecture**: Follow SOLID principles and MVVM patterns (detailed in [Coding Standards](./CODING_STANDARDS.md))

### Testing Requirements
- **Unit Tests**: Minimum 80% code coverage
- **Integration Tests**: All major workflows tested
- **Manual Testing**: User acceptance testing for all features
- **Performance Tests**: Load testing with realistic data volumes

### Security Standards
- **Input Validation**: All user inputs sanitized and validated
- **Data Encryption**: AES-256 encryption for all sensitive data
- **Audit Compliance**: Complete action logging for regulatory requirements

## Tools and Environment

### Development Tools
- **IDE**: Visual Studio 2022 or VS Code with C# extension
- **Database**: SQLite with SQLCipher for encryption
- **Testing**: xUnit framework with Moq for mocking
- **Code Analysis**: SonarQube or built-in VS analyzers

### Build and Deployment
- **Build System**: MSBuild with custom targets
- **Package Manager**: NuGet for dependencies
- **Deployment**: MSIX packaging with Windows App SDK
- **Version Control**: Git with feature branch workflow

---

This development guide ensures systematic progress through all phases while maintaining quality and security standards throughout the OPERATION PRIME development lifecycle.
