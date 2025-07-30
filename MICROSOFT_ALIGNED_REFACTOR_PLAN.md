# Microsoft-Aligned Refactoring Plan for IncidentCreateViewModel

## 📚 **Based on Microsoft Learn 2024 Documentation**

This refactoring plan aligns with the official Microsoft documentation for:
- [MVVM Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Model-View-ViewModel Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Dependency Injection Guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines)
- [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

## 🎯 **Microsoft's 2024 Recommended Architecture**

### **✅ Core Principles:**
1. **Single Inheritance**: ViewModels inherit from one base class (`ObservableValidator`)
2. **Service Injection**: Business logic delegated to injected services
3. **UI Properties in ViewModel**: All binding properties remain in ViewModel
4. **Composition Over Inheritance**: Use services and interfaces for shared functionality

### **✅ Current State Analysis:**
- ✅ **`BaseValidatingViewModel`**: Already perfectly aligned with Microsoft patterns
- ✅ **Service Injection**: Current DI pattern is correct
- ✅ **[ObservableProperty] Usage**: Follows Microsoft's recommended approach
- ❌ **Business Logic in ViewModel**: Needs extraction to services

---

## 🚀 **Refactoring Strategy**

### **Phase 1: Service Layer Extraction (Low Risk)**
Extract business logic to services while keeping all UI properties in ViewModel.

### **Phase 2: Service Integration (Medium Risk)**
Update ViewModel to delegate business operations to services.

### **Phase 3: Testing & Validation (Low Risk)**
Comprehensive testing to ensure functionality preservation.

---

## 📋 **Step-by-Step Implementation Plan**

### **Step 1: Extract Validation Service**

#### **1.1 Create Interface**
```csharp
// File: Application/Interfaces/IIncidentValidationService.cs
public interface IIncidentValidationService
{
    bool ValidateStep1(IncidentType incidentType);
    bool ValidateStep2(string title, string description, DateTime? timeIssueStarted, 
                      DateTime? timeReported, int? impactedUsers, string applicationAffected, 
                      string locationsAffected);
    bool ValidateStep3(string title, string description, DateTime? timeIssueStarted, 
                      DateTime? timeReported, int? impactedUsers, string applicationAffected, 
                      string locationsAffected);
    bool ValidateStep4(bool isMajorIncident, string businessImpact);
    bool ValidateCurrentStep(int currentStep, IncidentFormData formData);
    bool CanCreateIncident(IncidentFormData formData, int currentStep, int totalSteps);
}
```

#### **1.2 Create Implementation**
```csharp
// File: Application/Services/IncidentValidationService.cs
public class IncidentValidationService : IIncidentValidationService
{
    public bool ValidateStep1(IncidentType incidentType)
    {
        return incidentType != default;
    }

    public bool ValidateStep2(string title, string description, DateTime? timeIssueStarted, 
                             DateTime? timeReported, int? impactedUsers, string applicationAffected, 
                             string locationsAffected)
    {
        return ValidateCommonRequiredFields(timeIssueStarted, timeReported, impactedUsers, 
                                          applicationAffected, locationsAffected);
    }

    public bool ValidateStep3(string title, string description, DateTime? timeIssueStarted, 
                             DateTime? timeReported, int? impactedUsers, string applicationAffected, 
                             string locationsAffected)
    {
        return !string.IsNullOrWhiteSpace(title) &&
               !string.IsNullOrWhiteSpace(description) &&
               ValidateCommonRequiredFields(timeIssueStarted, timeReported, impactedUsers, 
                                          applicationAffected, locationsAffected);
    }

    public bool ValidateStep4(bool isMajorIncident, string businessImpact)
    {
        if (!isMajorIncident) return true;
        return !string.IsNullOrWhiteSpace(businessImpact);
    }

    public bool ValidateCurrentStep(int currentStep, IncidentFormData formData)
    {
        return currentStep switch
        {
            1 => ValidateStep1(formData.IncidentType),
            2 => ValidateStep2(formData.Title, formData.Description, formData.TimeIssueStarted,
                              formData.TimeReported, formData.ImpactedUsers, formData.ApplicationAffected,
                              formData.LocationsAffected),
            3 => ValidateStep3(formData.Title, formData.Description, formData.TimeIssueStarted,
                              formData.TimeReported, formData.ImpactedUsers, formData.ApplicationAffected,
                              formData.LocationsAffected),
            4 => ValidateStep4(formData.IsMajorIncident, formData.BusinessImpact),
            _ => false
        };
    }

    public bool CanCreateIncident(IncidentFormData formData, int currentStep, int totalSteps)
    {
        if (currentStep != totalSteps) return false;

        for (int step = 1; step <= totalSteps; step++)
        {
            if (!ValidateCurrentStep(step, formData)) return false;
        }

        return true;
    }

    private bool ValidateCommonRequiredFields(DateTime? timeIssueStarted, DateTime? timeReported, 
                                            int? impactedUsers, string applicationAffected, 
                                            string locationsAffected)
    {
        return timeIssueStarted.HasValue &&
               timeReported.HasValue &&
               impactedUsers.HasValue &&
               !string.IsNullOrWhiteSpace(applicationAffected) &&
               !string.IsNullOrWhiteSpace(locationsAffected);
    }
}
```

#### **1.3 Create Form Data DTO**
```csharp
// File: Application/DTOs/IncidentFormData.cs
public class IncidentFormData
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentType IncidentType { get; set; } = IncidentType.PreIncident;
    public Priority Priority { get; set; } = Priority.P3;
    public Status Status { get; set; } = Status.New;
    public string BusinessImpact { get; set; } = string.Empty;
    public DateTime? TimeIssueStarted { get; set; }
    public DateTime? TimeReported { get; set; }
    public int? ImpactedUsers { get; set; }
    public string ApplicationAffected { get; set; } = string.Empty;
    public string LocationsAffected { get; set; } = string.Empty;
    public string Workaround { get; set; } = string.Empty;
    public string IncidentNumber { get; set; } = string.Empty;
    public int Urgency { get; set; } = 3;
    public bool IsMajorIncident => IncidentType == IncidentType.MajorIncident;
}
```

#### **1.4 Register Service in DI**
```csharp
// File: App.xaml.cs (in ConfigureServices method)
services.AddScoped<IIncidentValidationService, IncidentValidationService>();
```

#### **1.5 Update ViewModel Constructor**
```csharp
// File: Presentation/ViewModels/IncidentCreateViewModel.cs
public IncidentCreateViewModel(
    IIncidentService incidentService, 
    IEnumService enumService, 
    IApplicationService applicationService,
    IIncidentValidationService validationService) // NEW
{
    _incidentService = incidentService;
    _enumService = enumService;
    _applicationService = applicationService;
    _validationService = validationService; // NEW
    
    LoadEnumCollections();
    RefreshValidationState();
}
```

#### **1.6 Update Validation Methods in ViewModel**
```csharp
// Replace existing validation methods with service calls
private bool ValidateCurrentStep()
{
    var formData = CreateFormData();
    return _validationService.ValidateCurrentStep(CurrentStep, formData);
}

private bool ValidateStep1() => _validationService.ValidateStep1(IncidentType);
private bool ValidateStep2() => _validationService.ValidateStep2(Title, Description, TimeIssueStarted, TimeReported, ImpactedUsers, ApplicationAffected, LocationsAffected);
private bool ValidateStep3() => _validationService.ValidateStep3(Title, Description, TimeIssueStarted, TimeReported, ImpactedUsers, ApplicationAffected, LocationsAffected);
private bool ValidateStep4() => _validationService.ValidateStep4(IsMajorIncident, BusinessImpact);

private bool CanCreateIncident()
{
    var formData = CreateFormData();
    return _validationService.CanCreateIncident(formData, CurrentStep, TotalSteps) && !IsSubmitting;
}

private IncidentFormData CreateFormData()
{
    return new IncidentFormData
    {
        Title = Title,
        Description = Description,
        IncidentType = IncidentType,
        Priority = Priority,
        Status = Status,
        BusinessImpact = BusinessImpact,
        TimeIssueStarted = TimeIssueStarted,
        TimeReported = TimeReported,
        ImpactedUsers = ImpactedUsers,
        ApplicationAffected = ApplicationAffected,
        LocationsAffected = LocationsAffected,
        Workaround = Workaround,
        IncidentNumber = IncidentNumber,
        Urgency = Urgency
    };
}
```

#### **1.7 Testing Checklist**
- [ ] Build compiles without errors
- [ ] All validation methods work identically
- [ ] Button states update correctly
- [ ] Form validation behaves the same
- [ ] No regression in user experience

---

### **Step 2: Extract Workflow Service**

#### **2.1 Create Interface**
```csharp
// File: Application/Interfaces/IIncidentWorkflowService.cs
public interface IIncidentWorkflowService
{
    int CalculateTotalSteps(IncidentType incidentType);
    bool CanGoToNextStep(int currentStep, int totalSteps, IncidentFormData formData);
    bool CanGoToPreviousStep(int currentStep);
    bool IsLastStep(int currentStep, int totalSteps);
    bool ShowNextButton(int currentStep, int totalSteps);
    IReadOnlyList<BreadcrumbItem> GenerateBreadcrumbItems(IncidentType incidentType);
    int CalculateBreadcrumbIndex(int currentStep);
}
```

#### **2.2 Create Implementation**
```csharp
// File: Application/Services/IncidentWorkflowService.cs
public class IncidentWorkflowService : IIncidentWorkflowService
{
    private readonly IIncidentValidationService _validationService;

    public IncidentWorkflowService(IIncidentValidationService validationService)
    {
        _validationService = validationService;
    }

    public int CalculateTotalSteps(IncidentType incidentType)
    {
        return incidentType == IncidentType.MajorIncident ? 4 : 3;
    }

    public bool CanGoToNextStep(int currentStep, int totalSteps, IncidentFormData formData)
    {
        return currentStep < totalSteps && _validationService.ValidateCurrentStep(currentStep, formData);
    }

    public bool CanGoToPreviousStep(int currentStep)
    {
        return currentStep > 1;
    }

    public bool IsLastStep(int currentStep, int totalSteps)
    {
        return currentStep == totalSteps;
    }

    public bool ShowNextButton(int currentStep, int totalSteps)
    {
        return currentStep < totalSteps;
    }

    public IReadOnlyList<BreadcrumbItem> GenerateBreadcrumbItems(IncidentType incidentType)
    {
        var items = new List<BreadcrumbItem>
        {
            new() { Text = "Type", IsEnabled = true },
            new() { Text = "Basic Info", IsEnabled = true },
            new() { Text = "Details", IsEnabled = true }
        };

        if (incidentType == IncidentType.MajorIncident)
        {
            items.Add(new BreadcrumbItem { Text = "Checklist", IsEnabled = true });
        }

        return items.AsReadOnly();
    }

    public int CalculateBreadcrumbIndex(int currentStep)
    {
        return Math.Max(0, currentStep - 1);
    }
}

public class BreadcrumbItem
{
    public string Text { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
```

#### **2.3 Register Service and Update ViewModel**
```csharp
// Register in DI
services.AddScoped<IIncidentWorkflowService, IncidentWorkflowService>();

// Update ViewModel constructor
public IncidentCreateViewModel(
    IIncidentService incidentService, 
    IEnumService enumService, 
    IApplicationService applicationService,
    IIncidentValidationService validationService,
    IIncidentWorkflowService workflowService) // NEW
{
    // ... existing code ...
    _workflowService = workflowService; // NEW
}

// Update workflow properties to use service
public int TotalSteps => _workflowService.CalculateTotalSteps(IncidentType);
public bool CanGoNext => _workflowService.CanGoToNextStep(CurrentStep, TotalSteps, CreateFormData());
public bool CanGoPrevious => _workflowService.CanGoToPreviousStep(CurrentStep);
public bool IsLastStep => _workflowService.IsLastStep(CurrentStep, TotalSteps);
public bool ShowNextButton => _workflowService.ShowNextButton(CurrentStep, TotalSteps);
public IReadOnlyList<BreadcrumbItem> BreadcrumbItems => _workflowService.GenerateBreadcrumbItems(IncidentType);
public int CurrentBreadcrumbIndex => _workflowService.CalculateBreadcrumbIndex(CurrentStep);
```

#### **2.4 Testing Checklist**
- [ ] Step navigation works identically
- [ ] Breadcrumb navigation functions correctly
- [ ] Button states update properly
- [ ] Workflow logic preserved
- [ ] No regression in step transitions

---

### **Step 3: Extract Data Mapping Service**

#### **3.1 Create Interface**
```csharp
// File: Application/Interfaces/IIncidentDataMappingService.cs
public interface IIncidentDataMappingService
{
    Incident MapFormDataToEntity(IncidentFormData formData);
    Task<Incident> CreateIncidentAsync(IncidentFormData formData, CancellationToken cancellationToken = default);
}
```

#### **3.2 Create Implementation**
```csharp
// File: Application/Services/IncidentDataMappingService.cs
public class IncidentDataMappingService : IIncidentDataMappingService
{
    private readonly IIncidentService _incidentService;

    public IncidentDataMappingService(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    public Incident MapFormDataToEntity(IncidentFormData formData)
    {
        return new Incident
        {
            Title = formData.Title,
            Description = formData.Description,
            IncidentType = formData.IncidentType,
            Priority = formData.Priority,
            Status = formData.Status,
            BusinessImpact = formData.BusinessImpact,
            TimeIssueStarted = formData.TimeIssueStarted,
            TimeReported = formData.TimeReported,
            ImpactedUsers = formData.ImpactedUsers,
            ApplicationAffected = formData.ApplicationAffected,
            LocationsAffected = formData.LocationsAffected,
            Workaround = formData.Workaround,
            IncidentNumber = formData.IncidentNumber,
            Urgency = formData.Urgency,
            CreatedDate = DateTimeOffset.UtcNow
        };
    }

    public async Task<Incident> CreateIncidentAsync(IncidentFormData formData, CancellationToken cancellationToken = default)
    {
        var incident = MapFormDataToEntity(formData);
        return await _incidentService.CreateAsync(incident, cancellationToken);
    }
}
```

#### **3.3 Update ViewModel**
```csharp
// Register service
services.AddScoped<IIncidentDataMappingService, IncidentDataMappingService>();

// Update ViewModel constructor
public IncidentCreateViewModel(
    IIncidentService incidentService, 
    IEnumService enumService, 
    IApplicationService applicationService,
    IIncidentValidationService validationService,
    IIncidentWorkflowService workflowService,
    IIncidentDataMappingService dataMappingService) // NEW
{
    // ... existing code ...
    _dataMappingService = dataMappingService; // NEW
}

// Simplify CreateIncidentAsync method
[RelayCommand(CanExecute = nameof(CanCreateIncident), IncludeCancelCommand = true)]
private async Task CreateIncidentAsync(CancellationToken cancellationToken)
{
    try
    {
        IsSubmitting = true;
        ClearMessages();
        
        var formData = CreateFormData();
        if (!_validationService.CanCreateIncident(formData, CurrentStep, TotalSteps))
        {
            SetErrorMessage("Please correct the validation errors and try again.");
            return;
        }

        var incident = await _dataMappingService.CreateIncidentAsync(formData, cancellationToken);
        
        SetSuccessMessage($"Incident '{incident.Title}' created successfully!");
        ResetForm();
    }
    catch (Exception ex)
    {
        SetErrorMessage($"Failed to create incident: {ex.Message}");
    }
    finally
    {
        IsSubmitting = false;
    }
}
```

#### **3.4 Testing Checklist**
- [ ] Incident creation works identically
- [ ] All form data is properly mapped
- [ ] Success/error handling preserved
- [ ] Form reset functionality works
- [ ] No data loss during mapping

---

## 📊 **Final Architecture Overview**

### **After Refactoring:**

```csharp
// ✅ Clean ViewModel (Microsoft Pattern)
public partial class IncidentCreateViewModel : BaseValidatingViewModel
{
    // ✅ UI Properties (for data binding)
    [ObservableProperty] public partial string Title { get; set; }
    [ObservableProperty] public partial int CurrentStep { get; set; }
    // ... all other UI properties ...

    // ✅ Injected Services (business logic)
    private readonly IIncidentValidationService _validationService;
    private readonly IIncidentWorkflowService _workflowService;
    private readonly IIncidentDataMappingService _dataMappingService;
    private readonly IIncidentService _incidentService;
    private readonly IEnumService _enumService;
    private readonly IApplicationService _applicationService;

    // ✅ Constructor Injection
    public IncidentCreateViewModel(/* all services */) { }

    // ✅ Commands delegate to services
    [RelayCommand]
    private async Task CreateIncidentAsync() 
    {
        var formData = CreateFormData();
        await _dataMappingService.CreateIncidentAsync(formData);
    }

    // ✅ Properties use services for computed values
    public bool CanGoNext => _workflowService.CanGoToNextStep(CurrentStep, TotalSteps, CreateFormData());
}
```

### **Benefits Achieved:**
- ✅ **100% Microsoft Compliance**: Follows official 2024 documentation exactly
- ✅ **Single Responsibility**: Each service has one clear purpose
- ✅ **Testability**: Business logic can be unit tested independently
- ✅ **Maintainability**: Clear separation of concerns
- ✅ **Reusability**: Services can be used by other ViewModels
- ✅ **Performance**: No inheritance overhead, clean dependency injection

### **File Structure:**
```
Application/
├── Interfaces/
│   ├── IIncidentValidationService.cs
│   ├── IIncidentWorkflowService.cs
│   └── IIncidentDataMappingService.cs
├── Services/
│   ├── IncidentValidationService.cs
│   ├── IncidentWorkflowService.cs
│   └── IncidentDataMappingService.cs
└── DTOs/
    └── IncidentFormData.cs

Presentation/ViewModels/
├── Base/
│   └── BaseValidatingViewModel.cs (unchanged)
└── IncidentCreateViewModel.cs (simplified)
```

---

## 🛡️ **Risk Mitigation**

### **Low Risk Steps:**
1. ✅ Service extraction (business logic only)
2. ✅ DTO creation (data structure)
3. ✅ Interface definition (contracts)

### **Medium Risk Steps:**
1. ⚠️ ViewModel constructor updates (DI changes)
2. ⚠️ Method delegation to services (behavior changes)

### **Testing Strategy:**
- Build after each step
- Functional testing after each service extraction
- Regression testing after ViewModel updates
- Performance testing after completion

---

## 🎯 **Success Criteria**

- ✅ Clean build with no errors or warnings
- ✅ All existing functionality works identically
- ✅ No regression in user experience
- ✅ Improved testability and maintainability
- ✅ 100% alignment with Microsoft 2024 best practices
- ✅ Reduced ViewModel complexity (from 705 lines to ~300 lines)
- ✅ Clear separation of concerns
- ✅ Reusable business logic services

---

## 🚀 **Ready to Implement**

This plan provides a **safe, incremental approach** that:
- ✅ Follows Microsoft's official 2024 documentation exactly
- ✅ Maintains all existing functionality
- ✅ Improves code quality and maintainability
- ✅ Reduces complexity through proper separation of concerns
- ✅ Enables comprehensive unit testing

**Each step can be implemented and tested independently, ensuring zero regression risk.**
