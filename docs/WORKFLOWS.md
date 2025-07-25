# OPERATION PRIME Workflow Guide

**Keywords**: incident workflows, pre-incident process, major incident process, NOI generation, conversion workflow, status management, wizard steps

## Table of Contents
1. [Clean Architecture Workflow Implementation](#clean-architecture-workflow-implementation)
2. [Application Layer Workflow Services](#application-layer-workflow-services)
3. [DTO-Based Workflow Data](#dto-based-workflow-data)
4. [Validation in Workflows](#validation-in-workflows)
5. [Dependency Injection in Workflows](#dependency-injection-in-workflows)
6. [Configuration-Driven Workflows](#configuration-driven-workflows)
7. [Logging & Audit in Workflows](#logging--audit-in-workflows)
8. [External Service Integration in Workflows](#external-service-integration-in-workflows)
9. [Overview](#overview)
10. [Pre-Incident Workflow](#pre-incident-workflow)
11. [Major Incident Workflow](#major-incident-workflow)
12. [NOI Generation Workflow](#noi-generation-workflow)
13. [Conversion Workflows](#conversion-workflows)
14. [Status Management](#status-management)

---

## Clean Architecture Workflow Implementation

OPERATION PRIME workflows strictly follow Clean Architecture principles:

1. **Domain Layer**: Workflow business rules and domain events
2. **Application Layer**: Workflow orchestration services and use cases
3. **Infrastructure Layer**: External service calls (Neurons integration)
4. **Presentation Layer**: UI workflow wizards and ViewModels

### Workflow Architecture Principles
- **Service Orchestration**: Application services coordinate workflow steps
- **DTO Communication**: Data flows between layers via DTOs
- **Validation Integration**: Business rules enforced at appropriate layers
- **Audit Logging**: All workflow steps logged for compliance
- **External Abstraction**: Neurons calls abstracted behind interfaces

## Application Layer Workflow Services

### Workflow Service Interfaces
```csharp
// Application layer interfaces
public interface IPreIncidentWorkflowService
{
    Task<WorkflowResultDto> StartPreIncidentAsync(CreatePreIncidentDto dto);
    Task<WorkflowResultDto> CompleteStepAsync(WorkflowStepDto step);
    Task<WorkflowResultDto> EscalateToMajorAsync(EscalateIncidentDto dto);
}

public interface IMajorIncidentWorkflowService
{
    Task<WorkflowResultDto> StartMajorIncidentAsync(CreateMajorIncidentDto dto);
    Task<NOIResultDto> GenerateNOIAsync(GenerateNOIDto dto);
    Task<WorkflowResultDto> UpdateStatusAsync(UpdateStatusDto dto);
}
```

## DTO-Based Workflow Data

### Workflow DTOs
```csharp
// Data transfer objects for workflow steps
public class CreatePreIncidentDto
{
    public string Title { get; set; }
    public string IncidentSource { get; set; }
    public DateTime IncidentStartTime { get; set; }
    public int ImpactedUsers { get; set; }
    // Additional properties...
}

public class WorkflowResultDto
{
    public bool IsSuccess { get; set; }
    public string IncidentId { get; set; }
    public List<string> ValidationErrors { get; set; }
    public string NextStepUrl { get; set; }
}
```

## Validation in Workflows

### Current: ViewModel Validation
- Data annotations on ViewModel properties
- Real-time validation in UI
- Step-by-step validation enforcement

### Future: Application Layer Validation
- FluentValidation rules for complex business logic
- Cross-step validation (e.g., escalation rules)
- Consistent validation across all entry points

## Dependency Injection in Workflows

### Service Registration
```csharp
// Workflow services registration
services.AddScoped<IPreIncidentWorkflowService, PreIncidentWorkflowService>();
services.AddScoped<IMajorIncidentWorkflowService, MajorIncidentWorkflowService>();
services.AddScoped<INOIGenerationService, NOIGenerationService>();
```

### ViewModel Dependencies
```csharp
public class IncidentWizardViewModel : BaseViewModel
{
    private readonly IPreIncidentWorkflowService _preIncidentService;
    private readonly IMajorIncidentWorkflowService _majorIncidentService;
    private readonly ILogger<IncidentWizardViewModel> _logger;
    
    // Constructor injection and implementation
}
```

## Configuration-Driven Workflows

### Workflow Configuration
```csharp
public class WorkflowOptions
{
    public int MaxStepsPerWorkflow { get; set; } = 5;
    public TimeSpan StepTimeout { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableAutoSave { get; set; } = true;
    public string DefaultPriority { get; set; } = "P3";
}
```

## Logging & Audit in Workflows

### Workflow Logging Pattern
```csharp
public async Task<WorkflowResultDto> StartPreIncidentAsync(CreatePreIncidentDto dto)
{
    _logger.LogInformation("Starting Pre-Incident workflow for user {UserId}", _currentUser.Id);
    
    // Workflow logic
    
    await _auditService.LogWorkflowStepAsync(new AuditEntryDto
    {
        Action = "PreIncident.Started",
        UserId = _currentUser.Id,
        Data = JsonSerializer.Serialize(dto)
    });
    
    return result;
}
```

## External Service Integration in Workflows

### Neurons Integration Pattern
```csharp
// Interface in Application/Domain layer
public interface INeuronsIntegrationService
{
    Task<NeuronsIncidentDto> FetchIncidentDataAsync(string incidentNumber);
    Task<bool> ValidateTokenAsync();
}

// Implementation in Infrastructure layer
public class NeuronsIntegrationService : INeuronsIntegrationService
{
    // HTTP client implementation with circuit breaker
}
```

## Overview

OPERATION PRIME supports two distinct incident management workflows: Pre-Incidents for minor issues and Major Incidents for critical events requiring comprehensive documentation.

> **📋 Implementation of these workflows must follow [Coding Standards](./CODING_STANDARDS.md) for component isolation and clean architecture patterns.**

## Pre-Incident Workflow

### Purpose
Pre-incidents handle minor issues with emphasis on quick resolution and optional escalation to Major status.

### Wizard Flow
**Trigger**: "New Pre-Incident" button from Dashboard
**Container**: Modal ContentDialog with TabView stepper

#### Step 1: Basics
**Required Fields**:
- **Title** (TextBox): Incident description (min 2 chars)
- **Incident Source** (ComboBox): Service Desk, NOC, SME, Business Escalation, Other
- **Incident Start Time** (DatePicker/TimePicker): When issue began for users
- **Time Reported to NOC** (DatePicker/TimePicker): When NOC was notified
- **Time Reported to Service Desk** (DatePicker/TimePicker): Conditional on source selection
- **Created By** (TextBox): Auto-filled from Environment.UserName, editable
- **Incident Number** (TextBox): For Neurons integration
- **Generating Multiple Calls** (RadioButtons): Yes/No selection

**Neurons Integration**:
- "Fetch from Neurons" button after incident number entry
- Auto-fills title, description, and timestamps when available
- Manual fallback if Neurons Integration unavailable or offline

#### Step 2: Details
**Additional Fields**:
- **Detailed Description** (TextArea): Comprehensive issue description
- **Impacted Users** (ComboBox): 10, 20, 30, 50, 100, Custom options
- **Custom Impacted Users** (NumberBox): Appears when "Custom" selected
- **Applications Affected** (TextBox): From managed application list, autocomplete suggestions from application list
- **Locations Affected** (TextBox/ComboBox): Geographic or network locations
- **Business Impact** (TextArea): Impact on business operations
- **Urgency Assessment** (ComboBox): High (1), Medium (2), Low (3)

**Priority Calculation**:
- Automatic calculation: Urgency × Impacted Users matrix
- Real-time updates as fields change
- Manual override option available

#### Step 3: Review
**Summary Display**:
- Read-only view of all entered data
- Calculated priority confirmation
- Edit links to return to previous steps
- Save as Draft or Submit options

### Detail View (Post-Creation)
**Tab Structure**:
- **Overview**: Key incident information with edit capabilities
- **Timeline**: Chronological updates and changes
- **Audit**: Complete change history

**Key Actions**:
- **Edit Fields**: In-place editing with validation
- **Add Timeline Entry**: Manual updates and notes
- **Convert to Major**: Escalation with data preservation
- **Close Incident**: Resolution with timestamp
- **Send Notification**: Email integration

## Major Incident Workflow

### Purpose
Major incidents require comprehensive documentation, NOI generation, and detailed tracking for critical business impacts.

### Wizard Flow
**Trigger**: "New Major Incident" button or conversion from Pre-Incident

#### Step 1: Basics
**Same as Pre-Incident fields - Can be the same creation wizard**:
**Required Fields**:
- **Title** (TextBox): Incident description (min 2 chars)
- **Incident Source** (ComboBox): Service Desk, NOC, SME, Business Escalation, Other
- **Incident Start Time** (DatePicker/TimePicker): When issue began for users
- **Time Reported to NOC** (DatePicker/TimePicker): When NOC was notified
- **Time Reported to Service Desk** (DatePicker/TimePicker): Conditional on source selection
- **Incident Number** (TextBox): For Neurons integration
- **Generating Multiple Calls** (RadioButtons): Yes/No selection
- **SME Contacted** (ComboBox): Yes or No - Subject matter expert involvement; If Yes new field appears 
- **SME Contacted Name** (TextBox with autocomplete suggestions from team member list) (Conditional on SME Contacted Yes)
- **SME Contacted Time** (Auto-populated when checked) (Conditional on SME Contacted Yes)
- **Support Team** (TextBox with autocomplete suggestions from team list)(Can suggest team based on SME Contacted Name)(Conditional on SME Contacted Yes)

#### Step 2: Details
**Enhanced fields for Major incidents**:
- **Detailed Description** (TextArea): Comprehensive issue description
- **Impacted Users** (ComboBox): 10, 20, 30, 50, 100, Custom options
- **Custom Impacted Users** (NumberBox): Appears when "Custom" selected
- **Applications Affected** (TextBox): From managed application list, autocomplete suggestions from application list
- **Locations Affected** (TextBox/ComboBox): Geographic or network locations
- **Business Impact** (TextArea): Impact on business operations - (What business/users/customers can or cannot do)
- **Urgency Assessment** (ComboBox): High (1), Medium (2), Low (3)


#### Step 3: Checklist
**Operational Tracking Grid**:
- **Declaring as Major** (ComboBox Yes/No confirmation)
- **Major Declaration Date** (Auto-set on declaringMajor Yes manual)
- **Bridge Call** (ComboBox Yes/No confirmation) - If Yes, bridge link URL will be requested, if user paster bridge URL it will be stored for the incident.

#### Step 4: NOI Preview
**Notice of Incident Generation**:
- **Template Selection**: Choose appropriate NOI template - Will use a template .docx file from the templates folder
- **Auto-Population**: Fields filled from incident data
- **Preview Display**: Formatted NOI content
- **Edit Capability**: Manual adjustments before sending
- **Distribution List**: Recipient configuration - Using team list to get email addresses, user can add email addresses manually and remove, will suggest emails based on team and team members selected. After email list is set, it will be stored for the incident.

#### Step 5: Review
**Comprehensive Summary**:
- All incident data display
- NOI preview confirmation
- Priority and impact validation
- Final submission controls

### Detail View (Post-Creation)
**Enhanced Tab Structure**:

#### Overview Tab
**Key Information Display**:
- All basic incident fields (editable)
- Priority calculation with override option
- Status tracking and updates
- Team assignments and contacts

#### Timeline Tab
**Event Chronology**:
- Automatic timestamps for key events
- Manual entry capability for updates
- Change tracking with user attribution
- Export functionality for reporting

#### Checklist Tab
**Operational Data Grid**:
- All checklist items from wizard
- Real-time status updates
- Progress tracking indicators
- Notes and comments section

#### NOI Tab
**Notice Management**:
- **Initial NOI**: First notification sent
- **Update NOI**: Interim status updates
- **Resolution NOI**: Final closure notification
- **Template Management**: NOI format customization
- **Send History**: Tracking of all sent notices
- **Recipient Management**: Distribution list control

#### Audit Tab
**Compliance Tracking**:
- Complete change history
- User action logging
- Timestamp verification
- Export for compliance reporting

## Conversion Workflow (Pre to Major)

### Trigger Conditions
- Manual conversion via "Convert to Major" button
- Automatic suggestion based on impact assessment
- Escalation from management or stakeholders

### Data Preservation
**Automatically Transferred**:
- All basic incident information
- Timeline entries and timestamps
- User assignments and contacts
- Audit trail and change history

**Additional Requirements**:
- Major-specific fields completion
- NOI template selection
- Enhanced checklist completion
- Stakeholder notification

### Conversion Process
1. **Validation**: Ensure all Pre-incident data complete
2. **Enhancement**: Add Major-specific fields
3. **NOI Setup**: Configure notice templates and recipients
4. **Checklist**: Complete operational tracking items
5. **Notification**: Alert stakeholders of escalation
6. **Audit**: Log conversion with reasoning

## Status Management

### Incident States
- **Draft**: Work in progress, not submitted
- **Open**: Active incident requiring attention
- **In Progress**: Work actively being performed
- **Pending**: Waiting for external input or action
- **Resolved**: Issue addressed, awaiting confirmation
- **Closed**: Incident fully completed and documented

### State Transitions
**Automatic Transitions**:
- Draft → Open (on submission)
- Open → In Progress (on first update)
- Resolved → Closed (after confirmation period)

**Manual Transitions**:
- Any state → Pending (waiting for input)
- In Progress → Resolved (issue addressed)
- Any state → Closed (manual closure with notes)

## Integration Points

### Neurons Integration
- **Authentication**: login to to Neurons to retrieve token and user details
- **Data Fetching**: Incident details and history
- **Offline Handling**: Graceful degradation when unavailable
- **Error Recovery**: Manual entry fallback options

### Email Integration
- **NOI Distribution**: Automated notice sending
- **Stakeholder Alerts**: Status change notifications
- **Template Management**: Customizable email formats
- **Delivery Tracking**: Confirmation and failure handling

### Audit Integration
- **Change Tracking**: All modifications logged
- **User Attribution**: Action ownership recording
- **Timestamp Verification**: Accurate time tracking
- **Compliance Export**: Formatted audit reports

---

These workflows ensure consistent incident management while providing flexibility for different incident types and organizational requirements.
