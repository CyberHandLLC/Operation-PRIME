# Architecture & Core Patterns

> **🏗️ Essential architecture principles and technical patterns for Operation Prime**

## Clean Architecture Foundation

**Layer Dependencies**: Domain ← Application ← Infrastructure, Presentation → Application

```
┌─────────────────┐    ┌─────────────────┐
│   Presentation  │───▶│   Application   │
│   (WinUI 3)     │    │   (Use Cases)   │
└─────────────────┘    └─────────────────┘
                                │
                                ▼
┌─────────────────┐    ┌─────────────────┐
│ Infrastructure  │───▶│     Domain      │
│ (Data/External) │    │ (Business Logic)│
└─────────────────┘    └─────────────────┘
```

### Layer Responsibilities
- **Domain**: Entities, value objects, business rules (no dependencies)
- **Application**: Services, DTOs, interfaces, use cases
- **Infrastructure**: Data access, external APIs, file system
- **Presentation**: ViewModels, Views, MVVM patterns

## Core Technical Patterns

### Dependency Injection Patterns
**DEFINITIVE REFERENCE** - *Cross-referenced in [Development](./02-development.md) and [Reference](./05-reference.md)*

### Service Registration Strategy
```csharp
// Program.cs - Complete service registration pattern
public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Domain Services (Business Logic)
    services.AddScoped<IPriorityMatrixService, PriorityMatrixService>();
    services.AddScoped<IIncidentWorkflowService, IncidentWorkflowService>();
    
    // Application Services (Use Cases)
    services.AddScoped<IIncidentService, IncidentService>();
    services.AddScoped<INOIBuilder, NOIBuilder>();
    services.AddScoped<IEmailDistributionService, EmailDistributionService>();
    
    // Infrastructure Services (Data & External)
    services.AddScoped<IIncidentRepository, IncidentRepository>();
    services.AddScoped<IAuditRepository, AuditRepository>();
    services.AddScoped<INeuronsIntegrationService, NeuronsIntegrationService>();
    
    // Presentation Services (ViewModels)
    services.AddTransient<IncidentViewModel>();
    services.AddTransient<DashboardViewModel>();
    services.AddTransient<NOIViewModel>();
    
    // DbContext with proper lifetime
    services.AddDbContext<OperationPrimeContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("Default")));
    
    // Configuration Options
    services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
    services.Configure<NeuronsOptions>(configuration.GetSection("Neurons"));
}
```

### Service Lifetime Rules
| Service Type | Lifetime | Reason | Examples |
|--------------|----------|--------|-----------|
| **Configuration** | Singleton | Immutable, shared | `IOptions<T>`, `IConfiguration` |
| **Logging** | Singleton | Thread-safe, shared | `ILogger<T>`, `ILoggerFactory` |
| **Business Services** | Scoped | Per-request state | `IIncidentService`, `IPriorityMatrixService` |
| **Repositories** | Scoped | DbContext lifetime | `IIncidentRepository`, `IAuditRepository` |
| **DbContext** | Scoped | EF Core requirement | `OperationPrimeContext` |
| **ViewModels** | Transient | UI-specific state | `IncidentViewModel`, `DashboardViewModel` |
| **External Services** | Scoped | Connection pooling | `INeuronsIntegrationService` |

### Constructor Injection Best Practices
```csharp
// ✅ CORRECT: Clean constructor with interface dependencies
public class IncidentService
{
    private readonly IIncidentRepository _repository;
    private readonly IPriorityMatrixService _priorityService;
    private readonly IAuditService _auditService;
    private readonly ILogger<IncidentService> _logger;
    
    public IncidentService(
        IIncidentRepository repository,
        IPriorityMatrixService priorityService,
        IAuditService auditService,
        ILogger<IncidentService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _priorityService = priorityService ?? throw new ArgumentNullException(nameof(priorityService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}

// ❌ WRONG: Too many dependencies (violates SRP)
public class IncidentService
{
    public IncidentService(
        IIncidentRepository repository,
        IPriorityMatrixService priorityService,
        IAuditService auditService,
        IEmailService emailService,
        INOIBuilder noiBuilder,
        IValidationService validationService,
        ILogger<IncidentService> logger) // 7+ dependencies = code smell
    {
        // This class is doing too much!
    }
}
```

### DI Troubleshooting
**See [Troubleshooting Guide](./04-troubleshooting.md#dependency-injection-failures) for detailed resolution steps**

- **Service not registered**: Check service registration in `Program.cs`
- **Circular dependencies**: Review constructor dependencies
- **Lifetime mismatches**: Verify service lifetimes are appropriate
- **Interface not found**: Ensure interface is properly implemented

### MVVM Pattern
```csharp
// ✅ ViewModel Pattern (prevents XAML compilation errors)
public partial class IncidentViewModel : ObservableValidator
{
    [ObservableProperty]
    private string? title;  // Nullable for XAML binding
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasIncidents))]
    private ObservableCollection<Incident>? incidents;
    
    public bool HasIncidents => Incidents?.Any() == true;
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Command implementation
    }
}
```

### Clean Architecture Layers

### Cross-Layer Communication Rules
**CRITICAL: These boundaries must never be violated**

```
Presentation → Application → Domain
     ↓              ↓         ↑
Infrastructure → Application → Domain
```

#### **Allowed Dependencies**:
- **Domain**: No dependencies (pure business logic)
- **Application**: Domain only
- **Infrastructure**: Domain + Application interfaces
- **Presentation**: Application + Domain (for ViewModels)

#### **FORBIDDEN Dependencies**:
- ❌ Domain → Application/Infrastructure/Presentation
- ❌ Application → Infrastructure/Presentation
- ❌ Infrastructure → Presentation (direct)

#### **Communication Patterns**:
```csharp
// ✅ CORRECT: Presentation → Application
public class IncidentViewModel
{
    private readonly IIncidentService _incidentService; // Application interface
    
    public async Task CreateIncidentAsync()
    {
        var request = new CreateIncidentRequest { /* DTO */ };
        await _incidentService.CreateAsync(request); // Through Application layer
    }
}

// ❌ WRONG: Presentation → Infrastructure
public class IncidentViewModel
{
    private readonly IncidentRepository _repository; // Direct Infrastructure access
}

// ✅ CORRECT: Application → Domain
public class IncidentService
{
    public async Task<Incident> CreateAsync(CreateIncidentRequest request)
    {
        var incident = new Incident(request.Title); // Domain entity
        var priority = _priorityService.Calculate(request); // Domain service
        return await _repository.AddAsync(incident); // Interface in Domain
    }
}
```

### Domain Layer (Core)
**No dependencies on other layers**

- **Entities**: Core business objects (`Incident`, `Priority`, `Status`)
- **Value Objects**: Immutable objects with business meaning
- **Domain Services**: Business logic that doesn't belong to entities
- **Repository Interfaces**: Data access contracts
- **Domain Events**: Business event notifications

### Application Layer
**Depends only on Domain**

- **Use Cases**: Application-specific business rules
- **DTOs**: Data transfer objects for layer boundaries
- **Interfaces**: Service contracts for infrastructure
- **Validation**: Input validation and business rule enforcement
- **Mapping**: Entity ↔ DTO transformations

## Core Business Systems

### Priority Calculation System
**4-Layer Architecture Implementation**:

```csharp
// Domain/Services/PriorityMatrixService.cs
public class PriorityMatrixService : IPriorityMatrixService
{
    // Priority Matrix: [Urgency, ImpactedUsers] -> Priority
    private readonly string[,] _priorityMatrix = new string[3, 5] 
    {
        // Users:     1-50   51-200  201-500  501-2000  2001-5000
        {"P2", "P1", "P1", "P1", "P1"},  // High urgency (1)
        {"P3", "P2", "P2", "P1", "P1"},  // Medium urgency (2)
        {"P4", "P3", "P3", "P2", "P2"}   // Low urgency (3)
    };

    public async Task<PriorityResult> CalculatePriorityAsync(PriorityRequest request)
    {
        // Layer 1: Input Validation
        var validationResult = ValidateInput(request);
        
        // Layer 2: Matrix Computation
        var matrixPriority = ComputeMatrixPriority(request.Urgency, request.ImpactedUsers);
        
        // Layer 3: Override Logic (Security incidents, Data center outages)
        var finalPriority = ApplyOverrideLogic(matrixPriority, request);
        
        // Layer 4: Real-time Integration & Audit
        await LogPriorityCalculation(request, finalPriority);
        
        return PriorityResult.Success(finalPriority, GetPriorityExplanation(request, finalPriority));
    }
}
```

### NOI Generation System
**Builder Pattern with .docx Template Processing**:

```csharp
// Application/Services/NOIBuilder.cs
public class NOIBuilder : INOIBuilder
{
    public INOIBuilder WithIncident(IncidentViewModel incident) { /* Set incident data */ }
    public INOIBuilder WithTemplate(string templatePath) { /* Set .docx template */ }
    public string BuildPreview() { /* Generate preview with field mappings */ }
    public async Task ExportToWordAsync(string filePath) { /* Export to .docx */ }
    public async Task<EmailResult> SendEmailAsync(string[] recipients) { /* Email distribution */ }
    
    private Dictionary<string, string> BuildFieldMappings()
    {
        return new Dictionary<string, string>
        {
            ["{{INCIDENT_NUMBER}}"] = _incident.IncidentNumber,
            ["{{TITLE}}"] = _incident.Title,
            ["{{PRIORITY}}"] = _incident.Priority,
            ["{{BUSINESS_IMPACT}}"] = _incident.BusinessImpact,
            ["{{AFFECTED_SYSTEMS}}"] = string.Join(", ", _incident.AffectedSystems),
            // ... additional field mappings
        };
    }
}
```

### Email Distribution Logic
**Team-Based Email Suggestions**:

```csharp
// Application/Services/EmailDistributionService.cs
public async Task<List<string>> GetSuggestedEmailsAsync(string[] supportTeams)
{
    var suggestedEmails = new List<string>();
    
    foreach (var team in supportTeams)
    {
        var teamMembers = await _teamMemberRepository.GetTeamMembersAsync(team);
        var teamEmails = teamMembers.Select(m => m.Email).Where(e => !string.IsNullOrEmpty(e));
        suggestedEmails.AddRange(teamEmails);
    }
    
    // Add default distribution lists
    var defaultLists = await _configurationService.GetDefaultDistributionListsAsync();
    suggestedEmails.AddRange(defaultLists);
    
    return suggestedEmails.Distinct().OrderBy(e => e).ToList();
}
```

## Performance & Scalability

### Circuit Breaker Pattern
**For External Service Integration**:

```csharp
// Infrastructure/CircuitBreaker/CircuitBreakerService.cs
public class CircuitBreakerService<T>
{
    private CircuitBreakerState _state = CircuitBreakerState.Closed;
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly int _failureThreshold;
    private readonly TimeSpan _timeout;
    
    public async Task<T> ExecuteAsync(Func<Task<T>> operation)
    {
        if (_state == CircuitBreakerState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime < _timeout)
                throw new CircuitBreakerOpenException();
            
            _state = CircuitBreakerState.HalfOpen;
        }
        
        try
        {
            var result = await operation();
            OnSuccess();
            return result;
        }
        catch (Exception ex)
        {
            OnFailure();
            throw;
        }
    }
}
```

### Caching Strategy
- **In-Memory Caching**: Priority matrix, team member data, configuration settings
- **Database Optimization**: Indexed queries on incident number, status, priority
- **Lazy Loading**: UI components and large datasets loaded on demand
- **Background Processing**: NOI generation, email sending, Neurons sync

### Resource Management
- **Memory Usage**: Efficient object disposal with `using` statements and `IDisposable`
- **Database Connections**: Connection pooling with configurable timeout (30s default)
- **UI Responsiveness**: Background threads for all I/O operations
- **Error Recovery**: Circuit breaker pattern for external services, graceful degradation

## Technology Stack

### Core Technologies
- **.NET 9**: Latest framework features
- **WinUI 3**: Modern Windows UI framework
- **EF Core**: Data access with SQLCipher encryption
- **CommunityToolkit.Mvvm**: MVVM source generators

### Key Libraries
- **Microsoft.Extensions.DependencyInjection**: Service container
- **Microsoft.Extensions.Logging**: Structured logging
- **FluentValidation**: Business rule validation
- **SQLCipher**: Database encryption

## Configuration Patterns

### Database Configuration
```csharp
// ✅ DbContext Registration
services.AddDbContext<OperationPrimeDbContext>(options =>
    options.UseSqlite(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
```

### Logging Configuration
```csharp
// ✅ Structured Logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

## Security & Compliance

### Data Encryption
- **Database**: SQLCipher for encrypted SQLite storage with AES-256 encryption
- **Configuration**: Sensitive settings encrypted at rest using DPAPI
- **Audit Trail**: All user actions logged with timestamps and digital signatures
- **Access Control**: Role-based permissions with Windows Authentication integration

### Database Encryption Implementation
```csharp
// Infrastructure/Data/OperationPrimeContext.cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var connectionString = $"Data Source={_databasePath};Password={_encryptionKey}";
    optionsBuilder.UseSqlite(connectionString, options => {
        options.CommandTimeout(30);
    });
    
    // Enable SQLCipher encryption
    optionsBuilder.UseSqlite(connectionString, sqliteOptions => {
        sqliteOptions.CommandTimeout(30);
    });
}

// Key derivation for database encryption
public class DatabaseKeyProvider
{
    public string DeriveKey(string masterKey, string salt)
    {
        using var rfc2898 = new Rfc2898DeriveBytes(masterKey, Encoding.UTF8.GetBytes(salt), 10000);
        return Convert.ToBase64String(rfc2898.GetBytes(32));
    }
}
```

### Compliance Features
- **Audit Logging**: Comprehensive action tracking with tamper-proof logging
- **Data Retention**: Configurable retention policies with automatic archival
- **Export Capabilities**: Incident data export for compliance reporting (CSV, JSON, XML)
- **Backup Strategy**: Automated encrypted backups with integrity verification

---
*Focus: Build-ready patterns | Essential for development*
