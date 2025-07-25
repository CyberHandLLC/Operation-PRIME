# OPERATION PRIME Technical Specifications

**Keywords**: technical implementation, priority calculation, NOI generation, audit service, performance optimization, security patterns, complex algorithms

## Table of Contents

1. [Clean Architecture Technical Implementation](#clean-architecture-technical-implementation)
2. [Four Core Layers Technical Details](#four-core-layers-technical-details)
3. [Dependency Injection Technical Patterns](#dependency-injection-technical-patterns)
4. [Configuration Technical Implementation](#configuration-technical-implementation)
5. [Testing Technical Architecture](#testing-technical-architecture)
6. [Logging Technical Patterns](#logging-technical-patterns)
7. [Web Service Technical Integration](#web-service-technical-integration)
8. [Validation Technical Implementation](#validation-technical-implementation)
9. [Complex System Implementations](#complex-system-implementations)
10. [Priority Calculation System](#priority-calculation-system)
    - [Multi-Layered Complexity Architecture](#multi-layered-complexity-architecture)
    - [Layer 1: Input Validation](#layer-1-input-validation)
    - [Layer 2: Matrix Computation](#layer-2-matrix-computation)
    - [Layer 3: Override Logic](#layer-3-override-logic)
    - [Layer 4: Real-Time Integration](#layer-4-real-time-integration)
11. [NOI (Notice of Incident) Generation System](#noi-notice-of-incident-generation-system)
    - [Template Engine Architecture](#template-engine-architecture)
    - [NOI Builder Implementation](#noi-builder-implementation)
    - [Template Service](#template-service)
12. [Real-Time Update System](#real-time-update-system)
    - [Event-Driven Architecture](#event-driven-architecture)
    - [Message Bus Implementation](#message-bus-implementation)
    - [Update Coordination Service](#update-coordination-service)
13. [Database Architecture](#database-architecture)
    - [Encryption and Performance](#encryption-and-performance)
    - [Encrypted Context Configuration](#encrypted-context-configuration)
    - [Performance Optimization](#performance-optimization)
14. [Error Handling and Resilience](#error-handling-and-resilience)
    - [Circuit Breaker Pattern for External Neurons Integration](#circuit-breaker-pattern-for-external-neurons-integration)

---

## Clean Architecture Technical Implementation

OPERATION PRIME's technical architecture strictly follows Clean Architecture principles with these 8 core technical areas:

1. **Four Core Layers**: Technical separation with compile-time enforcement
2. **Dependency Injection**: Advanced DI patterns and service lifetimes
3. **Configuration**: Technical configuration management and options patterns
4. **Testing**: Technical testing strategies and architecture validation
5. **Logging**: Technical logging patterns and structured logging
6. **Web Services**: Technical integration patterns and HTTP client management
7. **Validation**: Technical validation architecture and rule engines
8. **DTOs**: Technical data transfer patterns and mapping strategies

## Four Core Layers Technical Details

### Domain Layer Technical Implementation
- Pure C# with no external dependencies
- Domain events using MediatR pattern
- Value objects with proper equality semantics
- Repository interfaces with generic constraints

### Application Layer Technical Implementation
- CQRS pattern with MediatR
- FluentValidation for complex business rules
- AutoMapper for DTO transformations
- Use case orchestration patterns

### Infrastructure Layer Technical Implementation
- EF Core with SQLCipher encryption
- Repository pattern with Unit of Work
- HTTP client factory patterns
- External service integration patterns

### Presentation Layer Technical Implementation
- MVVM Toolkit with ObservableValidator
- Dependency injection for ViewModels
- Message bus for cross-ViewModel communication
- XAML binding optimization patterns

## Dependency Injection Technical Patterns

### Service Registration Patterns
```csharp
// Scoped services for request-scoped operations
services.AddScoped<IIncidentRepository, IncidentRepository>();
services.AddScoped<IIncidentService, IncidentService>();

// Singleton services for stateless operations
services.AddSingleton<IPriorityCalculationService, PriorityCalculationService>();
services.AddSingleton<IMapper, Mapper>();

// Transient services for lightweight operations
services.AddTransient<IValidator<Incident>, IncidentValidator>();
```

### Advanced DI Patterns
- Factory patterns for complex object creation
- Decorator patterns for cross-cutting concerns
- Strategy patterns for algorithm selection

## Configuration Technical Implementation

### Options Pattern Implementation
```csharp
// Configuration binding
services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
services.Configure<NeuronsOptions>(configuration.GetSection("Neurons"));

// Validation
services.AddOptions<DatabaseOptions>()
    .Bind(configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

## Testing Technical Architecture

### Architecture Testing with NetArchTest
```csharp
[Test]
public void Domain_Should_Not_Reference_Infrastructure()
{
    var result = Types.InAssembly(typeof(Incident).Assembly)
        .Should().NotHaveDependencyOn("Infrastructure")
        .GetResult();
    
    Assert.IsTrue(result.IsSuccessful);
}
```

## Logging Technical Patterns

### Structured Logging Implementation
```csharp
_logger.LogInformation("Incident created with {IncidentId} for {UserId} at {Timestamp}",
    incident.Id, userId, DateTime.UtcNow);
```

## Web Service Technical Integration

### HTTP Client Factory Pattern
```csharp
services.AddHttpClient<INeuronsService, NeuronsService>(client =>
{
    client.BaseAddress = new Uri(neuronsOptions.BaseUrl);
    client.Timeout = neuronsOptions.Timeout;
});
```

## Validation Technical Implementation

### FluentValidation Technical Patterns
```csharp
public class IncidentValidator : AbstractValidator<Incident>
{
    public IncidentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(5, 200).WithMessage("Title must be between 5 and 200 characters");
            
        RuleFor(x => x.ImpactedUsers)
            .GreaterThan(0).WithMessage("Must have at least 1 impacted user")
            .LessThanOrEqualTo(5000).WithMessage("Cannot exceed 5,000 users");
    }
}

## Complex System Implementations

This document details the technical implementation of OPERATION PRIME's most complex subsystems, providing architectural guidance for developers working on advanced features.

> **📋 All implementations must follow the guidelines in [Coding Standards](./CODING_STANDARDS.md) for clean code, component isolation, and verification strategies.**

## Priority Calculation System

### Multi-Layered Complexity Architecture

The priority calculation system involves four distinct computational layers that must work together seamlessly:

#### Layer 1: Input Validation
```csharp
public class PriorityValidationService
{
    public ValidationResult ValidateInputs(string urgency, int impactedUsers, string incidentType)
    {
        var errors = new List<string>();
        
        // Urgency validation
        if (!Enum.TryParse<UrgencyEnum>(urgency, out var parsedUrgency))
            errors.Add("Invalid urgency level specified");
            
        // Impact validation with type-specific rules
        if (incidentType == "Pre-Incident" && impactedUsers > 100)
            errors.Add("Pre-incidents with >100 users should be converted to Major Incident");
            
        if (impactedUsers < 0 || impactedUsers > 5000)
            errors.Add("Impacted users must be between 0 and 5,000");
            
        return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
    }
}
```

#### Layer 2: Matrix Computation
```csharp
public class PriorityMatrixService
{
    private readonly string[,] _priorityMatrix = new string[3, 5] 
    {
        {"P2", "P1", "P1", "P1", "P1"}, // High urgency
        {"P3", "P2", "P2", "P1", "P1"}, // Medium urgency
        {"P4", "P3", "P3", "P2", "P2"}  // Low urgency
    };
    
    public string ComputePriority(UrgencyEnum urgency, int impactedUsers)
    {
        int urgencyIndex = (int)urgency - 1;
        int userRangeIndex = GetUserRangeIndex(impactedUsers);
        
        return _priorityMatrix[urgencyIndex, userRangeIndex];
    }
    
    private int GetUserRangeIndex(int users)
    {
        return users switch
        {
            <= 10 => 0,
            <= 50 => 1,
            <= 100 => 2,
            <= 500 => 3,
            _ => 4
        };
    }
}
```

#### Layer 3: Override Logic
```csharp
public class PriorityOverrideService
{
    public string ApplyOverrides(string calculatedPriority, string? overridePriority, string justification)
    {
        if (string.IsNullOrEmpty(overridePriority))
            return calculatedPriority;
            
        // Log override for audit trail
        _auditService.LogPriorityOverride(calculatedPriority, overridePriority, justification);
        
        return overridePriority;
    }
}
```

#### Layer 4: Real-Time Integration
```csharp
public class PriorityService : IPriorityService
{
    private readonly PriorityValidationService _validator;
    private readonly PriorityMatrixService _matrix;
    private readonly PriorityOverrideService _override;
    
    public string CalculatePriority(string urgency, int impactedUsers, string incidentType, string? overridePriority = null, string? justification = null)
    {
        // Layer 1: Validate inputs
        var validation = _validator.ValidateInputs(urgency, impactedUsers, incidentType);
        if (!validation.IsValid)
            throw new ValidationException(string.Join(", ", validation.Errors));
            
        // Layer 2: Compute base priority
        var urgencyEnum = Enum.Parse<UrgencyEnum>(urgency);
        var basePriority = _matrix.ComputePriority(urgencyEnum, impactedUsers);
        
        // Layer 3: Apply overrides
        var finalPriority = _override.ApplyOverrides(basePriority, overridePriority, justification);
        
        return finalPriority;
    }
}
```

## NOI (Notice of Incident) Generation System

### .docx Template Processing Architecture

**Template Storage**: NOI templates are stored as .docx files in the `templates/` folder within the application directory.

**Field Mapping System**: Templates use placeholder tokens that are replaced with incident data:

```csharp
public class NOITemplateService
{
    private readonly Dictionary<string, Func<Incident, string>> _fieldMappings = new()
    {
        ["{{IncidentPriority}}"] = i => i.IncidentPriority,
        ["{{IncidentStartTime}}"] = i => i.IncidentStartTime.ToString("dddd, MMMM dd, yyyy, h:mm tt ET"),
        ["{{TimeReportedToNOC}}"] = i => i.TimeReportedToNOC.ToString("dddd, MMMM dd, yyyy, h:mm tt ET"),
        ["{{IncidentDescription}}"] = i => GenerateIncidentDescription(i),
        ["{{LocationsAffected}}"] = i => i.LocationsAffected,
        ["{{ImpactedUsers}}"] = i => i.ImpactedUsers.ToString(),
        ["{{IncidentNumber}}"] = i => i.IncidentNumber,
        ["{{SupportTeam}}"] = i => string.Join(", ", i.SupportTeam),
        ["{{Status}}"] = i => $"Initial Time [{DateTime.Now:dddd, MMMM dd, yyyy, h:mm tt ET}] - Support teams are investigating the issue."
    };
    
    private string GenerateIncidentDescription(Incident incident)
    {
        // Auto-generate description using template
        var description = $"{incident.ApplicationsAffected} users are experiencing issues. The business impact is that {incident.BusinessImpact}.";
        
        // Allow manual override if detailedDescription is provided
        return !string.IsNullOrEmpty(incident.DetailedDescription) 
            ? incident.DetailedDescription 
            : description;
    }
}
```

### Email Distribution Logic

**Team-Based Suggestions**: Email distribution lists are built from team member data:

```csharp
public class EmailDistributionService
{
    public async Task<List<string>> GetSuggestedEmailsAsync(string[] supportTeams)
    {
        var emails = new List<string>();
        
        foreach (var team in supportTeams)
        {
            var teamMembers = await _teamService.GetTeamMembersAsync(team);
            emails.AddRange(teamMembers.Select(m => m.Email));
        }
        
        return emails.Distinct().ToList();
    }
    
    public async Task<List<string>> GetEmailsFromSMEAsync(string smeContactedName)
    {
        var smeMember = await _teamService.GetMemberByNameAsync(smeContactedName);
        if (smeMember?.Team != null)
        {
            return await GetSuggestedEmailsAsync(new[] { smeMember.Team });
        }
        return new List<string>();
    }
}
```

### Template Engine Architecture

The NOI system uses a builder pattern to handle complex template generation with multiple output formats:

#### NOI Builder Implementation
```csharp
public class NOIBuilder
{
    private IncidentViewModel _incident;
    private string _templateType;
    private Dictionary<string, object> _templateData;
    
    public NOIBuilder WithIncident(IncidentViewModel incident)
    {
        _incident = incident;
        return this;
    }
    
    public NOIBuilder WithTemplate(string templateType)
    {
        _templateType = templateType;
        return this;
    }
    
    public NOIBuilder WithCustomData(Dictionary<string, object> data)
    {
        _templateData = data ?? new Dictionary<string, object>();
        return this;
    }
    
    public string BuildPreview()
    {
        var template = _templateService.GetTemplate(_templateType);
        var mergedData = MergeIncidentData();
        
        return _templateEngine.Render(template, mergedData);
    }
    
    public void ExportToWord(string filePath)
    {
        var content = BuildPreview();
        _documentService.CreateWordDocument(content, filePath);
    }
    
    public void SendEmail(string[] recipients)
    {
        var content = BuildPreview();
        _emailService.SendNOI(content, recipients, _incident.IncidentNumber);
    }
    
    private Dictionary<string, object> MergeIncidentData()
    {
        var data = new Dictionary<string, object>
        {
            ["IncidentNumber"] = _incident.IncidentNumber,
            ["Title"] = _incident.Title,
            ["Priority"] = _incident.IncidentPriority,
            ["StartTime"] = _incident.IncidentStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ["ImpactedUsers"] = _incident.ImpactedUsers,
            ["BusinessImpact"] = _incident.BusinessImpact,
            ["CurrentStatus"] = _incident.IncidentStatus,
            ["TeamResponsible"] = _incident.TeamResponsible,
            ["NOCAnalyst"] = _incident.CreatedBy
        };
        
        // Merge custom template data
        foreach (var kvp in _templateData)
            data[kvp.Key] = kvp.Value;
            
        return data;
    }
}
```

#### Template Service
```csharp
public class NOITemplateService
{
    private readonly Dictionary<string, string> _templates;
    
    public NOITemplateService()
    {
        _templates = LoadTemplatesFromConfig();
    }
    
    public string GetTemplate(string templateType)
    {
        return templateType switch
        {
            "Initial" => _templates["InitialNOI"],
            "Update" => _templates["UpdateNOI"],
            "Resolution" => _templates["ResolutionNOI"],
            _ => throw new ArgumentException($"Unknown template type: {templateType}")
        };
    }
    
    private Dictionary<string, string> LoadTemplatesFromConfig()
    {
        // Load from appsettings.json or external template files
        return new Dictionary<string, string>
        {
            ["InitialNOI"] = @"
                NOTICE OF INCIDENT - INITIAL
                
                Incident Number: {IncidentNumber}
                Title: {Title}
                Priority: {Priority}
                Start Time: {StartTime}
                Impacted Users: {ImpactedUsers}
                
                Business Impact:
                {BusinessImpact}
                
                Current Status: {CurrentStatus}
                Team Responsible: {TeamResponsible}
                NOC Analyst: {NOCAnalyst}
                
                This is an initial notification. Updates will follow.
            ",
            // Additional templates...
        };
    }
}
```

## Real-Time Update System

### Event-Driven Architecture

The real-time update system uses MVVM messaging to coordinate changes across multiple UI components:

#### Message Bus Implementation
```csharp
public class IncidentUpdateMessage
{
    public string IncidentId { get; set; }
    public string UpdateType { get; set; }
    public string FieldName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public string User { get; set; }
}

public class IncidentViewModel : ObservableObject
{
    private readonly IMessenger _messenger;
    
    [ObservableProperty]
    private int _impactedUsers;
    
    [ObservableProperty]
    private string _incidentPriority;
    
    partial void OnImpactedUsersChanged(int oldValue, int newValue)
    {
        // Recalculate priority
        var newPriority = _priorityService.CalculatePriority(IncidentUrgency, newValue, IncidentType);
        
        if (newPriority != IncidentPriority)
        {
            var oldPriority = IncidentPriority;
            IncidentPriority = newPriority;
            
            // Broadcast change
            _messenger.Send(new IncidentUpdateMessage
            {
                IncidentId = IncidentId,
                UpdateType = "PriorityRecalculated",
                FieldName = nameof(IncidentPriority),
                OldValue = oldPriority,
                NewValue = newPriority,
                Timestamp = DateTime.Now,
                User = Environment.UserName
            });
        }
    }
}
```

#### Update Coordination Service
```csharp
public class IncidentUpdateCoordinator : IRecipient<IncidentUpdateMessage>
{
    private readonly IAuditService _auditService;
    private readonly IIncidentService _incidentService;
    
    public void Receive(IncidentUpdateMessage message)
    {
        // Log to audit trail
        _auditService.LogChange(
            message.IncidentId,
            message.FieldName,
            message.OldValue?.ToString(),
            message.NewValue?.ToString(),
            message.User
        );
        
        // Update database
        _incidentService.UpdateFieldAsync(message.IncidentId, message.FieldName, message.NewValue);
        
        // Trigger dependent updates
        TriggerDependentUpdates(message);
    }
    
    private void TriggerDependentUpdates(IncidentUpdateMessage message)
    {
        switch (message.UpdateType)
        {
            case "PriorityRecalculated":
                // Update NOI preview if applicable
                _messenger.Send(new NOIUpdateMessage { IncidentId = message.IncidentId });
                break;
                
            case "StatusChanged":
                // Update timeline
                _messenger.Send(new TimelineUpdateMessage 
                { 
                    IncidentId = message.IncidentId,
                    Event = $"Status changed to {message.NewValue}"
                });
                break;
        }
    }
}
```

## Database Architecture

### Encryption and Performance

The database layer uses SQLCipher for encryption while maintaining performance:

#### Encrypted Context Configuration
```csharp
public class OperationPrimeDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"Data Source={_databasePath};Password={_encryptionKey}";
        optionsBuilder.UseSqlite(connectionString, options =>
        {
            options.CommandTimeout(30);
        });
        
        // Enable WAL mode for better concurrency
        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.EnableServiceProviderCaching();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure encryption for sensitive fields
        modelBuilder.Entity<Incident>()
            .Property(e => e.BusinessImpact)
            .HasConversion(
                v => _encryptionService.Encrypt(v),
                v => _encryptionService.Decrypt(v)
            );
            
        // Configure audit trail
        modelBuilder.Entity<AuditLogEntry>()
            .HasIndex(e => new { e.IncidentId, e.Timestamp })
            .HasDatabaseName("IX_AuditLog_IncidentId_Timestamp");
    }
}
```

#### Performance Optimization
```csharp
public class IncidentRepository : IIncidentRepository
{
    private readonly OperationPrimeDbContext _context;
    private readonly IMemoryCache _cache;
    
    public async Task<Incident> GetIncidentAsync(string incidentId)
    {
        // Check cache first
        if (_cache.TryGetValue($"incident_{incidentId}", out Incident cachedIncident))
            return cachedIncident;
            
        // Load from database with related data
        var incident = await _context.Incidents
            .Include(i => i.TimelineEntries)
            .Include(i => i.AuditLog)
            .FirstOrDefaultAsync(i => i.IncidentId == incidentId);
            
        // Cache for 5 minutes
        if (incident != null)
        {
            _cache.Set($"incident_{incidentId}", incident, TimeSpan.FromMinutes(5));
        }
        
        return incident;
    }
    
    public async Task<PagedResult<Incident>> GetIncidentsPagedAsync(int page, int pageSize, string filter = null)
    {
        var query = _context.Incidents.AsQueryable();
        
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(i => i.Title.Contains(filter) || i.IncidentNumber.Contains(filter));
        }
        
        var totalCount = await query.CountAsync();
        var incidents = await query
            .OrderByDescending(i => i.CreatedNoteTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return new PagedResult<Incident>
        {
            Items = incidents,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
```

## Error Handling and Resilience

### Circuit Breaker Pattern for External Neurons Integration
```csharp
public class NeuronsService : INeuronsService
{
    private readonly HttpClient _httpClient;
    private readonly CircuitBreaker _circuitBreaker;
    
    public async Task<IncidentDetailsDto> GetIncidentDetailsAsync(string incidentNumber)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            var response = await _httpClient.GetAsync($"/api/odata/businessobject/Incidents?filter=IncidentNumber eq '{incidentNumber}'");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IncidentDetailsDto>(json);
        });
    }
}

public class CircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly int _threshold = 5;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (_failureCount >= _threshold && DateTime.Now - _lastFailureTime < _timeout)
        {
            throw new CircuitBreakerOpenException("Circuit breaker is open");
        }
        
        try
        {
            var result = await operation();
            _failureCount = 0; // Reset on success
            return result;
        }
        catch (Exception)
        {
            _failureCount++;
            _lastFailureTime = DateTime.Now;
            throw;
        }
    }
}
```

---

These technical specifications provide detailed implementation guidance for OPERATION PRIME's most complex subsystems, ensuring maintainable and performant code architecture.
