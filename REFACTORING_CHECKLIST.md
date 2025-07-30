# IncidentCreateViewModel Refactoring Checklist

## 🎯 **Objective**
Refactor `IncidentCreateViewModel.cs` (725 lines) into maintainable, reusable components following Microsoft's Clean Architecture and MVVM best practices.

**Target**: Reduce to ~200 lines while maintaining 100% functionality.

---

## 📋 **Complete Refactoring Checklist**

### **Phase 1: Foundation Base Classes (Build Inheritance Hierarchy)**

#### ✅ **Step 1: Common Validation Patterns → BaseValidatingViewModel** - **COMPLETED** ✅
**Risk**: LOW | **Impact**: HIGH | **Lines Reduced**: ~25 lines

##### Files Created:
- [x] `Presentation/ViewModels/Base/BaseValidatingViewModel.cs` ✅
- [x] Updated `IncidentCreateViewModel.cs` inheritance ✅

##### Code Successfully Extracted from `IncidentCreateViewModel.cs`:
| Property/Method | Status | Description |
|-----------------|--------|-------------|
| `ErrorMessage` | ✅ Extracted | Error message display property |
| `SuccessMessage` | ✅ Extracted | Success message display property |
| `ValidateProperties()` | ✅ Extracted | Multi-property validation helper method |
| `IsLoading` | ✅ Added | New general loading state property |

##### Additional Enhancements Made:
- ✅ Added `ClearMessages()` helper method
- ✅ Added `SetErrorMessage()` helper method  
- ✅ Added `SetSuccessMessage()` helper method
- ✅ Proper inheritance from `ObservableValidator`
- ✅ Microsoft MVVM Community Toolkit compliance

##### Testing Results:
- [x] Build application successfully ✅
- [x] Application runs without errors ✅
- [x] Validation patterns preserved ✅
- [x] Clean inheritance established ✅
- [x] Foundation ready for next steps ✅

**Status**: **COMPLETE** - Ready for Step 2

---

#### ✅ **Step 2: Step Workflow Logic → BaseStepWorkflowViewModel**
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~80 lines

##### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseStepWorkflowViewModel.cs`
- [ ] Update `IncidentCreateViewModel.cs` inheritance

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Property/Method | Current Lines | Description |
|-----------------|---------------|-------------|
| `CurrentStep` | 118-121 | Current workflow step with 8-property notification chain |
| `IsStep1` | 238 | Step 1 indicator (computed property) |
| `IsStep2` | 243 | Step 2 indicator (computed property) |
| `IsStep3` | 248 | Step 3 indicator (computed property) |
| `IsStep4` | 253 | Step 4 indicator (computed property, Major Incident only) |
| `TotalSteps` | 258 | Dynamic step count (abstract in base class) |
| `ShowNextButton` | 263 | UI control for Next button visibility |
| `CanGoNext` | 268 | Navigation validation (abstract in base class) |
| `CanGoPrevious` | 273 | Previous step validation |
| `IsLastStep` | 278 | Last step indicator |
| `BreadcrumbItems` | 283-289 | Navigation breadcrumb collection |
| `CurrentBreadcrumbIndex` | 294 | Current breadcrumb position (0-based) |
| `IsMajorIncident` | 233 | Core workflow state property |
| `IsPreIncidentSelected` | 69 | UI binding for Pre-Incident type selection |
| `IsMajorIncidentSelected` | 74 | UI binding for Major Incident type selection |
| `SelectPreIncident()` | 513-537 | Pre-Incident workflow setup and configuration |
| `SelectMajorIncident()` | 539-556 | Major Incident workflow setup and configuration |
| `GoToNextStep()` | 473-484 | Next step navigation method |
| `GoToPreviousStep()` | 486-497 | Previous step navigation method |
| `GoToStep()` | 499-511 | Direct step navigation method |

##### Complex Dependencies to Handle:
- ⚠️ **`TotalSteps`**: Depends on `IsMajorIncident` (incident-specific logic)
- ⚠️ **`CanGoNext`**: Depends on `ValidateCurrentStep()` (validation not yet extracted)
- ⚠️ **`IsStep4`**: Depends on both `CurrentStep` and `IsMajorIncident`
- ✅ **Notification Chain**: `CurrentStep` triggers 8 property change notifications

##### Base Class Design Strategy:
- ✅ **Abstract Properties**: `TotalSteps`, `CanGoNext` (derived classes implement)
- ✅ **Concrete Properties**: `IsStep1`-`IsStep3`, `CanGoPrevious`, `IsLastStep`
- ✅ **Virtual Properties**: `IsStep4`, `ShowNextButton` (can be overridden)
- ✅ **Protected Methods**: Navigation methods for derived class use
- ✅ **Breadcrumb Support**: Generic breadcrumb collection and indexing

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test step navigation (Previous/Next buttons)
- [ ] Verify all step indicators work correctly (`IsStep1`-`IsStep4`)
- [ ] Test breadcrumb navigation and indexing
- [ ] Confirm `ShowNextButton` visibility logic
- [ ] Test workflow transitions with proper notifications
- [ ] Verify `TotalSteps` and `CanGoNext` abstract implementation works

---

#### ✅ **Step 2.5: Form State Management → BaseFormStateViewModel**
**Risk**: LOW | **Impact**: HIGH | **Lines Reduced**: ~200 lines

##### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseFormStateViewModel.cs`
- [ ] Update `IncidentCreateViewModel.cs` inheritance chain

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Method/Property | Current Lines | Description |
|-----------------|---------------|-------------|
| **Core Form Properties** | | |
| `Title` | 45-51 | Main incident title with validation attributes |
| `Description` | 56-62 | Detailed incident description with validation |
| `IncidentType` | 67-70 | Type of incident (Pre-Incident/Major Incident) |
| `Priority` | 85-86 | Incident priority level |
| `Status` | 91-92 | Current incident status |
| `BusinessImpact` | 107-112 | Major incident business impact description |
| `TimeIssueStarted` | 127-130 | When the issue began (DateTimeOffset) |
| `TimeReported` | 135-138 | When incident was reported (DateTimeOffset) |
| `ImpactedUsers` | 144-147 | Number of affected users (integer) |
| `SelectedImpactedUsersCount` | 153-156 | ComboBox enum selection for user count |
| `ApplicationAffected` | 184-187 | Affected applications field |
| `LocationsAffected` | 192-195 | Affected locations field |
| `Workaround` | 200-203 | Available workaround description |
| `IncidentNumber` | 208-209 | Unique incident tracking number |
| `Urgency` | 214-217 | Urgency level (1=High, 2=Medium, 3=Low) |
| `UrgencyIndex` | 222-226 | 0-based index for ComboBox binding |
| **State Management Methods** | | |
| `ResetForm()` | 670-704 | Complete form reset with validation clearing |
| `RefreshValidationState()` | 654-668 | UI state synchronization and command refresh |
| `LoadEnumCollections()` | 326-361 | Dropdown collection initialization |
| `LoadApplicationsAsync()` | 363-383 | Async application data loading |
| `IsSubmitting` | 97-99 | Async operation state property |
| **Property Change Handlers** | | |
| `OnSelectedImpactedUsersCountChanged()` | 161-164 | Enum-to-integer synchronization handler |
| `OnImpactedUsersChanged()` | 169-179 | Integer-to-enum synchronization handler |
| **Collection Properties** | | |
| `AvailableIncidentTypes` | 300 | Dropdown data source for incident types |
| `AvailablePriorities` | 306 | Dropdown data source for priorities |
| `AvailableStatuses` | 312 | Dropdown data source for statuses |
| `AvailableImpactedUsersCounts` | 318 | Dropdown data source for user counts |
| `AvailableApplications` | 324 | Auto-suggestion data source for applications |
| **Service Dependencies** | | |
| `_incidentService` field | 17 | Readonly service field for incident operations |
| `_enumService` field | 18 | Readonly service field for enum collections |
| `_applicationService` field | 19 | Readonly service field for application auto-suggestion |
| **Initialization** | | |
| Constructor initialization | 29-40 | Dependency injection pattern and service setup |
| Service initialization order | 35-39 | LoadEnumCollections() → RefreshValidationState() sequence |
| Default value patterns | Various | Property initialization with defaults |

##### Complex Dependencies to Handle:
- ⚠️ **Collection Dependencies**: `LoadEnumCollections()` depends on `_enumService`
- ⚠️ **Async Loading**: `LoadApplicationsAsync()` depends on `_applicationService`
- ⚠️ **Validation Integration**: `ResetForm()` calls `ClearErrors()` and `RefreshValidationState()`
- ✅ **Form Reset Logic**: Generic reset patterns with field clearing

##### Base Class Design Strategy:
- ✅ **Abstract Methods**: `ResetFormFields()`, `LoadCollections()` (derived classes implement)
- ✅ **Concrete Methods**: `RefreshValidationState()`, `ClearCollections()`, base `ResetForm()`
- ✅ **Protected Properties**: `IsSubmitting`, collection loading state indicators
- ✅ **Virtual Methods**: `ResetForm()`, `LoadCollections()` (can be overridden)
- ✅ **State Coordination**: Integration with validation and workflow base classes

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test complete form reset functionality
- [ ] Verify validation state refresh after operations
- [ ] Test collection loading (enum and async data)
- [ ] Confirm `IsSubmitting` state management
- [ ] Test default value initialization patterns
- [ ] Verify integration with BaseValidatingViewModel and BaseStepWorkflowViewModel

---

#### ✅ **Step 4: Async Command Patterns → BaseAsyncCommandViewModel**
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~150 lines

##### Files to Create:
- [ ] `Presentation/ViewModels/Base/BaseAsyncCommandViewModel.cs`
- [ ] Update `IncidentCreateViewModel.cs` inheritance chain

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Method/Pattern | Current Lines | Description |
|----------------|---------------|-------------|
| **Async Commands** | | |
| `CreateIncidentAsync()` | 389-471 | Main async command with cancellation, validation, error handling |
| **Navigation Commands** | | |
| `GoToNextStep()` | 476-484 | Step navigation command with validation |
| `GoToPreviousStep()` | 489-497 | Previous step navigation command |
| `GoToStep()` | 503-511 | Direct step navigation with parameter |
| **Workflow Commands** | | |
| `SelectPreIncident()` | 516-537 | Incident type selection with workflow state changes |
| `SelectMajorIncident()` | 542-556 | Major incident selection with workflow setup |
| **State Management Commands** | | |
| `ResetForm()` | 673-704 | Complete form reset command with state clearing |
| **Command Patterns** | | |
| Cancellation support | `IncludeCancelCommand = true` | Async command cancellation infrastructure |
| CanExecute validation | `CanExecute = nameof(method)` | Command state validation patterns |
| Error message clearing | All commands | Common `ErrorMessage = null` pattern |
| Try-catch-finally patterns | `CreateIncidentAsync()` | Async operation error handling |
| State management | `IsSubmitting` patterns | Loading state management in commands |

##### Complex Dependencies to Handle:
- ⚠️ **Async Command Infrastructure**: `IncludeCancelCommand`, `CanExecute` patterns
- ⚠️ **State Integration**: Commands interact with validation, workflow, and form state
- ⚠️ **Error Handling**: Consistent error message patterns across all commands
- ✅ **Command Inheritance**: Base class provides common command infrastructure

##### Base Class Design Strategy:
- ✅ **Abstract Methods**: `ExecuteMainActionAsync()` (derived classes implement core logic)
- ✅ **Concrete Methods**: Common error handling, state management patterns
- ✅ **Protected Properties**: `IsExecuting`, command state indicators
- ✅ **Virtual Methods**: Navigation commands (can be overridden)
- ✅ **Command Infrastructure**: Cancellation, validation, error handling base patterns

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test all 7 RelayCommand methods
- [ ] Verify async command cancellation works
- [ ] Test CanExecute validation for all commands
- [ ] Confirm error handling in async operations
- [ ] Test navigation command state changes
- [ ] Verify workflow command state transitions
- [ ] Test form reset command functionality

---

### **Phase 2: Service Layer Extractions (Using Base Classes)**

#### ✅ **Step 5: Validation Logic → Services**
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~80

##### Files to Create:
- [ ] `Application/Interfaces/IIncidentValidationService.cs`
- [ ] `Infrastructure/Services/IncidentValidationService.cs`
- [ ] Update `App.xaml.cs` service registration

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Method | Current Lines | Description |
|--------|---------------|-------------|
| `ValidateCurrentStep()` | 558-572 | Validates current step for navigation |
| `ValidateStep1()` | 574-580 | Validates incident type selection |
| `ValidateStep2()` | 582-589 | Validates basic information fields |
| `ValidateStep3()` | 591-600 | Validates incident details |
| `ValidateStep4()` | 602-610 | Validates major incident checklist |
| `ValidateCommonRequiredFields()` | 614-625 | Common field validation logic |
| `CanCreateIncident()` | 627-652 | Overall validation state check |

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test all 4 steps of incident creation workflow
- [ ] Verify validation messages display correctly
- [ ] Confirm step navigation works
- [ ] Test form reset functionality

---

#### ✅ **Step 6: Form State Management → Services**
**Risk**: MEDIUM | **Impact**: HIGH | **Lines Reduced**: ~60

##### Files to Create:
- [ ] `Application/Interfaces/IIncidentFormStateService.cs`
- [ ] `Infrastructure/Services/IncidentFormStateService.cs`
- [ ] Update `App.xaml.cs` service registration

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Logic Area | Current Location | Description |
|------------|------------------|-------------|
| `TotalSteps` calculation | Property (computed) | Dynamic step count based on incident type |
| `CurrentStep` management | Property + navigation methods | Step tracking and transitions |
| `ResetForm()` logic | Lines 695-724 | Form reset to default values |
| Step navigation state | Various properties | `CanGoNext`, `CanGoPrevious`, `IsLastStep` |

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test step navigation (Previous/Next buttons)
- [ ] Verify step counting works correctly
- [ ] Test form reset functionality
- [ ] Confirm workflow state transitions

---

#### ✅ **Step 7: Data Mapping → Services**
**Risk**: LOW | **Impact**: MEDIUM | **Lines Reduced**: ~40

##### Files to Create:
- [ ] `Application/Interfaces/IIncidentFormDataMapper.cs`
- [ ] `Infrastructure/Services/IncidentFormDataMapper.cs`
- [ ] `Application/DTOs/IncidentFormData.cs`
- [ ] Update `App.xaml.cs` service registration

##### Code to Extract from `IncidentCreateViewModel.cs`:
| Logic Area | Current Location | Description |
|------------|------------------|-------------|
| Entity creation | `CreateIncidentAsync()` method (394-480) | Incident entity creation from form data |
| Property mapping | Throughout ViewModel | Form properties to domain entity mapping |
| Data transformation | Various locations | Type conversions and data formatting |

##### Testing Requirements:
- [ ] Build application successfully
- [ ] Test incident creation end-to-end
- [ ] Verify data saves correctly to database
- [ ] Confirm all form fields map to entity properly
- [ ] Test with both Pre-Incident and Major Incident types

---

## 📁 **File Structure After Refactoring**

```
OperationPrime/
├── Application/
│   ├── DTOs/
│   │   └── IncidentFormData.cs                    ✨ NEW
│   └── Interfaces/
│       ├── IIncidentValidationService.cs          ✨ NEW
│       ├── IIncidentFormStateService.cs           ✨ NEW
│       └── IIncidentFormDataMapper.cs             ✨ NEW
├── Infrastructure/
│   └── Services/
│       ├── IncidentValidationService.cs           ✨ NEW
│       ├── IncidentFormStateService.cs            ✨ NEW
│       └── IncidentFormDataMapper.cs              ✨ NEW
└── Presentation/
    └── ViewModels/
        ├── Base/
        │   ├── BaseValidatingViewModel.cs          ✨ NEW
        │   ├── BaseStepWorkflowViewModel.cs        ✨ NEW
        │   ├── BaseFormStateViewModel.cs           ✨ NEW
        │   └── BaseAsyncCommandViewModel.cs        ✨ NEW
        └── IncidentCreateViewModel.cs              🔄 REFACTORED
```

---

## 📚 **Microsoft Documentation References**

### **Essential Documentation for Implementation:**

#### **MVVM Community Toolkit**
- **URL**: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
- **Focus**: `[ObservableProperty]`, `[RelayCommand]`, `[NotifyPropertyChangedFor]` patterns
- **Critical for**: Base class implementations and property notifications

#### **Clean Architecture**
- **URL**: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures
- **Focus**: Layer separation, dependency injection, service patterns
- **Critical for**: Service layer design and dependency management

#### **WinUI 3 Data Binding**
- **URL**: https://learn.microsoft.com/en-us/windows/apps/develop/data-binding/
- **Focus**: `x:Bind` patterns, property change notifications, validation
- **Critical for**: Maintaining UI binding after refactoring

#### **Dependency Injection Guidelines**
- **URL**: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
- **Focus**: Service lifetimes, registration patterns, best practices
- **Critical for**: Service registration in `App.xaml.cs`

#### **Data Annotations**
- **URL**: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations
- **Focus**: Validation attributes, error messages, custom validation
- **Critical for**: Validation service implementation

---

## 🎯 **Success Metrics**

### **Before Refactoring:**
- ✅ `IncidentCreateViewModel.cs`: **725 lines**
- ❌ Single responsibility violations
- ❌ Difficult to test and maintain
- ❌ Code duplication potential

### **After Refactoring:**
- ✅ `IncidentCreateViewModel.cs`: **~200 lines** (65% reduction)
- ✅ **10 new files** with single responsibilities
- ✅ **3 reusable base classes** for future ViewModels
- ✅ **6 service classes** with clean separation of concerns
- ✅ **1 DTO** for clean data transfer
- ✅ **100% functionality preservation**
- ✅ **Enterprise-grade maintainability**

---

## 🛡️ **Risk Mitigation Strategy**

### **At Each Step:**
1. **Build Verification**: Must compile without errors
2. **Functional Testing**: All existing features must work identically
3. **Regression Testing**: No new bugs introduced
4. **Performance Testing**: No performance degradation
5. **Rollback Plan**: Each step can be reverted independently

### **Success Criteria:**
- ✅ Clean build with no errors or warnings
- ✅ All existing functionality works identically
- ✅ No regression in user experience
- ✅ Code is more maintainable and testable
- ✅ Follows Microsoft 2024 best practices

---

## 🚀 **Getting Started**

**Ready to begin?** Start with **Step 1: Validation Logic → Services**

1. Create the validation service interface and implementation
2. Extract validation methods from `IncidentCreateViewModel.cs`
3. Update dependency injection registration
4. Test thoroughly before proceeding to Step 2

**Estimated Timeline**: 6 steps × 2-4 hours each = 12-24 hours total development time

---

*This checklist serves as the definitive guide for the IncidentCreateViewModel refactoring initiative. Follow each step sequentially and test thoroughly at each checkpoint.*
