# AGENTS.md - IncidentCreateViewModel Refactoring Plan

## Executive Summary

This document provides a comprehensive, risk-assessed refactoring plan for `IncidentCreateViewModel.cs` (725 lines) to improve maintainability, reusability, and adherence to Microsoft's 2024 best practices. The refactoring will extract reusable components while preserving all existing functionality and state management logic.

**Current Status**: Enterprise-grade implementation with 96/100 Microsoft compliance score
**Target**: Modular, maintainable architecture with 100% functionality preservation
**Risk Level**: MEDIUM (due to complex state management and validation dependencies)

---

## Critical State Management Analysis

### 🔍 **Core State Components Identified**

#### **1. Form Data State (40+ Properties)**
```csharp
// CRITICAL: These properties drive the entire UI state
[ObservableProperty] string Title, Description, BusinessImpact, etc.
[ObservableProperty] DateTimeOffset? TimeIssueStarted, TimeReported
[ObservableProperty] IncidentType IncidentType (drives workflow logic)
[ObservableProperty] int CurrentStep (navigation state)
```

#### **2. Validation State Management**
```csharp
// CRITICAL: Complex interdependencies between properties
[NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
[NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
[NotifyDataErrorInfo] // Automatic validation integration
```

#### **3. Workflow State Logic**
```csharp
// CRITICAL: Step-based navigation with business rules
public bool CanGoNext => ValidateCurrentStep();
public bool IsLastStep => CurrentStep == TotalSteps;
public int TotalSteps => IsMajorIncident ? 4 : 3; // Dynamic workflow
```

#### **4. Command State Dependencies**
```csharp
// CRITICAL: Commands depend on multiple state properties
[RelayCommand(CanExecute = nameof(CanCreateIncident), IncludeCancelCommand = true)]
private async Task CreateIncidentAsync(CancellationToken cancellationToken)
```

### ⚠️ **High-Risk State Dependencies**

1. **Circular Property Dependencies**: `IncidentType` → `TotalSteps` → `IsLastStep` → `CanGoNext`
2. **Validation Chain Reactions**: Property changes trigger multiple validation notifications
3. **Command State Synchronization**: 7+ commands depend on validation state
4. **Dynamic Workflow Logic**: Step count changes based on incident type
5. **Cross-Step Validation**: Fields validated across multiple steps

---

## Risk Assessment Matrix

| Component | Risk Level | Impact | Probability | Mitigation Strategy |
|-----------|------------|--------|-------------|-------------------|
| **Property Notification Chain** | HIGH | Critical | Medium | Preserve exact notification patterns |
| **Step Navigation Logic** | HIGH | Critical | Low | Extract with comprehensive tests |
| **Validation Dependencies** | MEDIUM | High | Medium | Service-based validation with same logic |
| **Command State Management** | MEDIUM | High | Low | Maintain exact CanExecute patterns |
| **Incident Creation Logic** | LOW | Medium | Low | Well-isolated, easy to extract |
| **Form Reset Logic** | LOW | Medium | Low | Simple state reset, low complexity |

---

## Incremental Extraction Plan - One Step at a Time

### 🎯 **Extraction Order (User-Defined)**

**Strategy**: Extract one responsibility at a time, test functionality, then proceed to next extraction.
**Goal**: Maintain 100% functionality while reducing IncidentCreateViewModel from 725 lines to ~200 lines.

---

## ✅ **Step-by-Step Extraction Checklist**

### **Step 1: Validation Logic → Services** 🔍
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~80 lines

#### Files to Create:
- [ ] `Application/Interfaces/IIncidentValidationService.cs`
- [ ] `Infrastructure/Services/IncidentValidationService.cs`
- [ ] Update `App.xaml.cs` service registration

#### Methods to Extract from IncidentCreateViewModel:
- [ ] `ValidateStep1()` (lines 583-589)
- [ ] `ValidateStep2()` (lines 591-598) 
- [ ] `ValidateStep3()` (lines 600-609)
- [ ] `ValidateStep4()` (lines 611-619)
- [ ] `ValidateCommonRequiredFields()` (lines 634-645)
- [ ] `CanCreateIncident()` validation logic (lines 647-672)

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test incident creation workflow (all 4 steps)
- [ ] Verify validation messages appear correctly
- [ ] Confirm step navigation works
- [ ] Test form reset functionality
- [ ] **STOP HERE** - Ensure 100% functionality before proceeding

---

### **Step 2: Form State Management → Services** 📊
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~60 lines

#### Files to Create:
- [ ] `Application/Interfaces/IIncidentFormStateService.cs`
- [ ] `Infrastructure/Services/IncidentFormStateService.cs`
- [ ] Update `App.xaml.cs` service registration

#### Logic to Extract from IncidentCreateViewModel:
- [ ] Step counting logic (`TotalSteps` calculation)
- [ ] Workflow state management
- [ ] Form reset logic (from `ResetForm()` method)
- [ ] Step navigation state tracking

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test step navigation (Previous/Next buttons)
- [ ] Verify step counting works correctly
- [ ] Test form reset functionality
- [ ] Confirm workflow state transitions
- [ ] **STOP HERE** - Ensure 100% functionality before proceeding

---

### **Step 3: Data Mapping → Services** 🔄
**Risk**: LOW | **Impact**: MEDIUM | **Lines Reduced**: ~40 lines

#### Files to Create:
- [ ] `Application/Interfaces/IIncidentFormDataMapper.cs`
- [ ] `Infrastructure/Services/IncidentFormDataMapper.cs`
- [ ] `Application/DTOs/IncidentFormData.cs`
- [ ] Update `App.xaml.cs` service registration

#### Logic to Extract from IncidentCreateViewModel:
- [ ] Entity creation logic from `CreateIncidentAsync()` method
- [ ] Data transformation between ViewModel and Domain
- [ ] Property mapping logic

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test incident creation end-to-end
- [ ] Verify data is saved correctly to database
- [ ] Confirm all form fields map to entity properly
- [ ] Test with both Pre-Incident and Major Incident types
- [ ] **STOP HERE** - Ensure 100% functionality before proceeding

---

### **Step 4: Common Validation Patterns → BaseValidatingViewModel** 🏗️
**Risk**: LOW | **Impact**: HIGH | **Lines Reduced**: ~30 lines

#### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseValidatingViewModel.cs`
- [ ] Update `IncidentCreateViewModel.cs` to inherit from base class

#### Properties/Methods to Extract:
- [ ] `IsLoading` property
- [ ] `ErrorMessage` property
- [ ] `SuccessMessage` property
- [ ] `ValidateProperties()` method (lines 621-632)
- [ ] Common validation patterns

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test validation error messages display
- [ ] Verify loading states work correctly
- [ ] Confirm success messages appear
- [ ] Test property validation triggers
- [ ] **STOP HERE** - Ensure 100% functionality before proceeding

---

### **Step 5: Step Workflow Logic → BaseStepWorkflowViewModel** 🔄
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~50 lines

#### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseStepWorkflowViewModel.cs`
- [ ] Update `IncidentCreateViewModel.cs` to inherit from workflow base

#### Properties/Methods to Extract:
- [ ] `CurrentStep` property and related logic
- [ ] `TotalSteps` abstract property
- [ ] `IsFirstStep`, `IsLastStep` computed properties
- [ ] `CanGoNext`, `CanGoPrevious` abstract properties
- [ ] `GoToNextStep()`, `GoToPreviousStep()` methods
- [ ] Step navigation command logic

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test step navigation (Previous/Next buttons)
- [ ] Verify step indicators work correctly
- [ ] Test workflow transitions
- [ ] Confirm button enable/disable states
- [ ] **STOP HERE** - Ensure 100% functionality before proceeding

---

### **Step 6: Async Command Patterns → BaseAsyncCommandViewModel** ⚡
**Risk**: LOW | **Impact**: MEDIUM | **Lines Reduced**: ~25 lines

#### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseAsyncCommandViewModel.cs`
- [ ] Update base class inheritance if needed

#### Patterns to Extract:
- [ ] Enhanced async command patterns with cancellation
- [ ] Common async command error handling
- [ ] Cancellation token management
- [ ] Command state management patterns

#### Testing Checklist:
- [ ] Build application successfully
- [ ] Test async commands with cancellation
- [ ] Verify command state management
- [ ] Test error handling in async operations
- [ ] Confirm cancellation works correctly
- [ ] **FINAL VERIFICATION** - Complete end-to-end testing

---

## 🎯 **Final Result Expectations**

### **Before Refactoring:**
- `IncidentCreateViewModel.cs`: **725 lines**
- Single responsibility violations
- Difficult to test and maintain
- Code duplication across potential future ViewModels

### **After Refactoring:**
- `IncidentCreateViewModel.cs`: **~200 lines** (65% reduction)
- **10 new files** with single responsibilities
- **3 reusable base classes** for future ViewModels
- **6 service classes** with clean separation of concerns
- **1 DTO** for clean data transfer
- **100% functionality preservation**
- **Enterprise-grade maintainability**

---

## 🛡️ **Risk Mitigation at Each Step**

### **Testing Strategy:**
1. **Build Verification**: Must compile without errors
2. **Functional Testing**: All existing features must work identically
3. **Regression Testing**: No new bugs introduced
4. **Performance Testing**: No performance degradation
5. **Rollback Plan**: Each step can be reverted independently

### **Success Criteria for Each Step:**
- ✅ Clean build with no errors or warnings
- ✅ All existing functionality works identically
- ✅ No regression in user experience
- ✅ Code is more maintainable and testable
- ✅ Follows Microsoft 2024 best practices

---

## 📋 **Detailed Refactoring Plan (Original Structure)**

### 📋 **Phase 1: Foundation (LOW RISK) - Week 1**

#### **1.1 Create Base Classes**
**Risk**: LOW | **Impact**: Foundation for all future work

**Files to Create:**
```
Presentation/ViewModels/Base/
├── BaseValidatingViewModel.cs
├── BaseStepWorkflowViewModel.cs
└── BaseAsyncCommandViewModel.cs
```

**BaseValidatingViewModel.cs** (Microsoft Documentation: [MVVM Community Toolkit](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/))
```csharp
public abstract partial class BaseValidatingViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty] 
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial string? SuccessMessage { get; set; }

    protected void ValidateProperties(params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            ClearErrors(propertyName);
            var value = GetType().GetProperty(propertyName)?.GetValue(this);
            ValidateProperty(value, propertyName);
        }
    }

    protected virtual void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
```

**BaseStepWorkflowViewModel.cs** (Microsoft Documentation: [WinUI 3 Navigation](https://learn.microsoft.com/en-us/windows/apps/design/basics/navigation-basics))
```csharp
public abstract partial class BaseStepWorkflowViewModel : BaseValidatingViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFirstStep), nameof(IsLastStep), nameof(CanGoNext), nameof(CanGoPrevious))]
    public partial int CurrentStep { get; set; } = 1;

    public abstract int TotalSteps { get; }
    public bool IsFirstStep => CurrentStep == 1;
    public bool IsLastStep => CurrentStep == TotalSteps;
    public virtual bool CanGoNext => ValidateCurrentStep();
    public virtual bool CanGoPrevious => CurrentStep > 1;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void GoToNextStep()
    {
        if (CanGoNext)
        {
            CurrentStep++;
            ClearMessages();
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    protected virtual void GoToPreviousStep()
    {
        if (CanGoPrevious)
        {
            CurrentStep--;
            ClearMessages();
        }
    }

    protected abstract bool ValidateCurrentStep();
}
```

**Testing Strategy:**
- Unit tests for each base class
- Integration tests with mock ViewModels
- Verify property notification chains work correctly

#### **1.2 Service Interfaces**
**Risk**: LOW | **Files to Create:**

```
Application/Interfaces/
├── IIncidentValidationService.cs
├── IIncidentFormStateService.cs
└── IIncidentFormDataMapper.cs
```

### 📋 **Phase 2: Service Implementation (MEDIUM RISK) - Week 2**

#### **2.1 Validation Service** 
**Risk**: MEDIUM | **Critical State Preservation Required**

**IncidentValidationService.cs** (Microsoft Documentation: [Data Annotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations))
```csharp
public class IncidentValidationService : IIncidentValidationService
{
    public ValidationResult ValidateStep(int step, IncidentFormData data)
    {
        // CRITICAL: Preserve exact validation logic from original
        return step switch
        {
            1 => ValidateStep1(data), // Incident Type selected
            2 => ValidateStep2(data), // Basic Information  
            3 => ValidateStep3(data), // Incident Details
            4 => ValidateStep4(data), // Master Checklist (Major only)
            _ => ValidationResult.Invalid("Invalid step")
        };
    }

    private ValidationResult ValidateStep2(IncidentFormData data)
    {
        // EXACT COPY of ValidateCommonRequiredFields logic
        var isValid = data.TimeIssueStarted.HasValue &&
                     data.TimeReported.HasValue &&
                     data.ImpactedUsers.HasValue &&
                     !string.IsNullOrWhiteSpace(data.ApplicationAffected) &&
                     !string.IsNullOrWhiteSpace(data.LocationsAffected);
        
        return isValid ? ValidationResult.Valid() : ValidationResult.Invalid("Required fields missing");
    }
    
    // ... other validation methods (EXACT COPIES)
}
```

**Critical Requirements:**
- ✅ **EXACT** validation logic preservation
- ✅ Same error messages and validation rules
- ✅ Identical field requirement logic
- ✅ Preserve cross-step validation dependencies

#### **2.2 Form State Service**
**Risk**: MEDIUM | **Complex State Management**

**IncidentFormStateService.cs** (Microsoft Documentation: [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures))
```csharp
public class IncidentFormStateService : IIncidentFormStateService
{
    public bool IsFieldVisible(string fieldName, IncidentType incidentType, int currentStep)
    {
        // CRITICAL: Preserve exact field visibility logic
        return fieldName switch
        {
            nameof(IncidentFormData.BusinessImpact) => incidentType == IncidentType.MajorIncident && currentStep == 4,
            nameof(IncidentFormData.Title) => currentStep >= 3,
            nameof(IncidentFormData.Description) => currentStep >= 3,
            // ... exact field visibility rules
            _ => true
        };
    }

    public int GetTotalSteps(IncidentType incidentType)
    {
        // CRITICAL: Preserve exact workflow logic
        return incidentType == IncidentType.MajorIncident ? 4 : 3;
    }
}
```

### 📋 **Phase 3: Data Transfer Objects (LOW RISK) - Week 2**

#### **3.1 IncidentFormData.cs**
**Risk**: LOW | **Clean data container**

```csharp
public class IncidentFormData
{
    // EXACT property mapping from IncidentCreateViewModel
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BusinessImpact { get; set; }
    public DateTimeOffset? TimeIssueStarted { get; set; }
    public DateTimeOffset? TimeReported { get; set; }
    public int? ImpactedUsers { get; set; }
    public string? ApplicationAffected { get; set; }
    public string? LocationsAffected { get; set; }
    public string? Workaround { get; set; }
    public string? IncidentNumber { get; set; }
    public int Urgency { get; set; } = 3;
    public IncidentType IncidentType { get; set; } = IncidentType.PreIncident;
    public Priority Priority { get; set; } = Priority.P3;
    public Status Status { get; set; } = Status.New;
}
```

### 📋 **Phase 4: ViewModel Refactoring (HIGH RISK) - Week 3**

#### **4.1 Refactored IncidentCreateViewModel**
**Risk**: HIGH | **Critical State Preservation**

**Strategy**: Incremental replacement with feature flags

```csharp
public partial class IncidentCreateViewModel : BaseStepWorkflowViewModel
{
    private readonly IIncidentService _incidentService;
    private readonly IIncidentValidationService _validationService;
    private readonly IIncidentFormStateService _formStateService;
    private readonly IIncidentFormDataMapper _formDataMapper;

    // PRESERVE: All original properties with EXACT same attributes
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGoNext), nameof(IsLastStep))]
    [NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Incident title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public partial string Title { get; set; } = string.Empty;

    // ... all other properties (EXACT COPIES)

    // OVERRIDE: Base class methods with original logic
    public override int TotalSteps => _formStateService.GetTotalSteps(IncidentType);

    protected override bool ValidateCurrentStep()
    {
        var formData = _formDataMapper.MapFromViewModel(this);
        var result = _validationService.ValidateStep(CurrentStep, formData);
        return result.IsValid;
    }

    // PRESERVE: All original commands and methods
    [RelayCommand(CanExecute = nameof(CanCreateIncident), IncludeCancelCommand = true)]
    private async Task CreateIncidentAsync(CancellationToken cancellationToken)
    {
        // EXACT COPY of original method
        // No changes to incident creation logic
    }
}
```

**Critical Preservation Requirements:**
- ✅ **EXACT** property attributes and notifications
- ✅ **IDENTICAL** validation logic and error messages  
- ✅ **SAME** command CanExecute patterns
- ✅ **PRESERVE** all property change notification chains
- ✅ **MAINTAIN** step navigation behavior
- ✅ **KEEP** incident creation and form reset logic

---

## Implementation Strategy

### 🎯 **Recommended Starting Point: Phase 1 (Base Classes)**

**Rationale:**
1. **Lowest Risk**: Base classes don't affect existing functionality
2. **Immediate Value**: Can be tested independently
3. **Foundation**: Required for all subsequent phases
4. **Parallel Development**: Team can work on services while base classes are being tested

### 🔧 **Development Approach**

#### **Week 1: Foundation**
- ✅ Create base classes with comprehensive unit tests
- ✅ Verify property notification chains work correctly
- ✅ Test step navigation logic in isolation
- ✅ Create service interfaces

#### **Week 2: Services & DTOs**
- ✅ Implement validation service with exact logic preservation
- ✅ Implement form state service with comprehensive tests
- ✅ Create data transfer objects and mappers
- ✅ Integration testing of all services

#### **Week 3: ViewModel Refactoring**
- ✅ Feature flag approach for gradual migration
- ✅ Side-by-side testing of old vs new implementation
- ✅ Comprehensive integration testing
- ✅ Performance testing and validation

#### **Week 4: Validation & Cleanup**
- ✅ Full regression testing
- ✅ Performance benchmarking
- ✅ Documentation updates
- ✅ Code cleanup and optimization

---

## Testing Strategy

### 🧪 **Comprehensive Test Coverage**

#### **Unit Tests (Required)**
```csharp
// Base class testing
[Test] public void BaseStepWorkflowViewModel_NavigationLogic_WorksCorrectly()
[Test] public void BaseValidatingViewModel_ValidationChain_PreservesNotifications()

// Service testing  
[Test] public void IncidentValidationService_ValidateStep2_MatchesOriginalLogic()
[Test] public void IncidentFormStateService_GetTotalSteps_ReturnsCorrectCount()

// Integration testing
[Test] public void RefactoredViewModel_PropertyNotifications_MatchOriginal()
[Test] public void RefactoredViewModel_CommandStates_IdenticalToOriginal()
```

#### **Integration Tests (Critical)**
```csharp
[Test] public void IncidentCreation_EndToEnd_ProducesSameResult()
[Test] public void StepNavigation_AllScenarios_BehaviorIdentical()
[Test] public void ValidationLogic_AllFields_ErrorMessagesMatch()
```

#### **Performance Tests**
```csharp
[Test] public void PropertyChangeNotifications_Performance_WithinThreshold()
[Test] public void ValidationExecution_Performance_NoRegression()
```

---

## Risk Mitigation Strategies

### 🛡️ **High-Risk Component Mitigation**

#### **1. Property Notification Chain Preservation**
- **Strategy**: Create comprehensive property change tests before refactoring
- **Validation**: Automated tests verify exact notification sequences
- **Rollback**: Feature flags allow instant rollback if issues detected

#### **2. Validation Logic Preservation**
- **Strategy**: Copy-paste validation methods initially, then optimize
- **Validation**: Side-by-side testing of validation results
- **Rollback**: Service interface allows swapping implementations

#### **3. Command State Management**
- **Strategy**: Preserve exact CanExecute patterns and dependencies
- **Validation**: Automated UI tests verify button states
- **Rollback**: Command implementations can be reverted independently

#### **4. Step Navigation Logic**
- **Strategy**: Extract navigation logic with comprehensive test coverage
- **Validation**: Test all navigation scenarios and edge cases
- **Rollback**: Base class inheritance allows easy reversion

---

## Microsoft Documentation References

### 📚 **Essential Documentation**

#### **MVVM Community Toolkit**
- **URL**: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- **Focus**: ObservableProperty, RelayCommand, NotifyPropertyChangedFor patterns
- **Critical Sections**: 
  - Property change notifications
  - Command patterns and CanExecute
  - Validation integration

#### **Clean Architecture**
- **URL**: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- **Focus**: Layer separation, dependency injection, service patterns
- **Critical Sections**:
  - Presentation layer responsibilities
  - Application service patterns
  - Dependency inversion principles

#### **WinUI 3 Data Binding**
- **URL**: https://learn.microsoft.com/en-us/windows/apps/develop/data-binding/
- **Focus**: x:Bind patterns, property change notifications, validation
- **Critical Sections**:
  - Two-way binding patterns
  - Validation error handling
  - Performance considerations

#### **Dependency Injection Guidelines**
- **URL**: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
- **Focus**: Service lifetimes, registration patterns, best practices
- **Critical Sections**:
  - Service lifetime management
  - Constructor injection patterns
  - Service composition

#### **Data Annotations**
- **URL**: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations
- **Focus**: Validation attributes, error messages, custom validation
- **Critical Sections**:
  - Built-in validation attributes
  - Custom validation implementation
  - Integration with MVVM patterns

---

## Success Criteria

### ✅ **Functional Requirements**
- [ ] **100% Feature Parity**: All existing functionality preserved
- [ ] **Identical Validation**: Same validation rules and error messages
- [ ] **Same User Experience**: Navigation and workflow behavior unchanged
- [ ] **Performance Maintained**: No regression in response times
- [ ] **Memory Usage**: No increase in memory footprint

### ✅ **Quality Requirements**
- [ ] **Code Reduction**: IncidentCreateViewModel reduced from 725 to ~200 lines
- [ ] **Reusability**: Base classes usable by other ViewModels
- [ ] **Maintainability**: Single Responsibility Principle applied
- [ ] **Testability**: 95%+ code coverage achieved
- [ ] **Documentation**: Comprehensive XML documentation

### ✅ **Technical Requirements**
- [ ] **Microsoft Compliance**: 100% adherence to 2024 best practices
- [ ] **Clean Architecture**: Proper layer separation maintained
- [ ] **SOLID Principles**: All principles correctly applied
- [ ] **Dependency Injection**: Proper service registration and lifetimes
- [ ] **Error Handling**: Robust error handling preserved

---

## Conclusion

This refactoring plan provides a comprehensive, risk-assessed approach to modernizing the `IncidentCreateViewModel.cs` while preserving all existing functionality. The phased approach minimizes risk while maximizing the benefits of improved maintainability and reusability.

**Key Success Factors:**
1. **Exact Logic Preservation**: All validation and state management logic copied exactly
2. **Comprehensive Testing**: Every component tested before and after refactoring
3. **Incremental Implementation**: Phased approach allows for validation at each step
4. **Risk Mitigation**: Multiple rollback strategies and feature flags
5. **Microsoft Compliance**: Following official documentation and best practices

**Estimated Timeline**: 4 weeks with 2 developers
**Risk Level**: MEDIUM (manageable with proper testing and phased approach)
**Expected Outcome**: 60% reduction in ViewModel complexity with 100% functionality preservation

---

*This document serves as the master plan for the IncidentCreateViewModel refactoring initiative. All implementation should follow this plan to ensure successful completion with minimal risk.*
