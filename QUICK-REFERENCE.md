# 05 - Quick Reference & Glossary

**Navigation**: [← Troubleshooting](./TROUBLESHOOTING-GUIDE.md) | [↑ Home](./README-DOCS.md) | [→ Business Logic](./BUSINESS-RULES.md)

## 🎯 **AI-Vectorization Optimized Reference**

**Purpose**: Fast lookup and pattern matching for daily development  
**Cross-References**: Consolidates patterns from [Architecture](./ARCHITECTURE.md), [Development](./DEVELOPMENT-GUIDE.md), [Implementation](./IMPLEMENTATION-ROADMAP.md)  
**Error Handling**: See [Troubleshooting](./TROUBLESHOOTING-GUIDE.md) for detailed error resolution  
**Complex Logic**: See [Business Logic](./BUSINESS-RULES.md) for full implementations

### 📚 **Quick Navigation Index**

| Category | Section | Use Case |
|----------|---------|----------|
| **Development** | [Essential Commands](#essential-commands) | Build, test, run operations |
| **Architecture** | [Service Registration](#service-registration-patterns) | DI setup and configuration |
| **UI Patterns** | [XAML Binding](#xaml-binding-patterns) | Data binding and validation |
| **Business Rules** | [Priority Matrix](#priority-matrix) | Priority calculation |
| **Workflows** | [Status Transitions](#workflow-states--transitions) | Incident state management |
| **Integration** | [NOI Templates](#noi-templates) | Document generation |
| **Shortcuts** | [Keyboard Shortcuts](#keyboard-shortcuts) | UI navigation |
| **Troubleshooting** | [Common Errors](#common-error-codes) | Quick error resolution |

## Essential Commands
**Cross-Reference**: [Development Workflow](./DEVELOPMENT-GUIDE.md#quality-gates) | [Troubleshooting](./TROUBLESHOOTING-GUIDE.md#build-system-failures)

### Development Commands
```bash
# Build Operations
dotnet clean                                    # Clean build artifacts
dotnet restore --force --no-cache             # Force package restore
dotnet build --no-incremental --verbosity detailed  # Full rebuild with details
dotnet test --collect:"XPlat Code Coverage"    # Run tests with coverage

# Hot Reload & Debugging
dotnet run --launch-profile "OperationPrime"   # Start with Hot Reload
dotnet watch run                               # Auto-restart on changes

# Package Management
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Serilog.Extensions.Logging

# Database Operations
dotnet ef migrations add InitialCreate --context OperationPrimeContext
dotnet ef database update --context OperationPrimeContext
dotnet ef migrations script --context OperationPrimeContext  # Generate SQL

# Architecture Testing
dotnet test tests/Architecture.Tests          # Verify layer dependencies
dotnet test tests/Domain.Tests               # Domain logic tests
dotnet test tests/Integration.Tests          # End-to-end tests
```

### Troubleshooting Commands
```bash
# Diagnostic Commands
dotnet --info                                 # Runtime information
dotnet list package --vulnerable             # Security vulnerabilities
dotnet list package --outdated              # Package updates available

# Build Diagnostics
dotnet build --verbosity diagnostic         # Maximum build details
msbuild /p:Configuration=Debug /v:diag      # MSBuild diagnostics

# Database Diagnostics
sqlite3 incidents.db ".schema"              # Database schema
sqlite3 incidents.db "PRAGMA integrity_check;" # Database integrity
```

## Code Patterns

### ViewModel Pattern
```csharp
public partial class ExampleViewModel : ObservableValidator
{
    [ObservableProperty]
    private string? title;  // Always nullable for XAML
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private ObservableCollection<Item>? items;
    
    public bool HasItems => Items?.Any() == true;
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Command implementation
    }
}
```

### Service Registration Patterns
**Definitive Reference**: [Architecture](./ARCHITECTURE.md#dependency-injection-patterns) | **Troubleshooting**: [DI Failures](./TROUBLESHOOTING-GUIDE.md#dependency-injection-failures)

### Complete Registration Template
```csharp
// Program.cs - Complete service registration
public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Domain Services (Business Logic) - Scoped
    services.AddScoped<IPriorityMatrixService, PriorityMatrixService>();
    services.AddScoped<IIncidentWorkflowService, IncidentWorkflowService>();
    
    // Application Services (Use Cases) - Scoped
    services.AddScoped<IIncidentService, IncidentService>();
    services.AddScoped<INOIBuilder, NOIBuilder>();
    services.AddScoped<IEmailDistributionService, EmailDistributionService>();
    
    // Infrastructure Services - Scoped
    services.AddScoped<IIncidentRepository, IncidentRepository>();
    services.AddScoped<IAuditRepository, AuditRepository>();
    services.AddScoped<INeuronsIntegrationService, NeuronsIntegrationService>();
    
    // Presentation Services (ViewModels) - Transient
    services.AddTransient<IncidentViewModel>();
    services.AddTransient<DashboardViewModel>();
    services.AddTransient<NOIViewModel>();
    
    // Data Context - Scoped
    services.AddDbContext<OperationPrimeContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("Default")));
    
    // Configuration Options - Singleton
    services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
    services.Configure<NeuronsOptions>(configuration.GetSection("Neurons"));
    
    // External Services - Scoped
    services.AddHttpClient<INeuronsIntegrationService, NeuronsIntegrationService>();
    services.AddScoped<ICircuitBreakerService, CircuitBreakerService>();
}
```

### Service Lifetime Quick Reference
| Lifetime | Use For | Examples |
|----------|---------|----------|
| **Singleton** | Stateless, thread-safe services | `IConfiguration`, `ILogger<T>` |
| **Scoped** | Per-request services, DbContext | `IIncidentService`, `OperationPrimeContext` |
| **Transient** | Lightweight, stateful services | ViewModels, DTOs |

### Constructor Injection Pattern
```csharp
// Standard constructor injection
public class IncidentService
{
    private readonly IIncidentRepository _repository;
    private readonly IPriorityMatrixService _priorityService;
    private readonly ILogger<IncidentService> _logger;
    
    public IncidentService(
        IIncidentRepository repository,
        IPriorityMatrixService priorityService,
        ILogger<IncidentService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _priorityService = priorityService ?? throw new ArgumentNullException(nameof(priorityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
```

### XAML Binding Patterns
```xml
<!-- ✅ Safe binding with null checks -->
<TextBlock Text="{x:Bind ViewModel.Title, Mode=OneWay}" />
<ListView ItemsSource="{x:Bind ViewModel.Items, Mode=OneWay}" 
          Visibility="{x:Bind ViewModel.HasItems, Mode=OneWay}" />

<!-- ✅ Command binding -->
<Button Content="Save" 
        Command="{x:Bind ViewModel.SaveCommand}" 
        IsEnabled="{x:Bind ViewModel.CanSave, Mode=OneWay}" />
```

## Glossary

### Architecture Terms
- **Clean Architecture**: 4-layer architecture with dependency inversion
- **Domain Layer**: Pure business logic, no external dependencies
- **Application Layer**: Use cases, services, DTOs
- **Infrastructure Layer**: Data access, external APIs, file system
- **Presentation Layer**: UI, ViewModels, MVVM patterns

### Technical Terms
- **DI**: Dependency Injection - Microsoft.Extensions.DependencyInjection
- **MVVM**: Model-View-ViewModel pattern
- **ObservableValidator**: Base class for ViewModels with validation
- **RelayCommand**: Command implementation from CommunityToolkit.Mvvm
- **Result Pattern**: Return type that encapsulates success/failure

### WinUI 3 Terms
- **x:Bind**: Compiled binding with better performance
- **Hot Reload**: Live code changes without rebuild
- **MSB3073**: Common XAML compilation error code
- **NavigationView**: Main navigation control
- **Frame**: Container for page navigation

### Development Terms
- **Nuclear Clean**: Complete clean of bin/obj folders and rebuild
- **Incremental Build**: Building only changed components
- **Verbose Build**: Build with detailed diagnostic output
- **Circuit Breaker**: Pattern for handling external service failures

## File Structure Reference

```
OperationPrime/
├── Domain/                 # Business logic
│   ├── Entities/          # Domain entities
│   ├── ValueObjects/      # Value objects
│   └── Services/          # Domain services
│   └── Priority Matrix
│       └── Calculation Rules
│       └── Override Rules
│       └── Priority Definitions
├── Application/           # Use cases
│   ├── Services/          # Application services
│   ├── DTOs/             # Data transfer objects
│   └── Interfaces/        # Service interfaces
├── Infrastructure/        # External concerns
│   ├── Data/             # Database context
│   ├── Repositories/     # Data access
│   └── Services/         # External services
└── Presentation/         # UI layer
    ├── ViewModels/       # MVVM ViewModels
    ├── Views/            # XAML pages
    └── Services/         # UI services
```

## Workflow States & Transitions

### Status Definitions
- **Draft**: Just created, no action taken
- **Open**: Active incident, work can begin
- **In Progress**: Being actively worked on
- **Pending**: Waiting for external input or resources
- **Resolved**: Issue fixed, awaiting closure confirmation
- **Closed**: Completed and documented

### Valid State Transitions
```
Draft → Open
Open → In Progress, Resolved
In Progress → Pending, Resolved
Pending → In Progress, Resolved
Resolved → Closed, In Progress (reopening)
Closed → In Progress (reopening with justification)
```

### Business Rules for Transitions
- **To Closed**: Resolution details required
- **Major Incident Closure**: NOI must be sent first
- **Reopening**: Requires justification and audit trail
- **Pending**: Must specify what is being waited forces

## NOI Templates

### Standard Template Structure
```
NOTICE OF INCIDENT (NOI)
========================

Incident Number: {{INCIDENT_NUMBER}}
Title: {{TITLE}}
Priority: {{PRIORITY}}
Start Time: {{START_TIME}}
Created By: {{CREATED_BY}}
Created Date: {{CREATED_DATE}}

BUSINESS IMPACT:
{{BUSINESS_IMPACT}}

AFFECTED SYSTEMS:
{{AFFECTED_SYSTEMS}}

IMPACTED USERS: {{IMPACTED_USERS}}
STATUS: {{STATUS}}

DESCRIPTION:
{{DESCRIPTION}}

NEXT STEPS:
- Investigation in progress
- Updates will be provided as available
- Contact {{CREATED_BY}} for questions
```

### Complete Field Mappings
- `{{INCIDENT_NUMBER}}`: Auto-generated or manual incident ID
- `{{TITLE}}`: Incident title (required, min 5 chars)
- `{{PRIORITY}}`: Calculated priority (P1-P4)
- `{{START_TIME}}`: Incident start timestamp (ISO format)
- `{{CREATED_BY}}`: User who created the incident
- `{{CREATED_DATE}}`: NOI generation timestamp
- `{{BUSINESS_IMPACT}}`: Business impact description (required for Major)
- `{{AFFECTED_SYSTEMS}}`: Comma-separated system list
- `{{IMPACTED_USERS}}`: Number of affected users (1-5000)
- `{{STATUS}}`: Current incident status
- `{{DESCRIPTION}}`: Detailed incident description

### Template Types
- **Standard NOI**: General incident notification
- **Security NOI**: Enhanced security incident template
- **Outage NOI**: Service outage specific template
- **Custom NOI**: User-defined template structure

## Keyboard Shortcuts

### Global Shortcuts
- `Ctrl+N`: New Pre-Incident
- `Ctrl+Shift+N`: New Major Incident
- `Ctrl+S`: Save current incident
- `Ctrl+F`: Search incidents
- `F5`: Refresh data from Neurons
- `Ctrl+D`: Duplicate current incident
- `Ctrl+E`: Export incident data
- `Ctrl+P`: Print/Generate NOI

### Navigation Shortcuts
- `Ctrl+1`: Dashboard view
- `Ctrl+2`: Incident list view
- `Ctrl+3`: Search view
- `Ctrl+4`: Settings view
- `Alt+Left`: Navigate back
- `Alt+Right`: Navigate forward

### Editing Shortcuts
- `Tab`: Next field
- `Shift+Tab`: Previous field
- `Ctrl+Enter`: Save and close
- `Esc`: Cancel current operation
- `F2`: Edit selected item

## Common Error Codes

- **MSB3073**: XAML compilation error - check nullable properties
- **CS0246**: Type not found - check using statements and references
- **CS1061**: Member not found - check property names and accessibility
- **InvalidOperationException**: Service not registered - check DI registration

## Performance Guidelines

### Memory Management
- Dispose ViewModels properly
- Use weak references for events
- Monitor memory usage < 200MB
- Profile with realistic data

### UI Performance
- Virtualize large lists
- Use async/await for operations
- Implement loading states
- Test with realistic data volumes

---
*Quick lookup for daily development | Essential patterns and commands*
