# OPERATION PRIME Quick Reference

**Keywords**: incident management, NOC, network operations, pre-incident, major incident, priority matrix, NOI, Neurons integration

## Table of Contents
1. [Clean Architecture Quick Reference](#clean-architecture-quick-reference)
2. [Four Core Layers Overview](#four-core-layers-overview)
3. [Dependency Injection Quick Guide](#dependency-injection-quick-guide)
4. [Configuration Quick Setup](#configuration-quick-setup)
5. [Testing Quick Commands](#testing-quick-commands)
6. [Logging Quick Patterns](#logging-quick-patterns)
7. [Web Service Quick Integration](#web-service-quick-integration)
8. [Validation Quick Implementation](#validation-quick-implementation)
9. [Quick Start Cheat Sheet](#quick-start-cheat-sheet)
10. [Common Workflows](#common-workflows)
11. [Keyboard Shortcuts](#keyboard-shortcuts)
12. [Troubleshooting](#troubleshooting)

---

## Clean Architecture Quick Reference

### 8 Core Implementation Areas
1. **Four Core Layers**: Domain → Application → Infrastructure → Presentation
2. **Dependency Injection**: Use Microsoft.Extensions.DependencyInjection
3. **Configuration**: appsettings.json + IOptions<T>
4. **Testing**: Domain.Tests + Application.Tests + NetArchTest
5. **Logging**: ILogger<T> in all layers
6. **Web Services**: Abstract external calls behind interfaces
7. **Validation**: Toolkit → FluentValidation migration path
8. **DTOs**: Application layer for complex workflows

## Four Core Layers Overview

| Layer | Purpose | Dependencies | Key Files |
|-------|---------|--------------|----------|
| Domain | Business logic, entities | None | Entities/, ValueObjects/, Interfaces/ |
| Application | Use cases, DTOs | Domain only | Services/, DTOs/, Validators/ |
| Infrastructure | Data access, external APIs | Domain, Application | Data/, Repositories/, Services/ |
| Presentation | UI, ViewModels | Application, Domain | ViewModels/, Views/, Utils/ |

## Dependency Injection Quick Guide

```csharp
// Service Registration
services.AddScoped<IIncidentRepository, IncidentRepository>();
services.AddScoped<IIncidentService, IncidentService>();
services.AddTransient<IncidentViewModel>();
services.AddDbContext<OperationPrimeContext>(options => 
    options.UseSqlite(connectionString));
```

## Configuration Quick Setup

```csharp
// appsettings.json
{
  "Database": {
    "ConnectionString": "Data Source=incidents.db",
    "EncryptionKey": "your-key-here"
  },
  "Neurons": {
    "BaseUrl": "https://api.neurons.com",
    "Timeout": "00:00:30"
  }
}

// Configuration class
public class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public string EncryptionKey { get; set; }
}
```

## Testing Quick Commands

```bash
# Run all tests
dotnet test

# Run Domain tests only
dotnet test tests/Domain.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Architecture tests
dotnet test tests/Architecture.Tests
```

## Logging Quick Patterns

```csharp
// Constructor injection
public class IncidentService
{
    private readonly ILogger<IncidentService> _logger;
    
    public IncidentService(ILogger<IncidentService> logger)
    {
        _logger = logger;
    }
    
    public async Task CreateIncident(Incident incident)
    {
        _logger.LogInformation("Creating incident {IncidentId}", incident.Id);
        // Implementation
    }
}
```

## Web Service Quick Integration

```csharp
// Interface (in Application/Domain)
public interface ITokenProviderService
{
    Task<string> GetTokenAsync();
    Task<IncidentData> GetIncidentDataAsync(string incidentNumber);
}

// Implementation (in Infrastructure)
public class NeuronsTokenProviderService : ITokenProviderService
{
    // HTTP client implementation
}
```

## Validation Quick Implementation

```csharp
// Current: Toolkit validation
public class IncidentViewModel : BaseViewModel
{
    [ObservableProperty]
    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
    private string title;
}

// Future: FluentValidation
public class IncidentValidator : AbstractValidator<Incident>
{
    public IncidentValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ImpactedUsers).GreaterThan(0);
    }
}
```

## Quick Start Cheat Sheet

### Creating Incidents
| Task | Action | Location |
|------|--------|----------|
| Create Pre-Incident | Click "New Pre-Incident" | Dashboard |
| Create Major Incident | Click "New Major Incident" | Dashboard |

### Priority Matrix (Urgency × Impact)
| Urgency | 1-50 Users | 51-200 Users | 201-500 Users | 500-5000 Users |
|---------|------------|---------------|---------------|----------------|
| High (1) | P2 | P1 | P1 | P1 |
| Medium (2) | P3 | P2 | P2 | P1 |
| Low (3) | P4 | P3 | P2 | P2 |

*Maximum 5,000 impacted users supported*

### Common Workflows
1. **Unified Incident Flow**: Basics → Details (conditional) → Checklist (Major only) → NOI Preview (Major only)
2. **Autocomplete Fields**: Applications, SME Names, Support Teams auto-suggest from lists
3. **NOI Generation**: Select .docx template → Auto-populate → Manual edit → Send to team emails
4. **Conditional Logic**: SME fields appear when "SME Contacted" = Yes

### Keyboard Shortcuts
- `Ctrl+N`: New Pre-Incident
- `Ctrl+Shift+N`: New Major Incident
- `Ctrl+S`: Save current incident
- `F5`: Refresh data from Neurons
- `Ctrl+F`: Search incidents

### Status Definitions
- **New**: Just created, no action taken
- **In Progress**: Being actively worked
- **Resolved**: Issue fixed, awaiting closure
- **Closed**: Completed and documented

### Required Fields by Type
**Pre-Incident**: Title, Source, Start Time, Created By, Incident Number
**Major Incident**: All Pre-Incident fields + Business Impact, NOI Recipients

### Neurons Integration
- **Login**: Settings → Neurons → Login with credentials
- **Fetch Data**: Enter incident number → Click "Fetch from Neurons"
- **Offline Mode**: All features work without Neurons connection

### Common Troubleshooting
| Issue | Solution |
|-------|----------|
| Can't connect to Neurons | Check network, verify credentials in Settings |
| Priority not calculating | Verify Urgency and Impacted Users are set |
| NOI won't generate | Ensure all required fields completed |
| Data not saving | Check disk space, restart application |

### File Locations
- **Database**: `%APPDATA%\OperationPrime\incidents.db`
- **Logs**: `%APPDATA%\OperationPrime\logs\`
- **Config**: `%APPDATA%\OperationPrime\config.json`

## Development Resources

- **[Coding Standards](./CODING_STANDARDS.md)** - Clean code practices and architectural guidelines
- **[Technical Specifications](./TECHNICAL_SPECS.md)** - Complex system implementations
- **[Architecture Guide](./ARCHITECTURE.md)** - System design and patterns

---
*For detailed information, see the full documentation in the `/docs` folder.*
