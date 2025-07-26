# Development Workflow & Pain Point Prevention

> **🛡️ Proactive strategies to prevent critical WinUI 3 and .NET development issues**

## Daily Development Protocol

### 1. XAML Development (Prevents MSB3073 Compilation Errors)

**Root Cause**: Non-nullable properties, complex bindings, incremental compilation issues

**✅ Prevention Protocol**:
```csharp
// ALWAYS use nullable properties for XAML binding
public partial class IncidentListViewModel : ObservableValidator
{
    [ObservableProperty]
    private string? searchText;  // Nullable prevents MSB3073
    
    [ObservableProperty]
    private ObservableCollection<Incident>? incidents;
    
    // Safe property access
    public bool HasResults => Incidents?.Any() == true;
}
```

**Daily Workflow**:
1. Create minimal test XAML first
2. Build after each property addition: `dotnet build`
3. Use verbose logging for errors: `dotnet build -v detailed`
4. Test Hot Reload after each change

### 2. Dependency Injection (Prevents Runtime DI Errors)

**Root Cause**: Incorrect lifetimes, missing registrations, circular dependencies

**✅ Prevention Protocol**:
```csharp
// Correct service registration pattern
public static IServiceCollection AddOperationPrimeServices(this IServiceCollection services)
{
    // ViewModels - Transient (new instance per injection)
    services.AddTransient<IncidentListViewModel>();
    services.AddTransient<IncidentCreateViewModel>();
    
    // Services - Scoped (per request/operation)
    services.AddScoped<IIncidentService, IncidentService>();
    
    // Infrastructure - Singleton (app lifetime)
    services.AddSingleton<INavigationService, NavigationService>();
    
    return services;
}
```

**Validation Checklist**:
- [ ] All ViewModels registered as Transient
- [ ] No circular dependencies
- [ ] DbContext registered once only
- [ ] Test DI container at startup

### 3. Build System (Prevents Build Complexity)

**Root Cause**: Complex project references, XAML compilation conflicts, package conflicts

**✅ Prevention Protocol**:
```xml
<!-- Simplified project configuration -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <UseWinUI>true</UseWinUI>
    <EnableMsixTooling>true</EnableMsixTooling>
    <UseWinUIHostingAPI>true</UseWinUIHostingAPI>
  </PropertyGroup>
</Project>
```

**Daily Build Protocol**:
1. Clean before major changes: `dotnet clean`
2. Build incrementally: one feature at a time
3. Test after each significant change
4. Use separate test project for experiments

### 4. Hot Reload (Prevents Development Slowdown)

**Root Cause**: Complex XAML, binding conflicts, resource conflicts

**✅ Prevention Protocol**:
- Keep XAML simple during development
- Test bindings incrementally
- Restart Hot Reload if issues persist: `Ctrl+Shift+F5`
- Use code-behind for complex logic during development

## Feature Development Workflow

### New Feature Protocol
1. **Create Feature Branch**: `git checkout -b feature/incident-list`
2. **Write Failing Test**: Start with test-driven approach
3. **Implement Minimal Solution**: Get it working first
4. **Refactor for Quality**: Clean up after it works
5. **Update Documentation**: Keep docs current

### XAML Feature Protocol
1. **Create Test Page**: Isolate new XAML in separate page
2. **Test Basic Binding**: Verify ViewModel connection
3. **Add Features Incrementally**: Build after each addition
4. **Move to Production**: Only after thorough testing

## Quality Gates

### Pre-Commit Checklist
- [ ] Solution builds without warnings
- [ ] All unit tests pass
- [ ] XAML Hot Reload functional
- [ ] No binding errors in Output window
- [ ] Memory usage acceptable (< 100MB idle)

### Pre-Feature Checklist
- [ ] Architecture review completed
- [ ] Dependencies identified and registered
- [ ] Test strategy defined
- [ ] Documentation plan created

## SOLID Principles in Practice

### Single Responsibility Principle (SRP)
**Each class should have one reason to change**

```csharp
// ❌ BAD: Multiple responsibilities
public class IncidentService
{
    public void CreateIncident(Incident incident) { }
    public void SendEmail(string email) { }
    public void LogActivity(string message) { }
    public string CalculatePriority(int urgency, int users) { }
    public void GenerateNOI(Incident incident) { }
}

// ✅ GOOD: Single responsibility (Operation Prime pattern)
public class IncidentService
{
    private readonly IIncidentRepository _repository;
    private readonly IPriorityMatrixService _priorityService;
    private readonly IAuditService _auditService;
    
    public async Task<Incident> CreateIncidentAsync(CreateIncidentRequest request)
    {
        // Only handles incident creation logic
        var incident = new Incident(request.Title, request.Description);
        await _repository.AddAsync(incident);
        await _auditService.LogCreationAsync(incident.Id, request.CreatedBy);
        return incident;
    }
}

// Separate services for distinct responsibilities
public class PriorityMatrixService : IPriorityMatrixService
{
    public async Task<PriorityResult> CalculatePriorityAsync(PriorityRequest request) { }
}

public class NOIBuilder : INOIBuilder
{
    public INOIBuilder WithIncident(IncidentViewModel incident) { }
    public string BuildPreview() { }
    public async Task ExportToWordAsync(string filePath) { }
}

public class EmailDistributionService : IEmailDistributionService
{
    public async Task<List<string>> GetSuggestedEmailsAsync(string[] teams) { }
}
```

### Open/Closed Principle (OCP)
**Open for extension, closed for modification**

```csharp
// ✅ GOOD: Extensible validation (Operation Prime pattern)
public abstract class BaseValidator<T>
{
    public abstract ValidationResult Validate(T entity);
    protected virtual List<ValidationError> ValidateCommonFields(T entity) => new();
}

public class IncidentValidator : BaseValidator<Incident>
{
    public override ValidationResult Validate(Incident incident)
    {
        var errors = ValidateCommonFields(incident);
        
        // Incident-specific validation
        if (string.IsNullOrWhiteSpace(incident.Title))
            errors.Add(new ValidationError("Title", "Title is required"));
            
        if (incident.ImpactedUsers.HasValue && incident.ImpactedUsers > 5000)
            errors.Add(new ValidationError("ImpactedUsers", "Maximum 5,000 users supported"));
            
        return new ValidationResult(errors.Count == 0, errors);
    }
}

public class MajorIncidentValidator : BaseValidator<MajorIncident>
{
    public override ValidationResult Validate(MajorIncident majorIncident)
    {
        var errors = ValidateCommonFields(majorIncident);
        
        // Major incident specific validation
        if (string.IsNullOrWhiteSpace(majorIncident.BusinessImpact))
            errors.Add(new ValidationError("BusinessImpact", "Business impact required for Major Incidents"));
            
        if (majorIncident.NOIRecipients?.Any() != true)
            errors.Add(new ValidationError("NOIRecipients", "NOI recipients required for Major Incidents"));
            
        return new ValidationResult(errors.Count == 0, errors);
    }
}

// Extensible priority calculation strategies
public interface IPriorityCalculationStrategy
{
    string CalculatePriority(int urgency, int impactedUsers, IncidentContext context);
}

public class StandardPriorityStrategy : IPriorityCalculationStrategy
{
    public string CalculatePriority(int urgency, int impactedUsers, IncidentContext context)
    {
        // Standard matrix calculation
    }
}

public class SecurityIncidentPriorityStrategy : IPriorityCalculationStrategy
{
    public string CalculatePriority(int urgency, int impactedUsers, IncidentContext context)
    {
        // Security incidents always P1 regardless of matrix
        return "P1";
    }
}
```

### 6-Phase Development Plan
**Structured approach to prevent common WinUI 3 issues:**

#### Phase 1: Foundation (Clean Architecture Setup)
- **Domain Layer**: Entities, value objects, domain services
- **Application Layer**: Use cases, DTOs, interfaces
- **Infrastructure Layer**: Data access, external services
- **Presentation Layer**: ViewModels, views, dependency injection

#### Phase 2: UI Framework (MVVM + Validation)
- **Modern MVVM**: ObservableValidator, [ObservableProperty] partials
- **Validation**: Data annotations → FluentValidation migration
- **Navigation**: NavigationView shell with proper routing
- **Theming**: Dark/light mode support

#### Phase 3: Core Features (Incident Management)
- **Incident Creation**: Pre-incident and Major incident workflows
- **Priority Calculation**: 4-layer priority matrix system
- **Status Management**: State transitions with business rules
- **Data Persistence**: EF Core with SQLCipher encryption

#### Phase 4: Advanced Features (NOI & Integration)
- **NOI Generation**: Builder pattern with .docx templates
- **Email Distribution**: Team-based suggestions and validation
- **Neurons Integration**: Token-based API with circuit breaker
- **Audit Trail**: Comprehensive logging and compliance

#### Phase 5: Testing & Quality Assurance
- **Unit Tests**: Domain and application layer coverage
- **Integration Tests**: Database and external service tests
- **UI Tests**: Automated UI testing with WinAppDriver
- **Architecture Tests**: Dependency rule enforcement

#### Phase 6: Deployment & Distribution
- **MSIX Packaging**: Modern Windows app packaging
- **Auto-Updates**: Seamless update mechanism
- **Configuration**: Environment-specific settings
- **Documentation**: User guides and technical docs

### Quality Gates
**Every development step must pass these gates:**

1. **Build Success**: Code compiles without errors
2. **Unit Tests**: All tests pass with >80% coverage
3. **Hot Reload**: XAML changes reflect immediately
4. **Architecture Compliance**: Follows Clean Architecture principles
5. **Code Review**: Peer review with checklist validation
6. **Performance**: No memory leaks, responsive UI (<100ms)

### Incremental Development Protocol
1. **Start Small**: Single feature, single file changes
2. **Test Early**: Write tests before implementation (TDD)
3. **Verify Often**: Check build and Hot Reload after each change
4. **Document Changes**: Update relevant documentation
5. **Commit Frequently**: Small, atomic commits with clear messages
6. **Review Dependencies**: Ensure proper layer separation

## Debugging Strategy

### Structured Logging Pattern
```csharp
public class IncidentService : IIncidentService
{
    private readonly ILogger<IncidentService> _logger;
    
    public async Task<Result<Incident>> CreateAsync(CreateIncidentRequest request)
    {
        _logger.LogInformation("Creating incident: {Title}", request.Title);
        
        try
        {
            var incident = new Incident(request.Title, request.Description);
            _logger.LogInformation("Incident created: {Id}", incident.Id);
            return Result.Success(incident);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create incident: {Title}", request.Title);
            return Result.Failure<Incident>("Creation failed");
        }
    }
}
```

### Debug Configuration
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Performance Guidelines

### Memory Management
- Dispose ViewModels properly
- Use weak references for event handlers
- Monitor memory usage during development
- Profile regularly with diagnostic tools

### UI Performance
- Virtualize large lists
- Use async/await for long operations
- Implement proper loading states
- Test with realistic data volumes

---
*Focus: Prevent issues before they occur | Daily reference guide*
