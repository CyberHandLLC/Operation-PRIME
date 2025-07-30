# Microsoft-Aligned Refactoring Checklist

## 📚 **Documentation References**

This checklist is based on official Microsoft documentation:
- ✅ [MVVM Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) - Official MVVM patterns
- ✅ [Model-View-ViewModel Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm) - Architectural guidance
- ✅ [Dependency Injection Guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines) - Service registration
- ✅ [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures) - Layer separation
- ✅ [Putting Things Together](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/puttingthingstogether) - Practical examples

---

## 🎯 **Pre-Refactoring Checklist**

### **✅ Prerequisites**
- [ ] **Backup Current Code**: Create git branch or backup before starting
- [ ] **Verify Build**: Ensure current code builds without errors
- [ ] **Run Tests**: Execute any existing tests to establish baseline
- [ ] **Document Current Behavior**: Note all current functionality for regression testing

### **✅ Environment Verification**
- [ ] **NuGet Packages**: Verify CommunityToolkit.Mvvm 8.4.0+ is installed
- [ ] **Target Framework**: Confirm .NET 9 target framework
- [ ] **Project Structure**: Verify Clean Architecture folder structure exists
- [ ] **DI Container**: Confirm dependency injection is properly configured in App.xaml.cs

---

## 🚀 **Step 1: Extract Validation Service**

### **1.1 Create Service Interface**
- [ ] **Create File**: `Application/Interfaces/IIncidentValidationService.cs`
- [ ] **Define Interface**: Add all validation method signatures
- [ ] **Add Documentation**: Include XML documentation for each method
- [ ] **Verify Namespace**: Ensure proper namespace `OperationPrime.Application.Interfaces`

**Code Location Reference**: Lines 574-652 in `IncidentCreateViewModel.cs`

**Methods to Extract**:
```csharp
// From IncidentCreateViewModel.cs lines:
- ValidateStep1() → line 574-580
- ValidateStep2() → line 582-589  
- ValidateStep3() → line 591-600
- ValidateStep4() → line 602-610
- ValidateCommonRequiredFields() → line 614-625
- CanCreateIncident() → line 627-652
```

**Interface Definition Checklist**:
- [ ] `bool ValidateStep1(IncidentType incidentType)`
- [ ] `bool ValidateStep2(string title, string description, DateTime? timeIssueStarted, DateTime? timeReported, int? impactedUsers, string applicationAffected, string locationsAffected)`
- [ ] `bool ValidateStep3(string title, string description, DateTime? timeIssueStarted, DateTime? timeReported, int? impactedUsers, string applicationAffected, string locationsAffected)`
- [ ] `bool ValidateStep4(bool isMajorIncident, string businessImpact)`
- [ ] `bool ValidateCurrentStep(int currentStep, IncidentFormData formData)`
- [ ] `bool CanCreateIncident(IncidentFormData formData, int currentStep, int totalSteps)`

### **1.2 Create Form Data DTO**
- [ ] **Create File**: `Application/DTOs/IncidentFormData.cs`
- [ ] **Add Properties**: All form properties without MVVM attributes
- [ ] **Add Computed Properties**: `IsMajorIncident` calculated property
- [ ] **Verify Data Types**: Ensure all types match ViewModel properties

**Properties to Include**:
```csharp
// All properties from IncidentCreateViewModel.cs:
- Title (string) → line 44-50
- Description (string) → line 52-58
- IncidentType (IncidentType) → line 60-66
- Priority (Priority) → line 75-77
- Status (Status) → line 79-81
- BusinessImpact (string) → line 88-94
- TimeIssueStarted (DateTime?) → line 96-102
- TimeReported (DateTime?) → line 104-110
- ImpactedUsers (int?) → line 112-118
- ApplicationAffected (string) → line 138-144
- LocationsAffected (string) → line 146-152
- Workaround (string) → line 154-156
- IncidentNumber (string) → line 120-126
- Urgency (int) → line 128-136
```

**DTO Checklist**:
- [ ] All properties are plain C# properties (no MVVM attributes)
- [ ] Default values match ViewModel defaults
- [ ] `IsMajorIncident` computed property included
- [ ] Proper XML documentation added

### **1.3 Create Service Implementation**
- [ ] **Create File**: `Application/Services/IncidentValidationService.cs`
- [ ] **Implement Interface**: Implement `IIncidentValidationService`
- [ ] **Copy Logic**: Move validation logic from ViewModel (preserve exact behavior)
- [ ] **Add Error Handling**: Include proper exception handling
- [ ] **Add Logging**: Consider adding ILogger for debugging

**Implementation Checklist**:
- [ ] `ValidateStep1()` - Checks IncidentType is not default
- [ ] `ValidateStep2()` - Validates common required fields
- [ ] `ValidateStep3()` - Validates title, description, and common fields
- [ ] `ValidateStep4()` - Validates business impact for major incidents
- [ ] `ValidateCurrentStep()` - Switch statement for step-specific validation
- [ ] `CanCreateIncident()` - Validates all steps for current incident type
- [ ] `ValidateCommonRequiredFields()` - Private helper method

**Microsoft Pattern Compliance**:
- [ ] **Service Registration**: Uses constructor injection pattern
- [ ] **Single Responsibility**: Only handles validation logic
- [ ] **Stateless Design**: No instance state, pure functions
- [ ] **Interface Segregation**: Clean, focused interface

### **1.4 Register Service in DI Container**
- [ ] **Open File**: `App.xaml.cs`
- [ ] **Add Registration**: `services.AddScoped<IIncidentValidationService, IncidentValidationService>()`
- [ ] **Verify Order**: Add after existing service registrations
- [ ] **Build Test**: Ensure application builds after registration

**DI Registration Location**: In `ConfigureServices()` method around line 35-45

### **1.5 Update ViewModel Constructor**
- [ ] **Add Parameter**: `IIncidentValidationService validationService`
- [ ] **Add Field**: `private readonly IIncidentValidationService _validationService;`
- [ ] **Initialize Field**: `_validationService = validationService;`
- [ ] **Update XML Docs**: Add parameter documentation

**Constructor Update Checklist**:
- [ ] Parameter added to constructor signature
- [ ] Field declaration added to class
- [ ] Field initialized in constructor body
- [ ] XML documentation updated
- [ ] Build verification successful

### **1.6 Update Validation Methods in ViewModel**
- [ ] **Replace ValidateStep1()**: Delegate to `_validationService.ValidateStep1(IncidentType)`
- [ ] **Replace ValidateStep2()**: Delegate to service with parameters
- [ ] **Replace ValidateStep3()**: Delegate to service with parameters
- [ ] **Replace ValidateStep4()**: Delegate to service with parameters
- [ ] **Replace ValidateCurrentStep()**: Create DTO and delegate to service
- [ ] **Replace CanCreateIncident()**: Create DTO and delegate to service
- [ ] **Add CreateFormData()**: Helper method to create DTO from ViewModel properties

**Method Replacement Checklist**:
- [ ] All validation methods now delegate to service
- [ ] `CreateFormData()` helper method created
- [ ] No validation logic remains in ViewModel
- [ ] All method signatures preserved (no breaking changes)
- [ ] Return types and behavior identical

### **1.7 Testing Checklist - Step 1**
- [ ] **Build Verification**: Clean build with no errors or warnings
- [ ] **Functional Testing**: All validation behaves identically
- [ ] **Button States**: Next/Create buttons enable/disable correctly
- [ ] **Error Messages**: Validation errors display properly
- [ ] **Step Navigation**: Can navigate between steps as before
- [ ] **Form Submission**: Incident creation works identically
- [ ] **Regression Testing**: No new bugs introduced

**Test Scenarios**:
- [ ] Test each step validation individually
- [ ] Test step navigation with invalid data
- [ ] Test form submission with valid/invalid data
- [ ] Test incident type switching (Pre-Incident ↔ Major Incident)
- [ ] Test all validation error messages

---

## 🚀 **Step 2: Extract Workflow Service**

### **2.1 Create Workflow Interface**
- [ ] **Create File**: `Application/Interfaces/IIncidentWorkflowService.cs`
- [ ] **Define Methods**: All workflow-related method signatures
- [ ] **Add BreadcrumbItem**: Define supporting types
- [ ] **Add Documentation**: XML docs for all methods

**Code Location Reference**: Lines 180-324 in `IncidentCreateViewModel.cs`

**Methods to Extract**:
```csharp
// From IncidentCreateViewModel.cs:
- TotalSteps property logic → line 180-182
- CanGoNext property logic → line 184-186
- CanGoPrevious property logic → line 188-190
- IsLastStep property logic → line 192-194
- ShowNextButton property logic → line 196-198
- BreadcrumbItems property logic → line 200-218
- CurrentBreadcrumbIndex property logic → line 220-222
```

**Interface Method Checklist**:
- [ ] `int CalculateTotalSteps(IncidentType incidentType)`
- [ ] `bool CanGoToNextStep(int currentStep, int totalSteps, IncidentFormData formData)`
- [ ] `bool CanGoToPreviousStep(int currentStep)`
- [ ] `bool IsLastStep(int currentStep, int totalSteps)`
- [ ] `bool ShowNextButton(int currentStep, int totalSteps)`
- [ ] `IReadOnlyList<BreadcrumbItem> GenerateBreadcrumbItems(IncidentType incidentType)`
- [ ] `int CalculateBreadcrumbIndex(int currentStep)`

### **2.2 Create BreadcrumbItem Class**
- [ ] **Create File**: `Application/Models/BreadcrumbItem.cs`
- [ ] **Add Properties**: `Text`, `IsEnabled`
- [ ] **Add Documentation**: XML documentation
- [ ] **Verify Namespace**: Proper namespace structure

**BreadcrumbItem Checklist**:
- [ ] `string Text { get; set; }` property
- [ ] `bool IsEnabled { get; set; }` property
- [ ] Default constructor
- [ ] XML documentation

### **2.3 Create Workflow Service Implementation**
- [ ] **Create File**: `Application/Services/IncidentWorkflowService.cs`
- [ ] **Implement Interface**: `IIncidentWorkflowService`
- [ ] **Inject Dependencies**: `IIncidentValidationService` for CanGoNext logic
- [ ] **Copy Logic**: Move workflow logic from ViewModel

**Implementation Checklist**:
- [ ] `CalculateTotalSteps()` - Returns 3 for PreIncident, 4 for MajorIncident
- [ ] `CanGoToNextStep()` - Validates current step before allowing navigation
- [ ] `CanGoToPreviousStep()` - Checks if not on first step
- [ ] `IsLastStep()` - Compares current step to total steps
- [ ] `ShowNextButton()` - Determines if Next button should be visible
- [ ] `GenerateBreadcrumbItems()` - Creates breadcrumb list based on incident type
- [ ] `CalculateBreadcrumbIndex()` - Converts step number to zero-based index

### **2.4 Register Workflow Service**
- [ ] **Update DI**: Add `services.AddScoped<IIncidentWorkflowService, IncidentWorkflowService>()`
- [ ] **Verify Dependencies**: Ensure validation service is registered first
- [ ] **Build Test**: Confirm clean build

### **2.5 Update ViewModel for Workflow Service**
- [ ] **Add Constructor Parameter**: `IIncidentWorkflowService workflowService`
- [ ] **Add Field**: `private readonly IIncidentWorkflowService _workflowService;`
- [ ] **Update Properties**: Convert computed properties to use service
- [ ] **Remove Workflow Logic**: Delete workflow methods from ViewModel

**Property Updates**:
```csharp
// Convert these properties to use service:
public int TotalSteps => _workflowService.CalculateTotalSteps(IncidentType);
public bool CanGoNext => _workflowService.CanGoToNextStep(CurrentStep, TotalSteps, CreateFormData());
public bool CanGoPrevious => _workflowService.CanGoToPreviousStep(CurrentStep);
public bool IsLastStep => _workflowService.IsLastStep(CurrentStep, TotalSteps);
public bool ShowNextButton => _workflowService.ShowNextButton(CurrentStep, TotalSteps);
public IReadOnlyList<BreadcrumbItem> BreadcrumbItems => _workflowService.GenerateBreadcrumbItems(IncidentType);
public int CurrentBreadcrumbIndex => _workflowService.CalculateBreadcrumbIndex(CurrentStep);
```

### **2.6 Testing Checklist - Step 2**
- [ ] **Build Verification**: Clean build with no errors
- [ ] **Step Navigation**: Forward/backward navigation works
- [ ] **Button States**: Next/Previous buttons enable/disable correctly
- [ ] **Breadcrumb Navigation**: Breadcrumb clicking works
- [ ] **Incident Type Switching**: Breadcrumbs update when switching types
- [ ] **Step Indicators**: IsStep1-IsStep4 properties work correctly
- [ ] **Total Steps**: Correct step count for each incident type

---

## 🚀 **Step 3: Extract Data Mapping Service**

### **3.1 Create Data Mapping Interface**
- [ ] **Create File**: `Application/Interfaces/IIncidentDataMappingService.cs`
- [ ] **Define Methods**: Entity mapping and creation methods
- [ ] **Add Documentation**: XML docs for all methods

**Code Location Reference**: Lines 385-471 in `IncidentCreateViewModel.cs` (`CreateIncidentAsync` method)

**Interface Method Checklist**:
- [ ] `Incident MapFormDataToEntity(IncidentFormData formData)`
- [ ] `Task<Incident> CreateIncidentAsync(IncidentFormData formData, CancellationToken cancellationToken = default)`

### **3.2 Create Data Mapping Service Implementation**
- [ ] **Create File**: `Application/Services/IncidentDataMappingService.cs`
- [ ] **Implement Interface**: `IIncidentDataMappingService`
- [ ] **Inject Dependencies**: `IIncidentService` for database operations
- [ ] **Copy Mapping Logic**: Extract entity creation from ViewModel

**Implementation Checklist**:
- [ ] `MapFormDataToEntity()` - Creates Incident entity from form data
- [ ] `CreateIncidentAsync()` - Maps data and calls incident service
- [ ] **Property Mapping**: All form properties mapped to entity
- [ ] **Default Values**: CreatedDate set to UtcNow
- [ ] **Error Handling**: Proper exception handling

**Entity Mapping Verification**:
```csharp
// Verify all properties are mapped:
- Title → incident.Title
- Description → incident.Description
- IncidentType → incident.IncidentType
- Priority → incident.Priority
- Status → incident.Status
- BusinessImpact → incident.BusinessImpact
- TimeIssueStarted → incident.TimeIssueStarted
- TimeReported → incident.TimeReported
- ImpactedUsers → incident.ImpactedUsers
- ApplicationAffected → incident.ApplicationAffected
- LocationsAffected → incident.LocationsAffected
- Workaround → incident.Workaround
- IncidentNumber → incident.IncidentNumber
- Urgency → incident.Urgency
- CreatedDate → DateTimeOffset.UtcNow (auto-set)
```

### **3.3 Register Data Mapping Service**
- [ ] **Update DI**: Add `services.AddScoped<IIncidentDataMappingService, IncidentDataMappingService>()`
- [ ] **Verify Order**: Register after IIncidentService
- [ ] **Build Test**: Ensure clean build

### **3.4 Update ViewModel CreateIncidentAsync Method**
- [ ] **Add Constructor Parameter**: `IIncidentDataMappingService dataMappingService`
- [ ] **Add Field**: `private readonly IIncidentDataMappingService _dataMappingService;`
- [ ] **Simplify Method**: Replace entity creation with service call
- [ ] **Preserve Error Handling**: Keep try-catch and user feedback

**Method Simplification**:
```csharp
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

### **3.5 Testing Checklist - Step 3**
- [ ] **Build Verification**: Clean build with no errors
- [ ] **Incident Creation**: Can create incidents successfully
- [ ] **Data Mapping**: All form data appears in created incident
- [ ] **Success Messages**: Success message displays correctly
- [ ] **Error Handling**: Error messages display for failures
- [ ] **Form Reset**: Form resets after successful creation
- [ ] **Cancellation**: Cancellation token works properly

---

## 🧪 **Final Integration Testing**

### **✅ Comprehensive Testing Checklist**
- [ ] **Build Verification**: Clean build with zero errors/warnings
- [ ] **Functional Equivalence**: All features work identically to before
- [ ] **Performance**: No noticeable performance degradation
- [ ] **Memory Usage**: No memory leaks introduced
- [ ] **Error Handling**: All error scenarios handled properly

### **✅ User Experience Testing**
- [ ] **Form Interaction**: All form fields work correctly
- [ ] **Step Navigation**: Can navigate between all steps
- [ ] **Validation Feedback**: Real-time validation works
- [ ] **Button States**: All buttons enable/disable correctly
- [ ] **Incident Creation**: Can create both Pre-Incident and Major Incident
- [ ] **Form Reset**: Reset functionality works properly

### **✅ Regression Testing Scenarios**
- [ ] **Empty Form Submission**: Proper validation errors
- [ ] **Partial Form Completion**: Navigation restrictions work
- [ ] **Incident Type Switching**: Form adapts correctly
- [ ] **Field Validation**: Individual field validation works
- [ ] **Async Operations**: Loading states and cancellation work
- [ ] **Error Recovery**: Can recover from errors and continue

---

## 📊 **Success Metrics**

### **✅ Code Quality Improvements**
- [ ] **Line Count Reduction**: ViewModel reduced from 705 to ~300 lines
- [ ] **Separation of Concerns**: Business logic separated from UI logic
- [ ] **Testability**: Each service can be unit tested independently
- [ ] **Maintainability**: Clear, focused responsibilities
- [ ] **Reusability**: Services can be used by other ViewModels

### **✅ Architecture Compliance**
- [ ] **Microsoft MVVM Patterns**: 100% compliance with official documentation
- [ ] **Clean Architecture**: Proper layer separation maintained
- [ ] **Dependency Injection**: Follows Microsoft DI guidelines
- [ ] **Single Responsibility**: Each class has one clear purpose
- [ ] **Interface Segregation**: Clean, focused interfaces

### **✅ Documentation and Maintenance**
- [ ] **XML Documentation**: All public APIs documented
- [ ] **Code Comments**: Complex logic explained
- [ ] **Service Contracts**: Clear interface definitions
- [ ] **Error Messages**: Meaningful error messages
- [ ] **Logging**: Appropriate logging for debugging

---

## 🎯 **Post-Refactoring Verification**

### **✅ Final Architecture Review**
- [ ] **Service Layer**: All business logic in services
- [ ] **ViewModel Layer**: Only UI state and binding properties
- [ ] **Interface Layer**: Clean contracts between layers
- [ ] **DTO Layer**: Clean data transfer objects
- [ ] **Dependency Injection**: Proper service registration

### **✅ Microsoft Documentation Alignment**
- [ ] **MVVM Community Toolkit**: Follows official patterns exactly
- [ ] **Clean Architecture**: Proper layer dependencies
- [ ] **Dependency Injection**: Uses Microsoft recommended patterns
- [ ] **Service Lifetimes**: Correct service registration lifetimes
- [ ] **Best Practices**: Aligns with 2024 Microsoft guidance

### **✅ Future Readiness**
- [ ] **Extensibility**: Easy to add new features
- [ ] **Testability**: Comprehensive unit testing possible
- [ ] **Maintainability**: Clear code organization
- [ ] **Scalability**: Architecture supports growth
- [ ] **Team Development**: Clear patterns for team members

---

## 🚀 **Completion Checklist**

### **✅ Final Deliverables**
- [ ] **7 New Files Created**: 3 interfaces, 3 services, 1 DTO
- [ ] **1 File Modified**: IncidentCreateViewModel.cs simplified
- [ ] **DI Registration**: All services registered properly
- [ ] **Documentation**: All code properly documented
- [ ] **Testing**: All functionality verified

### **✅ Success Criteria Met**
- [ ] **100% Functionality Preservation**: No features lost
- [ ] **Microsoft Compliance**: Follows official 2024 patterns
- [ ] **Code Quality**: Improved maintainability and testability
- [ ] **Performance**: No degradation in user experience
- [ ] **Architecture**: Clean separation of concerns achieved

---

## 📚 **Reference Documentation**

### **Microsoft Official Sources**
- [MVVM Community Toolkit Introduction](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [MVVM Pattern Overview](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Dependency Injection Guidelines](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines)
- [Clean Architecture Patterns](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Putting Things Together](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/puttingthingstogether)

### **Implementation Examples**
- [MVVM Toolkit Sample App](https://github.com/CommunityToolkit/dotnet/tree/main/samples/CommunityToolkit.Mvvm.Samples)
- [WinUI 3 MVVM Examples](https://github.com/microsoft/WinUI-Gallery)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

**This checklist ensures 100% compliance with Microsoft's 2024 documentation and provides a safe, incremental refactoring path.** ✅
