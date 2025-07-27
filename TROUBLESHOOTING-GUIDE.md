# Troubleshooting & Emergency Protocols

> **🚨 Rapid issue resolution for critical WinUI 3 and .NET development problems**

## Emergency Response Protocols

### XAML Compilation Failure (MSB3073)

**Symptoms**: Build fails with XAML compilation errors, unclear error messages

**🚨 Immediate Response**:
1. **Revert to Last Working State**
   ```bash
   git stash
   git checkout HEAD~1
   dotnet build  # Verify it works
   ```

2. **Isolate the Problem**
   - Create minimal test page with problematic XAML
   - Remove complex bindings temporarily
   - Test with simple string properties first

3. **Fix Common Issues**
   ```csharp
   // ❌ WRONG - Non-nullable property
   private string title;
   
   // ✅ CORRECT - Nullable property
   private string? title;
   ```

4. **Rebuild Incrementally**
   ```bash
   dotnet clean
   dotnet build -v detailed
   ```

### Dependency Injection Failure

**Symptoms**: Runtime crashes, service not found exceptions, circular dependency errors

**🚨 Immediate Response**:
1. **Check Service Registration Order**
   ```csharp
   // ✅ Register dependencies first, consumers second
   services.AddScoped<IIncidentRepository, IncidentRepository>();
   services.AddScoped<IIncidentService, IncidentService>();  // Uses repository
   services.AddTransient<IncidentViewModel>();  // Uses service
   ```

2. **Validate Service Lifetimes**
   ```csharp
   // ❌ WRONG - Singleton consuming Scoped
   services.AddSingleton<IIncidentService, IncidentService>();  // Bad
   services.AddScoped<IIncidentRepository, IncidentRepository>();
   
   // ✅ CORRECT - Scoped consuming Scoped
   services.AddScoped<IIncidentService, IncidentService>();
   services.AddScoped<IIncidentRepository, IncidentRepository>();
   ```

3. **Test with Minimal Services**
   - Comment out all but essential services
   - Add back one by one to identify problem

### Build System Failure

**Symptoms**: Random build failures, package conflicts, MSBuild errors

**🚨 Nuclear Clean Protocol**:
```bash
# 1. Complete clean
dotnet clean
Remove-Item -Recurse -Force bin, obj

# 2. Restore packages
dotnet restore

# 3. Build with diagnostics
dotnet build -v detailed

# 4. If still failing, check for:
# - Package version conflicts
# - Corrupted NuGet cache
# - File locks from previous builds
```

### Hot Reload Failure

**Symptoms**: Changes not reflecting, Hot Reload stops working

**🚨 Immediate Response**:
1. **Restart Hot Reload**: `Ctrl+Shift+F5`
2. **Check for XAML Errors**: Look in Output window
3. **Simplify XAML**: Remove complex bindings temporarily
4. **Full Restart**: Close Visual Studio, rebuild solution

## Common Issues & Solutions

### Issue: "Cannot resolve service for type 'X'"

**Root Cause**: Service not registered or wrong lifetime

**Solution**:
```csharp
// Check App.xaml.cs or ServiceCollectionExtensions.cs
services.AddTransient<YourViewModel>();
services.AddScoped<IYourService, YourService>();
```

### Issue: "Object reference not set to an instance"

**Root Cause**: Null reference in ViewModel or binding

**Solution**:
```csharp
// ✅ Always initialize collections
[ObservableProperty]
private ObservableCollection<Incident> incidents = new();

// ✅ Use null-conditional operators
public bool HasIncidents => Incidents?.Any() == true;
```

### Issue: "The name 'X' does not exist in the current context"

**Root Cause**: XAML binding to non-existent property

**Solution**:
```csharp
// ✅ Ensure property exists and is public
[ObservableProperty]
private string? title;  // Generates public Title property
```

### Issue: Memory leaks in ViewModels

**Root Cause**: Event handlers not unsubscribed

**Solution**:
```csharp
public partial class IncidentViewModel : ObservableObject, IDisposable
{
    private readonly IViewModelStateService _stateService;
    
    public IncidentViewModel(IViewModelStateService stateService)
    {
        _stateService = stateService;
    }
    
    public void Dispose()
    {
        // Unsubscribe from events
        SomeService.SomeEvent -= OnSomeEvent;
        
        // Dispose of injected services if needed
        if (_stateService is IDisposable disposableState)
            disposableState.Dispose();
    }
}
```

## Diagnostic Tools & Commands

### Build Diagnostics
```bash
# Verbose build output
dotnet build -v detailed

# MSBuild binary log (for deep analysis)
dotnet build -bl:build.binlog

# Check for warnings
dotnet build --verbosity normal
```

### Runtime Diagnostics
```csharp
// Add to App.xaml.cs for DI diagnostics
#if DEBUG
services.AddLogging(builder => builder.AddDebug().SetMinimumLevel(LogLevel.Trace));
#endif
```

### Memory Diagnostics
- Use Visual Studio Diagnostic Tools
- Monitor memory usage during development
- Profile with realistic data sets

## Prevention Checklist

### Before Each Commit
- [ ] Solution builds without warnings
- [ ] All unit tests pass
- [ ] XAML Hot Reload functional
- [ ] No binding errors in Output window
- [ ] Memory usage acceptable

### Before Each Feature
- [ ] Create feature branch
- [ ] Plan service registrations
- [ ] Design ViewModel structure
- [ ] Consider XAML complexity

### Weekly Health Check
- [ ] Review build warnings
- [ ] Check memory usage trends
- [ ] Validate all navigation paths
- [ ] Test with realistic data

## Escalation Path

### Level 1: Self-Service (5 minutes)
- Check this troubleshooting guide
- Try nuclear clean protocol
- Revert to last working state

### Level 2: Team Review (15 minutes)
- Code review with team member
- Architecture review if needed
- Check against coding standards

### Level 3: Deep Dive (1 hour)
- Detailed diagnostic logging
- Memory profiling
- Architecture refactoring if needed

## Error Handling & Logging Patterns
**DEFINITIVE REFERENCE** - *Referenced by [Development](./DEVELOPMENT-GUIDE.md) and [Business Logic](./BUSINESS-RULES.md)*

### Structured Logging Standards
```csharp
// ✅ CORRECT: Structured logging with context
public class IncidentService
{
    private readonly ILogger<IncidentService> _logger;
    
    public async Task<Incident> CreateAsync(CreateIncidentRequest request)
    {
        using var scope = _logger.BeginScope("IncidentCreation");
        
        _logger.LogInformation("Starting incident creation for user {UserId} with title {Title}", 
            request.CreatedBy, request.Title);
            
        try
        {
            var incident = new Incident(request.Title, request.Description);
            var result = await _repository.AddAsync(incident);
            
            _logger.LogInformation("Successfully created incident {IncidentId} with priority {Priority}", 
                result.Id, result.Priority);
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create incident for user {UserId}: {Error}", 
                request.CreatedBy, ex.Message);
            throw;
        }
    }
}

// ❌ WRONG: String interpolation and poor context
_logger.LogInformation($"Creating incident {incident.Id} for user {userId}");
_logger.LogError($"Error: {ex.Message}"); // No context or structured data
```

### Exception Handling Hierarchy
```csharp
// Custom exception hierarchy for Operation Prime
public abstract class OperationPrimeException : Exception
{
    protected OperationPrimeException(string message) : base(message) { }
    protected OperationPrimeException(string message, Exception innerException) : base(message, innerException) { }
}

// Domain exceptions
public class PriorityCalculationException : OperationPrimeException
{
    public PriorityCalculationException(string message) : base(message) { }
    public PriorityCalculationException(string message, Exception innerException) : base(message, innerException) { }
}

public class WorkflowValidationException : OperationPrimeException
{
    public WorkflowValidationException(string message) : base(message) { }
    public List<ValidationError> ValidationErrors { get; set; } = new();
}

// Infrastructure exceptions
public class DatabaseException : OperationPrimeException
{
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

public class ExternalServiceException : OperationPrimeException
{
    public ExternalServiceException(string service, string message, Exception innerException) 
        : base($"External service '{service}' failed: {message}", innerException) 
    {
        ServiceName = service;
    }
    
    public string ServiceName { get; }
}

// Application exceptions
public class IncidentNotFoundException : OperationPrimeException
{
    public IncidentNotFoundException(string incidentId) 
        : base($"Incident with ID '{incidentId}' was not found") 
    {
        IncidentId = incidentId;
    }
    
    public string IncidentId { get; }
}
```

## Business Logic Error Debugging
**Cross-Reference**: [Business Logic](./BUSINESS-RULES.md) | [Implementation](./IMPLEMENTATION-ROADMAP.md#workflow-specifications)

### Priority Calculation Failures
```csharp
// Debug priority calculation issues
public class PriorityCalculationDebugger
{
    public void DebugPriorityCalculation(PriorityRequest request)
    {
        _logger.LogDebug("=== Priority Calculation Debug ===");
        _logger.LogDebug("Input - Urgency: {Urgency}, Users: {Users}, Security: {Security}", 
            request.Urgency, request.ImpactedUsers, request.IsSecurityIncident);
        
        // Validate input ranges
        if (request.Urgency < 1 || request.Urgency > 3)
        {
            _logger.LogError("Invalid urgency value: {Urgency}. Must be 1-3", request.Urgency);
            return;
        }
        
        if (request.ImpactedUsers < 1 || request.ImpactedUsers > 5000)
        {
            _logger.LogError("Invalid impacted users: {Users}. Must be 1-5000", request.ImpactedUsers);
            return;
        }
        
        var userIndex = GetUserRangeIndex(request.ImpactedUsers);
        _logger.LogDebug("User Range Index: {Index} (Range: {Range})", userIndex, GetUserRangeDescription(userIndex));
        
        var matrixResult = _priorityMatrix[request.Urgency - 1, userIndex];
        _logger.LogDebug("Matrix Result: {Result}", matrixResult);
        
        if (request.IsSecurityIncident)
            _logger.LogDebug("Security Override Applied: {Original} -> P1", matrixResult);
    }
    
    // Emergency priority fallback
    public string GetEmergencyPriority(int urgency, int? impactedUsers)
    {
        _logger.LogWarning("Using emergency priority fallback");
        
        // Fail-safe priority assignment
        if (urgency == 1 || (impactedUsers.HasValue && impactedUsers > 1000))
            return "P1";
        if (urgency == 2 || (impactedUsers.HasValue && impactedUsers > 100))
            return "P2";
        return "P3";
    }
}
```

### NOI Generation Failures
```csharp
// Debug NOI generation issues
public class NOIGenerationDebugger
{
    public void DebugNOIGeneration(IncidentViewModel incident, string templatePath)
    {
        _logger.LogDebug("=== NOI Generation Debug ===");
        _logger.LogDebug("Incident ID: {IncidentId}, Template: {Template}", 
            incident.IncidentNumber, templatePath);
        
        // Check template file
        if (!File.Exists(templatePath))
        {
            _logger.LogError("Template file not found: {TemplatePath}", templatePath);
            return;
        }
        
        // Check required fields
        var missingFields = new List<string>();
        if (string.IsNullOrEmpty(incident.Title)) missingFields.Add("Title");
        if (string.IsNullOrEmpty(incident.IncidentNumber)) missingFields.Add("IncidentNumber");
        if (string.IsNullOrEmpty(incident.Priority)) missingFields.Add("Priority");
        
        if (missingFields.Any())
        {
            _logger.LogError("Missing required fields for NOI: {Fields}", string.Join(", ", missingFields));
            return;
        }
        
        // Check field mappings
        var fieldMappings = BuildFieldMappings(incident);
        _logger.LogDebug("Field Mappings: {@Mappings}", fieldMappings);
        
        // Test template processing
        try
        {
            var preview = ProcessTemplate(templatePath, fieldMappings);
            _logger.LogDebug("Template processing successful. Preview length: {Length}", preview.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template processing failed: {Error}", ex.Message);
        }
    }
}
```

### Workflow State Transition Errors
```csharp
// Debug workflow transition issues
public class WorkflowTransitionDebugger
{
    public void DebugWorkflowTransition(string incidentId, IncidentStatus from, IncidentStatus to)
    {
        _logger.LogDebug("=== Workflow Transition Debug ===");
        _logger.LogDebug("Incident: {Id}, Transition: {From} -> {To}", incidentId, from, to);
        
        var isValid = _validTransitions.ContainsKey(from) && _validTransitions[from].Contains(to);
        _logger.LogDebug("Transition Valid: {Valid}", isValid);
        
        if (!isValid)
        {
            var allowedTransitions = _validTransitions.GetValueOrDefault(from, new List<IncidentStatus>());
            _logger.LogDebug("Allowed Transitions: [{Transitions}]", string.Join(", ", allowedTransitions));
        }
        
        // Check business rules
        if (to == IncidentStatus.Closed)
        {
            var incident = _repository.GetByIdAsync(incidentId).Result;
            if (incident != null)
            {
                if (string.IsNullOrEmpty(incident.ResolutionDetails))
                    _logger.LogWarning("Cannot close incident without resolution details");
                    
                if (incident.Type == IncidentType.MajorIncident && !incident.NOISent)
                    _logger.LogWarning("Cannot close Major Incident without sending NOI");
            }
        }
    }
}
```

## Layer-Specific Error Handling

### Domain Layer Errors
```csharp
// Priority calculation errors
public class PriorityMatrixService
{
    public async Task<PriorityResult> CalculatePriorityAsync(PriorityRequest request)
    {
        try
        {
            var validationResult = ValidateInput(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Priority calculation validation failed for incident {IncidentId}: {Errors}", 
                    request.IncidentId, string.Join(", ", validationResult.Errors));
                return PriorityResult.Invalid(validationResult.Errors);
            }
            
            var priority = ComputeMatrixPriority(request.Urgency, request.ImpactedUsers);
            _logger.LogDebug("Calculated priority {Priority} for incident {IncidentId}", 
                priority, request.IncidentId);
                
            return PriorityResult.Success(priority);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calculating priority for incident {IncidentId}", 
                request.IncidentId);
            throw new PriorityCalculationException("Priority calculation failed", ex);
        }
    }
}
```

### Application Layer Errors
```csharp
// Use case error handling
public class IncidentService
{
    public async Task<IncidentDto> CreateAsync(CreateIncidentRequest request)
    {
        using var scope = _logger.BeginScope("CreateIncident");
        
        try
        {
            // Validate request
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Incident creation validation failed: {Errors}", string.Join(", ", errors));
                throw new WorkflowValidationException("Validation failed") { ValidationErrors = validationResult.Errors.ToList() };
            }
            
            // Create and save incident
            var incident = new Incident(request.Title, request.Description);
            var savedIncident = await _repository.AddAsync(incident);
            
            _logger.LogInformation("Successfully created incident {IncidentId}", savedIncident.Id);
            return _mapper.Map<IncidentDto>(savedIncident);
        }
        catch (OperationPrimeException)
        {
            throw; // Re-throw known exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating incident");
            throw new IncidentCreationException("Failed to create incident", ex);
        }
    }
}
```

### Infrastructure Layer Errors
```csharp
// Database error handling
public class IncidentRepository
{
    public async Task<Incident> AddAsync(Incident incident)
    {
        try
        {
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();
            
            _logger.LogDebug("Successfully saved incident {IncidentId} to database", incident.Id);
            return incident;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error saving incident {IncidentId}: {Error}", 
                incident.Id, ex.Message);
            throw new DatabaseException("Failed to save incident to database", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected database error saving incident {IncidentId}", incident.Id);
            throw new DatabaseException("Unexpected database error", ex);
        }
    }
}

// External service error handling
public class NeuronsIntegrationService
{
    public async Task<IncidentData> FetchIncidentDataAsync(string incidentNumber)
    {
        try
        {
            var response = await _circuitBreaker.ExecuteAsync(async () =>
            {
                var token = await _tokenProvider.GetTokenAsync();
                return await _httpClient.GetAsync($"/incidents/{incidentNumber}");
            });
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<IncidentData>();
                _logger.LogInformation("Successfully fetched incident data for {IncidentNumber}", incidentNumber);
                return data;
            }
            else
            {
                _logger.LogWarning("Neurons API returned {StatusCode} for incident {IncidentNumber}", 
                    response.StatusCode, incidentNumber);
                throw new ExternalServiceException("Neurons", $"API returned {response.StatusCode}", null);
            }
        }
        catch (CircuitBreakerOpenException ex)
        {
            _logger.LogWarning("Circuit breaker open for Neurons service: {Error}", ex.Message);
            throw new ExternalServiceException("Neurons", "Service temporarily unavailable", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error fetching incident data for {IncidentNumber}", incidentNumber);
            throw new ExternalServiceException("Neurons", "Network connectivity issue", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching incident data for {IncidentNumber}", incidentNumber);
            throw new ExternalServiceException("Neurons", "Unexpected service error", ex);
        }
    }
}
```

## Complex System Debugging

### Integration Testing Failures
```bash
# Test specific business logic components
dotnet test tests/Domain.Tests/PriorityCalculationTests.cs -v detailed
dotnet test tests/Application.Tests/IncidentServiceTests.cs -v detailed
dotnet test tests/Infrastructure.Tests/RepositoryTests.cs -v detailed

# Test workflow integration
dotnet test tests/Integration.Tests/WorkflowTests.cs -v detailed
dotnet test tests/Integration.Tests/NOIGenerationTests.cs -v detailed

# Test external service integration
dotnet test tests/Integration.Tests/NeuronsIntegrationTests.cs -v detailed
```

### Performance Debugging
```csharp
// Performance monitoring for business logic
public class PerformanceMonitor
{
    public async Task<T> MonitorAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await operation();
            stopwatch.Stop();
            
            _logger.LogInformation("{Operation} completed in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
                
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("{Operation} took longer than expected: {ElapsedMs}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{Operation} failed after {ElapsedMs}ms: {Error}", 
                operationName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
```

---
*Focus: Rapid resolution | Prevent recurring issues | Maintain development velocity*
