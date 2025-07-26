# OPERATION PRIME Coding Standards & Best Practices

**Keywords**: coding standards, clean code, SOLID principles, MVVM patterns, component isolation, verification strategies, quality assurance

## Table of Contents

1. [Core Coding Principles](#core-coding-principles)
   - [SOLID Principles Application](#solid-principles-application)
     - [Single Responsibility Principle (SRP)](#single-responsibility-principle-srp)
     - [Open/Closed Principle (OCP)](#openclosed-principle-ocp)
     - [Dependency Inversion Principle (DIP)](#dependency-inversion-principle-dip)
2. [Clean Code Guidelines](#clean-code-guidelines)
   - [Naming Conventions](#naming-conventions)
     - [Classes and Methods](#classes-and-methods)
     - [Variables and Properties](#variables-and-properties)
   - [Method Complexity Rules](#method-complexity-rules)
     - [Single Purpose Methods](#single-purpose-methods)
3. [Component Isolation & Anti-Reuse Patterns](#component-isolation--anti-reuse-patterns)
   - [Avoid Component Code Reuse](#avoid-component-code-reuse)
     - [Create Dedicated Components](#create-dedicated-components)
     - [Separate Service Implementations](#separate-service-implementations)
4. [Complex System Implementation Rules](#complex-system-implementation-rules)
   - [Priority Calculation System](#priority-calculation-system)
     - [Layer Separation](#layer-separation)
   - [NOI Generation System](#noi-generation-system)
     - [Builder Pattern Implementation](#builder-pattern-implementation)
5. [Verification & Documentation Strategies](#verification--documentation-strategies)
   - [Complex Plan Execution Checklist](#complex-plan-execution-checklist)
     - [Pre-Implementation Verification](#pre-implementation-verification)
     - [Implementation Evidence Documentation](#implementation-evidence-documentation)
   - [Anti-Assumption Patterns](#anti-assumption-patterns)
     - [Explicit Requirement Verification](#explicit-requirement-verification)
6. [MVVM Architecture Standards](#mvvm-architecture-standards)
   - [ViewModel Implementation Rules](#viewmodel-implementation-rules)
     - [Property Change Handling](#property-change-handling)
7. [Error Handling & Resilience Patterns](#error-handling--resilience-patterns)
   - [Circuit Breaker Implementation](#circuit-breaker-implementation)
8. [Testing & Quality Assurance](#testing--quality-assurance)
   - [Unit Testing Standards](#unit-testing-standards)

## Core Coding Principles

### SOLID Principles Application

#### Single Responsibility Principle (SRP)
```csharp
// ✅ GOOD - Each service has one responsibility
public class PriorityCalculationService
{
    public string CalculatePriority(UrgencyEnum urgency, int impactedUsers) { }
}

public class PriorityValidationService
{
    public ValidationResult ValidateInputs(string urgency, int impactedUsers) { }
}

// ❌ BAD - Multiple responsibilities in one class
public class PriorityManager
{
    public string CalculatePriority() { }
    public ValidationResult ValidateInputs() { }
    public void SaveToDatabase() { }
    public void SendNotification() { }
}
```

#### Open/Closed Principle (OCP)
```csharp
// ✅ GOOD - Extensible without modification
public abstract class IncidentBase
{
    public abstract string GenerateReport();
}

public class PreIncident : IncidentBase
{
    public override string GenerateReport() => "Pre-incident report format";
}

public class MajorIncident : IncidentBase
{
    public override string GenerateReport() => "Major incident report format";
}
```

#### Dependency Inversion Principle (DIP)
```csharp
// ✅ GOOD - Depend on abstractions
public class IncidentService
{
    private readonly IIncidentRepository _repository;
    private readonly IAuditService _auditService;
    
    public IncidentService(IIncidentRepository repository, IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }
}

// ❌ BAD - Direct dependency on concrete classes
public class IncidentService
{
    private readonly SqliteIncidentRepository _repository = new();
    private readonly FileAuditService _auditService = new();
}
```

## Clean Code Guidelines

### Naming Conventions

#### Classes and Methods
```csharp
// ✅ GOOD - Clear, descriptive names
public class IncidentPriorityCalculationService
{
    public PriorityResult CalculateBasedOnUrgencyAndImpact(UrgencyLevel urgency, int userCount)
    {
        // Implementation
    }
}

// ❌ BAD - Abbreviated, unclear names
public class IncPrioCalcSvc
{
    public PrioRes Calc(int urg, int cnt)
    {
        // Implementation
    }
}
```

#### Variables and Properties
```csharp
// ✅ GOOD - Descriptive variable names
public class IncidentViewModel
{
    public string IncidentTitle { get; set; }
    public DateTime IncidentStartTime { get; set; }
    public int ImpactedUserCount { get; set; }
    
    private void UpdatePriorityBasedOnUserInput()
    {
        var calculatedPriority = _priorityService.Calculate(Urgency, ImpactedUserCount);
        var validationResult = _validator.ValidateCalculation(calculatedPriority);
        
        if (validationResult.IsValid)
        {
            IncidentPriority = calculatedPriority.Value;
        }
    }
}

// ❌ BAD - Cryptic variable names
public class IncidentViewModel
{
    public string t { get; set; }
    public DateTime st { get; set; }
    public int uc { get; set; }
    
    private void UpdPrio()
    {
        var cp = _ps.Calc(u, uc);
        var vr = _v.Val(cp);
        if (vr.ok) p = cp.v;
    }
}
```

### Method Complexity Rules

#### Single Purpose Methods
```csharp
// ✅ GOOD - Each method has one clear purpose
public class NOIBuilder
{
    public NOIBuilder WithIncidentData(IncidentViewModel incident)
    {
        ValidateIncidentData(incident);
        _incident = incident;
        return this;
    }
    
    public NOIBuilder WithTimestamps()
    {
        AddCreationTimestamp();
        AddLastUpdatedTimestamp();
        return this;
    }
    
    public string GeneratePreview()
    {
        var template = LoadTemplate();
        var populatedTemplate = PopulateTemplate(template);
        return FormatForPreview(populatedTemplate);
    }
    
    private void ValidateIncidentData(IncidentViewModel incident)
    {
        if (incident == null) throw new ArgumentNullException(nameof(incident));
        if (string.IsNullOrEmpty(incident.Title)) throw new ArgumentException("Title required");
    }
    
    private void AddCreationTimestamp()
    {
        _templateData["CreationTime"] = DateTime.Now.ToString("dddd, MMMM dd, yyyy, hh:mm tt ET");
    }
}

// ❌ BAD - Method doing too many things
public string GenerateNOICompleteProcess(IncidentViewModel incident, bool sendEmail, string[] recipients)
{
    // Validation
    if (incident == null) throw new ArgumentNullException();
    if (string.IsNullOrEmpty(incident.Title)) throw new ArgumentException();
    
    // Template loading
    var template = File.ReadAllText("template.txt");
    
    // Data population
    template = template.Replace("{title}", incident.Title);
    template = template.Replace("{time}", DateTime.Now.ToString());
    
    // Preview generation
    var preview = $"PREVIEW:\n{template}";
    
    // File export
    File.WriteAllText("noi.txt", template);
    
    // Email sending
    if (sendEmail)
    {
        foreach (var recipient in recipients)
        {
            // Email logic
        }
    }
    
    // Audit logging
    _auditService.Log("NOI Generated");
    
    return preview;
}
```

## Component Isolation & Anti-Reuse Patterns

### Avoid Component Code Reuse

#### Unified Wizard with Conditional Fields
```csharp
// ✅ GOOD - Single wizard ViewModel with conditional logic
public partial class IncidentWizardViewModel : WizardViewModelBase<IncidentWizardViewModel>
{
    private readonly IPreIncidentWorkflowService _preWorkflow;
    private readonly IMajorIncidentWorkflowService _majorWorkflow;

    public PreIncidentViewModel PreIncident { get; }
    public MajorIncidentViewModel MajorIncident { get; }

    public IncidentType IncidentType { get; private set; }

    public IncidentWizardViewModel(
        IPreIncidentWorkflowService preWorkflow,
        IMajorIncidentWorkflowService majorWorkflow,
        PreIncidentViewModel preIncident,
        MajorIncidentViewModel majorIncident,
        ILogger<IncidentWizardViewModel> logger)
        : base(logger)
    {
        _preWorkflow = preWorkflow;
        _majorWorkflow = majorWorkflow;
        PreIncident = preIncident;
        MajorIncident = majorIncident;
    }

    public void Initialize(IncidentType type)
    {
        IncidentType = type;
        Logger.LogDebug("Wizard initialized for {Type}", type);
    }

    protected override ValidationResult ValidateStep(int stepIndex)
    {
        return IncidentType switch
        {
            IncidentType.PreIncident => ValidatePreIncidentStep(stepIndex),
            IncidentType.MajorIncident => ValidateMajorIncidentStep(stepIndex),
            _ => ValidationResult.Success
        };
    }
}

// ❌ BAD - Duplicating entire wizard ViewModels
public class PreIncidentWizardViewModel : WizardViewModelBase { /* duplicate fields */ }
public class MajorIncidentWizardViewModel : WizardViewModelBase { /* duplicate fields */ }
```

#### Separate Service Implementations
```csharp
// ✅ GOOD - Dedicated services
public class PreIncidentService : IIncidentService
{
    public async Task<PreIncident> CreateAsync(PreIncidentDto dto)
    {
        var incident = new PreIncident
        {
            Title = dto.Title,
            Source = dto.Source,
            // Pre-incident specific mapping
        };
        
        await _repository.AddAsync(incident);
        await _auditService.LogCreationAsync(incident);
        
        return incident;
    }
}

public class MajorIncidentService : IIncidentService
{
    public async Task<MajorIncident> CreateAsync(MajorIncidentDto dto)
    {
        var incident = new MajorIncident
        {
            Title = dto.Title,
            BusinessImpact = dto.BusinessImpact,
            RequiresNOI = true,
            // Major incident specific mapping
        };
        
        await _repository.AddAsync(incident);
        await _auditService.LogCreationAsync(incident);
        await _noiService.QueueGenerationAsync(incident);
        
        return incident;
    }
}

// ❌ BAD - Generic service with type switching
public class GenericIncidentService
{
    public async Task<object> CreateIncidentAsync(object dto, string type)
    {
        if (type == "Pre")
        {
            var preDto = (PreIncidentDto)dto;
            var incident = new PreIncident { /* mapping */ };
            // Pre-specific logic
            return incident;
        }
        else if (type == "Major")
        {
            var majorDto = (MajorIncidentDto)dto;
            var incident = new MajorIncident { /* mapping */ };
            // Major-specific logic
            return incident;
        }
        
        throw new ArgumentException("Unknown incident type");
    }
}
```

## Complex System Implementation Rules

### Priority Calculation System

#### Layer Separation
```csharp
// ✅ GOOD - Clear layer separation
public class PriorityCalculationOrchestrator
{
    private readonly IPriorityValidationService _validator;
    private readonly IPriorityMatrixService _matrix;
    private readonly IPriorityOverrideService _override;
    private readonly IPriorityAuditService _audit;
    
    public async Task<PriorityResult> CalculateAsync(PriorityRequest request)
    {
        // Layer 1: Validation
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return PriorityResult.Invalid(validationResult.Errors);
        }
        
        // Layer 2: Matrix Calculation
        var matrixResult = await _matrix.CalculateAsync(request.Urgency, request.ImpactedUsers);
        
        // Layer 3: Override Application
        var finalResult = await _override.ApplyAsync(matrixResult, request.OverridePriority);
        
        // Layer 4: Audit Logging
        await _audit.LogCalculationAsync(request, finalResult);
        
        return finalResult;
    }
}

// Each layer is a separate, testable service
public class PriorityValidationService : IPriorityValidationService
{
    public async Task<ValidationResult> ValidateAsync(PriorityRequest request)
    {
        var errors = new List<string>();
        
        if (!Enum.IsDefined(typeof(UrgencyLevel), request.Urgency))
            errors.Add("Invalid urgency level");
            
        if (request.ImpactedUsers < 0 || request.ImpactedUsers > 10000)
            errors.Add("Impacted users must be between 0 and 10,000");
            
        if (request.IncidentType == IncidentType.PreIncident && request.ImpactedUsers > 100)
            errors.Add("Pre-incidents cannot exceed 100 impacted users");
            
        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}
```

### NOI Generation System

#### Builder Pattern Implementation
```csharp
// ✅ GOOD - Proper builder pattern with validation
public class NOIBuilder
{
    private IncidentViewModel _incident;
    private Dictionary<string, object> _templateData = new();
    private string _templateType;
    private bool _isBuilt = false;
    
    public NOIBuilder ForIncident(IncidentViewModel incident)
    {
        _incident = incident ?? throw new ArgumentNullException(nameof(incident));
        ValidateIncidentForNOI(incident);
        return this;
    }
    
    public NOIBuilder WithTemplate(string templateType)
    {
        _templateType = templateType ?? throw new ArgumentNullException(nameof(templateType));
        ValidateTemplateExists(templateType);
        return this;
    }
    
    public NOIBuilder AddCustomData(string key, object value)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be empty");
        _templateData[key] = value;
        return this;
    }
    
    public NOIDocument Build()
    {
        ValidateBuilderState();
        
        var document = new NOIDocument
        {
            IncidentId = _incident.IncidentId,
            GeneratedAt = DateTime.Now,
            Content = RenderTemplate(),
            TemplateType = _templateType
        };
        
        _isBuilt = true;
        return document;
    }
    
    private void ValidateBuilderState()
    {
        if (_incident == null) throw new InvalidOperationException("Incident must be set");
        if (string.IsNullOrEmpty(_templateType)) throw new InvalidOperationException("Template type must be set");
        if (_isBuilt) throw new InvalidOperationException("Builder can only be used once");
    }
    
    private void ValidateIncidentForNOI(IncidentViewModel incident)
    {
        if (incident.IncidentType != IncidentType.Major)
            throw new ArgumentException("NOI can only be generated for Major incidents");
            
        if (string.IsNullOrEmpty(incident.BusinessImpact))
            throw new ArgumentException("Business impact is required for NOI generation");
    }
}
```

## Verification & Documentation Strategies

### Complex Plan Execution Checklist

#### Pre-Implementation Verification
```csharp
/// <summary>
/// VERIFICATION CHECKLIST - Complete before claiming implementation is done
/// 
/// Phase 1 Foundation Requirements:
/// □ Project structure created with exact folder hierarchy
/// □ All NuGet packages installed and verified working
/// □ Base models (Incident, PreIncident, MajorIncident) implemented
/// □ Database context configured with SQLCipher encryption
/// □ MVVM foundation classes created and tested
/// 
/// Evidence Required:
/// - Screenshot of Solution Explorer showing folder structure
/// - Build output showing successful compilation
/// - Unit test results for base models
/// - Database connection test results
/// 
/// DO NOT PROCEED TO PHASE 2 UNTIL ALL ITEMS VERIFIED
/// </summary>
public class Phase1VerificationChecklist
{
    // Implementation verification methods
    public static bool VerifyProjectStructure()
    {
        var requiredFolders = new[] { "Models", "Views", "ViewModels", "Services", "Data", "Utils" };
        return requiredFolders.All(folder => Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder)));
    }
    
    public static bool VerifyDatabaseConnection()
    {
        try
        {
            using var context = new OperationPrimeDbContext();
            return context.Database.CanConnect();
        }
        catch
        {
            return false;
        }
    }
}
```

#### Implementation Evidence Documentation
```csharp
/// <summary>
/// EVIDENCE DOCUMENTATION - Prove completion with specific examples
/// 
/// For Priority Calculation System:
/// CLAIM: "Priority calculation system implemented"
/// EVIDENCE REQUIRED:
/// - Show PriorityCalculationService.cs file exists
/// - Show unit test results for all matrix combinations
/// - Show integration test with ViewModel property binding
/// - Show audit log entries for priority calculations
/// 
/// For NOI Generation:
/// CLAIM: "NOI generation working"
/// EVIDENCE REQUIRED:
/// - Show generated NOI document sample
/// - Show email integration test results
/// - Show template rendering with real incident data
/// - Show error handling for invalid incidents
/// </summary>
public class ImplementationEvidence
{
    public static class PrioritySystemEvidence
    {
        // Prove priority calculation works
        public static void DemonstrateCalculation()
        {
            var service = new PriorityCalculationService();
            
            // Test all matrix combinations
            var testCases = new[]
            {
                (UrgencyLevel.High, 10, "P2"),
                (UrgencyLevel.High, 500, "P1"),
                (UrgencyLevel.Low, 10, "P4")
            };
            
            foreach (var (urgency, users, expected) in testCases)
            {
                var result = service.Calculate(urgency, users);
                Debug.Assert(result == expected, $"Expected {expected}, got {result}");
            }
        }
    }
}
```

### Anti-Assumption Patterns

#### Explicit Requirement Verification
```csharp
/// <summary>
/// RED FLAG DETECTION - Stop and verify when these phrases appear:
/// - "mostly complete" → STOP: Define what's missing
/// - "largely achieved" → STOP: List specific gaps  
/// - "should work" → STOP: Test and prove it works
/// - "similar to" → STOP: Check exact requirements
/// </summary>
public static class RequirementVerification
{
    public static void VerifySpecificRequirement(string requirement, Func<bool> verificationTest)
    {
        Console.WriteLine($"VERIFYING: {requirement}");
        
        var result = verificationTest();
        
        if (result)
        {
            Console.WriteLine($"✅ VERIFIED: {requirement}");
        }
        else
        {
            Console.WriteLine($"❌ FAILED: {requirement}");
            throw new RequirementNotMetException($"Requirement not met: {requirement}");
        }
    }
    
    // Example usage
    public static void VerifyPriorityMatrixRequirements()
    {
        VerifySpecificRequirement(
            "Priority matrix calculates P1 for High urgency + 500+ users",
            () => new PriorityCalculationService().Calculate(UrgencyLevel.High, 500) == "P1"
        );
        
        VerifySpecificRequirement(
            "Priority matrix handles Pre-incident user limits",
            () => {
                try
                {
                    new PriorityCalculationService().Calculate(UrgencyLevel.High, 150, IncidentType.PreIncident);
                    return false; // Should have thrown exception
                }
                catch (ArgumentException)
                {
                    return true; // Expected exception
                }
            }
        );
    }
}
```

## MVVM Architecture Standards

### ViewModel Implementation Rules

#### Modern MVVM Pattern (2025+)
- All ViewModels must inherit from `BaseViewModel : ObservableValidator` (CommunityToolkit.Mvvm v8.4+).
- Use `[ObservableProperty]` partials for observable properties (supports protected set, required, null-annotations, etc.).
- Uses `Directory.Build.props` with `<LangVersion>preview</LangVersion>` for partial property support and future-proofing across all projects.
- MVVM Toolkit analyzers must be enabled to enforce correct usage and surface errors/warnings.
- Use `[RelayCommand]` and `IAsyncRelayCommand` for commands; use `IMessenger` for decoupled messaging between ViewModels.
- Validation is handled via data annotations and toolkit support—no need to manually implement `INotifyDataErrorInfo`.

```csharp
// ✅ GOOD - Modern MVVM with CommunityToolkit
public partial class IncidentViewModel : BaseViewModel
{
    [ObservableProperty]
    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
    private string _title = string.Empty;
    
    [ObservableProperty]
    [Range(1, 10000, ErrorMessage = "Impacted users must be between 1 and 10,000")]
    private int _impactedUsers;
    
    [ObservableProperty]
    private string _calculatedPriority = string.Empty;
    
    // Reactive property changes
    partial void OnImpactedUsersChanged(int oldValue, int newValue)
    {
        RecalculatePriority();
        ValidateProperty(newValue, nameof(ImpactedUsers));
    }
    
    partial void OnTitleChanged(string oldValue, string newValue)
    {
        ValidateProperty(newValue, nameof(Title));
    }
    
    private void RecalculatePriority()
    {
        if (Urgency != UrgencyLevel.None && ImpactedUsers > 0)
        {
            CalculatedPriority = _priorityService.Calculate(Urgency, ImpactedUsers, IncidentType);
            
            // Notify other components
            WeakReferenceMessenger.Default.Send(new PriorityCalculatedMessage
            {
                IncidentId = IncidentId,
                NewPriority = CalculatedPriority
            });
        }
    }
}

// ❌ BAD - Manual property implementation with potential issues
public class IncidentViewModel : INotifyPropertyChanged
{
    private string _title;
    private int _impactedUsers;
    
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            // Missing validation
            // Missing null checks
            // No old/new value comparison
        }
    }
    
    public int ImpactedUsers
    {
        get => _impactedUsers;
        set
        {
            _impactedUsers = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImpactedUsers)));
            // Should recalculate priority but missing
            // No validation
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
}
```

## Error Handling & Resilience Patterns

### Circuit Breaker Implementation
```csharp
// ✅ GOOD - Proper circuit breaker for external dependencies
public class NeuronsIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly CircuitBreakerPolicy _circuitBreaker;
    private readonly ILogger<NeuronsIntegrationService> _logger;
    
    public NeuronsIntegrationService(HttpClient httpClient, ILogger<NeuronsIntegrationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _circuitBreaker = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, duration) =>
                {
                    _logger.LogWarning("Circuit breaker opened for {Duration}ms due to {Exception}", 
                        duration.TotalMilliseconds, exception.Message);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker reset - connection restored");
                });
    }
    
    public async Task<IncidentDetailsDto?> GetIncidentDetailsAsync(string incidentNumber)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"/api/odata/businessobject/Incidents?filter=IncidentNumber eq '{incidentNumber}'");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IncidentDetailsDto>(json);
            });
        }
        catch (CircuitBreakerOpenException)
        {
            _logger.LogWarning("Neurons service unavailable - circuit breaker open");
            return null; // Graceful degradation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch incident details for {IncidentNumber}", incidentNumber);
            return null;
        }
    }
}
```

## Testing & Quality Assurance

### Unit Testing Standards
```csharp
// ✅ GOOD - Comprehensive unit tests with clear naming
[TestClass]
public class PriorityCalculationServiceTests
{
    private PriorityCalculationService _service;
    private Mock<IPriorityMatrixService> _mockMatrix;
    private Mock<IPriorityValidationService> _mockValidator;
    
    [TestInitialize]
    public void Setup()
    {
        _mockMatrix = new Mock<IPriorityMatrixService>();
        _mockValidator = new Mock<IPriorityValidationService>();
        _service = new PriorityCalculationService(_mockMatrix.Object, _mockValidator.Object);
    }
    
    [TestMethod]
    public void Calculate_WithHighUrgencyAnd500Users_ReturnsP1()
    {
        // Arrange
        var urgency = UrgencyLevel.High;
        var impactedUsers = 500;
        var expected = "P1";
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<PriorityRequest>()))
            .ReturnsAsync(ValidationResult.Success);
        _mockMatrix.Setup(m => m.CalculateAsync(urgency, impactedUsers))
            .ReturnsAsync(expected);
        
        // Act
        var result = _service.Calculate(urgency, impactedUsers);
        
        // Assert
        Assert.AreEqual(expected, result);
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<PriorityRequest>()), Times.Once);
        _mockMatrix.Verify(m => m.CalculateAsync(urgency, impactedUsers), Times.Once);
    }
    
    [TestMethod]
    public void Calculate_WithInvalidInput_ThrowsArgumentException()
    {
        // Arrange
        var urgency = (UrgencyLevel)999; // Invalid enum value
        var impactedUsers = -1; // Invalid count
        
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<PriorityRequest>()))
            .ReturnsAsync(new ValidationResult { IsValid = false, Errors = new[] { "Invalid input" } });
        
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _service.Calculate(urgency, impactedUsers));
    }
}
```

---

These coding standards ensure maintainable, testable, and reliable code for the OPERATION PRIME project while preventing common pitfalls and promoting clean architecture principles.
