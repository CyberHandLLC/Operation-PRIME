# OPERATION PRIME User Interface Guide

**Keywords**: user interface, WinUI3 design, navigation structure, dashboard components, wizard design, responsive layout, dark theme, accessibility

## Table of Contents
1. [Clean Architecture UI Design](#clean-architecture-ui-design)
2. [Presentation Layer Components](#presentation-layer-components)
3. [MVVM UI Patterns](#mvvm-ui-patterns)
4. [UI Validation Implementation](#ui-validation-implementation)
5. [DTO-Based UI Data Flow](#dto-based-ui-data-flow)
6. [Dependency Injection in UI](#dependency-injection-in-ui)
7. [UI Configuration Management](#ui-configuration-management)
8. [UI Testing Patterns](#ui-testing-patterns)
9. [Design Philosophy](#design-philosophy)
10. [Main Application Layout](#main-application-layout)
11. [Dashboard Components](#dashboard-components)
12. [Incident Creation Wizard](#incident-creation-wizard)
13. [Responsive Design](#responsive-design)
14. [Accessibility Features](#accessibility-features)

---

## Clean Architecture UI Design

OPERATION PRIME's UI strictly follows Clean Architecture principles in the Presentation layer:

1. **Presentation Layer Isolation**: UI components depend only on Application layer DTOs and services
2. **MVVM Pattern**: ViewModels use ObservableValidator and [ObservableProperty] partials
3. **Dependency Injection**: All ViewModels and UI services are registered and injected
4. **Configuration-Driven**: UI settings managed through IOptions<T> pattern
5. **Validation Integration**: UI validation tied to Application layer validation rules
6. **DTO Consumption**: ViewModels consume DTOs from Application services
7. **Logging Integration**: UI actions logged through ILogger<T>
8. **Service Abstraction**: UI calls Application services, never Infrastructure directly

## Presentation Layer Components

### ViewModels Architecture
- **BaseViewModel**: Inherits from ObservableValidator for validation support
- **Page ViewModels**: Scoped to specific UI pages/workflows
- **Shared ViewModels**: Singleton for global state management
- **Command Implementation**: Use [RelayCommand] and IAsyncRelayCommand

### View Structure
- **XAML Views**: WinUI 3 with proper data binding
- **User Controls**: Reusable UI components
- **ContentDialogs**: Modal wizards and forms
- **NavigationView**: Main application shell

## MVVM UI Patterns

### ViewModel Implementation
```csharp
public partial class IncidentViewModel : BaseViewModel
{
    private readonly IIncidentService _incidentService;
    private readonly ILogger<IncidentViewModel> _logger;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
    private string title = string.Empty;
    
    [RelayCommand]
    private async Task SaveIncidentAsync()
    {
        var dto = new CreateIncidentDto { Title = Title };
        await _incidentService.CreateIncidentAsync(dto);
    }
}
```

## UI Validation Implementation

### Current: Toolkit Validation
- Data annotations on ViewModel properties
- ObservableValidator base class for real-time validation
- UI binding to HasErrors and error messages

### Future: Application Layer Integration
- FluentValidation rules in Application layer
- DTO validation before service calls
- Consistent validation across UI and API boundaries

## DTO-Based UI Data Flow

### Data Flow Pattern
1. **UI Input**: User interacts with View
2. **ViewModel Processing**: ViewModel creates DTO from UI data
3. **Service Call**: ViewModel calls Application service with DTO
4. **Response Handling**: Service returns DTO, ViewModel updates UI
5. **UI Update**: View reflects changes through data binding

## Dependency Injection in UI

### ViewModel Registration
```csharp
// In App.xaml.cs or DI configuration
services.AddTransient<IncidentViewModel>();
services.AddTransient<DashboardViewModel>();
services.AddSingleton<NavigationViewModel>();
```

### Service Injection
```csharp
public IncidentViewModel(
    IIncidentService incidentService,
    ILogger<IncidentViewModel> logger,
    IOptions<UIOptions> uiOptions)
{
    _incidentService = incidentService;
    _logger = logger;
    _uiOptions = uiOptions.Value;
}
```

## UI Configuration Management

### UI Options Pattern
```csharp
public class UIOptions
{
    public string DefaultTheme { get; set; } = "Dark";
    public int AutoSaveInterval { get; set; } = 30;
    public bool EnableAnimations { get; set; } = true;
}
```

## UI Testing Patterns

### ViewModel Testing
- Unit tests for ViewModel logic
- Mock Application services
- Test command execution and validation

### UI Integration Testing
- Test complete user workflows
- Validate data binding and navigation
- Test error handling and validation display

## Design Philosophy

OPERATION PRIME's interface prioritizes efficiency and clarity for network operations personnel. The design follows modern Windows UI guidelines with dark/light theme support and responsive layouts.

## Main Application Layout

### Navigation Structure
- **Side Navigation**: Fixed left sidebar with collapsible design
- **Main Content**: Dynamic content area with tabbed interfaces
- **Status Bar**: Bottom panel for system status and quick notes

### Navigation Menu Items
| Component | Icon | Function | Sub-Items |
|-----------|------|----------|-----------|
| Dashboard | 🏠 | Home overview with quick actions | - |
| Incidents | 📋 | Incident management | Pre-Incidents, Major Incidents |
| Teams | 👥 | Team and member management | - |
| Applications | 💻 | Application catalog management | - |
| Business Units | 🏢 | Network contact information | Coming Soon |
| Servers | 🖥️ | Server information management | Coming Soon |
| Pingdom Reports | 📊 | Website monitoring reports | Coming Soon |
| Settings & Profile | ⚙️ | Configuration and user settings | - |

## Dashboard Components

### Unified Incident Creation Wizard

**Design Approach**: Single wizard that adapts based on incident type selection, with conditional fields appearing for Major incidents.

#### Wizard Structure
- **Step 1: Basics** - Core incident information (same for Pre and Major)
- **Step 2: Details** - Enhanced fields for Major incidents, simplified for Pre-incidents
- **Step 3: Checklist** - Major incident specific operational tracking
- **Step 4: NOI Preview** - Template-based NOI generation (Major incidents only)

#### Autocomplete Behaviors
- **Applications Affected**: TextBox with autocomplete from managed application list
- **SME Contacted Name**: TextBox with autocomplete from team member list
- **Support Team**: TextBox with suggestions based on selected SME
- **Email Distribution**: Team-based email suggestions with manual add/remove capability

#### Conditional Field Logic
- **SME Fields**: Appear only when "SME Contacted" = Yes
- **Support Team**: Suggests teams based on SME selection
- **Bridge URL**: Appears only when "Bridge Call" = Yes
- **Custom Impacted Users**: Appears when "Custom" selected from dropdown

#### NOI Template Integration
- **Template Selection**: Choose from .docx templates in templates folder
- **Auto-Population**: Fields automatically filled from incident data
- **Description Generation**: Auto-generates using template: "<applicationsAffected> users are experiencing issues. The business impact is that <businessImpact>."
- **Manual Override**: Users can edit any auto-generated content
- **Email Distribution**: Smart suggestions based on team selections
**Purpose**: Quick incident creation with one-click type selection

**Design**: Compact card layout with prominent action buttons
- **Create Pre Button**: Blue styling for minor incidents
- **Create Major Button**: Red styling for critical incidents
- **Expandable Details**: Additional fields revealed on demand
- **Responsive Design**: Stacks vertically on narrow screens

**Fields**:
- Incident Number (TextBox with Neurons query integration)
- Impacted Users (Dropdown: 10, 20, 30, 50, 100, Custom)
- Applications Affected (Multi-select autocomplete)
- Locations (ComboBox with predefined options)
- Summary (TextArea: "What users can/cannot do")

### Status Cards
**Layout**: Responsive column grid with color-coded priorities

#### Changes Today Card
- Shows 3 most recent changes
- Timestamp and user badges
- Quick action buttons (View, Edit)
- "View All" expansion option

#### Active Major Incidents Card
- Priority color coding (Red: P1, Orange: P2, Yellow: P3)
- Essential info display (ID, Title, Status)
- Hover effects for additional details
- Direct navigation to incident details

#### Active Pre-Incidents Card
- Simplified display for minor incidents
- Status indicators and quick actions
- Conversion to Major option visible

### Persistent Notes Panel
**Position**: Fixed bottom panel, always accessible
**Features**:
- Auto-timestamping with user tracking
- Checkbox items for task management
- Click-to-copy functionality
- Collapsible to icon on small screens
- Dark mode compatibility

## Incident Creation Wizard

### Modal Design
- **Container**: ContentDialog for modal presentation
- **Navigation**: TabView stepper with progress indication
- **Validation**: Real-time field validation with inline errors
- **Actions**: Next/Previous navigation with Save Draft option

### Wizard Steps

#### Step 1: Basics
**Layout**: Form with grouped fields
- Required field indicators (*)
- Inline validation messages
- Neurons integration button for auto-fill
- Conditional field display based on selections

#### Step 2: Details
**Layout**: Expanded form with categorized sections
- Multi-select components for teams and applications
- Date/time pickers with validation
- TextArea components for descriptions
- Priority calculation display

#### Step 3: Checklist (Major Only)
**Layout**: Grid-based checklist with categories
- Boolean checkboxes for operational items
- Conditional fields based on selections
- Progress tracking for completion
- Notes sections for additional context

#### Step 4: Review
**Layout**: Summary view with edit links
- Read-only field display
- Priority calculation confirmation
- NOI preview (Major incidents)
- Final submission controls

## Incident Detail Views

### Tab Structure
- **Overview**: Key incident information
- **Timeline**: Chronological event history
- **Checklist**: Operational tracking (Major only)
- **NOI**: Notice generation and preview (Major only)
- **Audit**: Change history and compliance log

### Field Display Patterns
- **Read-Only**: Grayed background with clear labeling
- **Editable**: Standard input styling with save indicators
- **Calculated**: Distinct styling for auto-generated values
- **Conditional**: Dynamic show/hide based on incident type

## Form Controls and Validation

### Input Components
| Control Type | Use Case | Validation |
|--------------|----------|------------|
| TextBox | Single-line text input | Min/max length, required |
| TextArea | Multi-line descriptions | Character limits |
| ComboBox | Single selection from list | Required selection |
| Multi-Select | Multiple item selection | Minimum selection count |
| DatePicker | Date selection | Date range validation |
| TimePicker | Time selection | Format validation |
| NumberBox | Numeric input | Range validation |
| RadioButtons | Exclusive choice | Required selection |
| CheckBox | Boolean options | Conditional requirements |

### Validation Patterns
- **Real-Time**: Validation on field blur
- **Visual Indicators**: Red borders and error messages
- **Form-Level**: Submit button disabled until valid
- **Error Summary**: Consolidated error display

## Responsive Design

### Breakpoints
- **Desktop**: > 1200px - Full layout with sidebar
- **Tablet**: 768px - 1200px - Collapsible sidebar
- **Mobile**: < 768px - Hidden sidebar with hamburger menu

### Adaptive Components
- **Navigation**: Collapsible sidebar to hamburger menu
- **Cards**: Column stacking on narrow screens
- **Forms**: Single-column layout on mobile
- **Tables**: Horizontal scrolling with sticky headers

## Accessibility Features

### Keyboard Navigation
- Tab order follows logical flow
- All interactive elements keyboard accessible
- Shortcut keys for common actions
- Focus indicators clearly visible

### Screen Reader Support
- Semantic HTML structure
- ARIA labels for complex components
- Form field associations
- Status announcements for dynamic content

## Theme Support

### Dark Mode
- High contrast color scheme
- Consistent component styling
- System theme detection
- Manual theme toggle option

### Light Mode
- Standard Windows styling
- Accessibility-compliant contrast ratios
- Professional appearance
- Consistent with system themes

---

This UI guide ensures OPERATION PRIME provides an intuitive, efficient interface for network operations personnel while maintaining accessibility and professional appearance standards.
