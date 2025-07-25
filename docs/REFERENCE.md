# OPERATION PRIME Reference

**Keywords**: data models, field definitions, incident fields, priority matrix, status values, enums, database schema, validation rules

## Table of Contents
1. [Clean Architecture Data Models](#clean-architecture-data-models)
2. [Domain Layer Entities](#domain-layer-entities)
3. [Application Layer DTOs](#application-layer-dtos)
4. [Infrastructure Layer Schema](#infrastructure-layer-schema)
5. [Presentation Layer Models](#presentation-layer-models)
6. [Dependency Injection Contracts](#dependency-injection-contracts)
7. [Configuration Models](#configuration-models)
8. [Validation Rules Reference](#validation-rules-reference)
9. [Data Models and Field Definitions](#data-models-and-field-definitions)
10. [Enumeration Definitions](#enumeration-definitions)
11. [Database Schema](#database-schema)
12. [API Contracts](#api-contracts)

---

## Clean Architecture Data Models

### Layer-Specific Model Organization

| Layer | Model Types | Purpose | Examples |
|-------|-------------|---------|----------|
| Domain | Entities, Value Objects, Enums | Core business logic | Incident, PreIncident, MajorIncident |
| Application | DTOs, Commands, Queries | Data transfer, use cases | IncidentDto, CreateIncidentCommand |
| Infrastructure | EF Models, Configurations | Data persistence | IncidentEntity, IncidentConfiguration |
| Presentation | ViewModels, UI Models | User interface binding | IncidentViewModel, IncidentDisplayModel |

## Domain Layer Entities

### Core Domain Models
- **Incident**: Base entity with shared properties
- **PreIncident**: Lightweight incident for minor issues
- **MajorIncident**: Comprehensive incident with full tracking
- **Priority**: Value object for priority calculation
- **AuditEntry**: Domain event for change tracking

## Application Layer DTOs

### Data Transfer Objects
- **IncidentDto**: Cross-layer data transfer
- **NOIDto**: Notice of Incident generation data
- **PriorityCalculationDto**: Priority matrix computation data
- **ValidationResultDto**: Validation outcome transfer

## Infrastructure Layer Schema

### Database Entities
- **IncidentEntity**: EF Core entity mapping
- **AuditLogEntity**: Persistence audit trail
- **ConfigurationEntity**: Application settings storage

## Presentation Layer Models

### UI-Specific Models
- **IncidentViewModel**: MVVM binding model
- **WizardStepModel**: UI wizard navigation
- **ValidationDisplayModel**: UI validation feedback

## Dependency Injection Contracts

### Service Interfaces
```csharp
// Domain/Application Interfaces
public interface IIncidentRepository
public interface IIncidentService
public interface ITokenProviderService
public interface IAuditService
public interface IPriorityCalculationService
```

## Configuration Models

### Options Pattern Classes
```csharp
public class DatabaseOptions
public class NeuronsOptions
public class LoggingOptions
public class SecurityOptions
```

## Validation Rules Reference

### Toolkit Validation (Current)
- Data annotations on ViewModels
- ObservableValidator base class
- Real-time UI validation

### FluentValidation (Future)
- Application layer validation
- Complex business rules
- Cross-field validation logic

## Data Models and Field Definitions

### Core Incident Model

#### Base Incident Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `incidentId` | string | Auto-generated unique identifier | "Incident ID" (display only) |
| `title` | string | Incident title/summary | "Incident Title" (min 5 chars) |
| `incidentNumber` | string | External system reference | "Incident Number" |
| `incidentType` | enum | Pre-Incident or Major-Incident | "Incident Type" (display only) |
| `schemaVersion` | string | Data model version | Internal use only |
| `incidentStatus` | enum | Current incident state | "Incident Status" |
| `incidentPriority` | enum | P1, P2, P3, P4 priority | "Incident Priority" |
| `incidentUrgency` | enum | High (1), Medium (2), Low (3) | "Urgency Assessment" |
| `createdNoteTime` | DateTime | Creation timestamp | "Created Time" (display only) |
| `updatedNoteTime` | DateTime | Last modification time | "Last Updated Time" (display only) |

#### Timing Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `incidentStartTime` | DateTime | When issue began for users | "Incident Start Time" |
| `timeReportedToNOC` | DateTime | NOC notification time | "Time Reported to NOC" |
| `timeReportedToServiceDesk` | DateTime? | Service desk notification (conditional) | "Time Reported to Service Desk" |
| `preIncidentEmailSentTime` | DateTime? | Pre-incident notification sent | "Pre-Incident Email Sent Time" (display) |
| `smeContactedTime` | DateTime? | SME contact timestamp | "SME Contacted Time" (display) |
| `resolvedTime` | DateTime? | Resolution timestamp | "Resolved Time" (display) |

#### Impact and Source Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `incidentSource` | enum | Service Desk, NOC, SME, Business Escalation, Other | "Source" |
| `customSource` | string? | Custom source description | "Custom Source" (conditional) |
| `createdBy` | string | NOC analyst name | "Created By" (auto-filled, editable) |
| `impactedUsers` | int | Number of affected users | "Impacted Users" |
| `customImpactedUsers` | int? | Custom user count | "Custom Impacted Users" (conditional) |
| `applicationsAffected` | string | Affected applications with autocomplete | "Applications Affected" (TextBox with autocomplete) |
| `locationsAffected` | string | Geographic/network locations | "Locations Affected" |
| `businessImpact` | string | Business impact description | "Business Impact" |

### Major Incident Extensions

#### Major-Specific Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `majorDeclarationDate` | DateTime | When declared as major | "Major Date (Declaration)" (display) |
| `declaringMajor` | bool | Confirmation of major status | "Declaring as Major Incident" |
| `supportTeam` | string | Support team with autocomplete suggestions | "Support Team" (TextBox with autocomplete, conditional on SME) |
| `teamResponsible` | string | Primary responsible team | "Team Responsible" |
| `incidentCategory` | enum | Incident categorization | "Incident Category" |
| `teamsCall` | bool | Bridge call initiated | "Teams Call" |
| `smeContacted` | bool | SME involvement flag | "SME Contacted?" |
| `smeContactedName` | string | SME name | "SME Contacted Name" |
| `smeContactedTime` | DateTime | SME contact timestamp | "SME Contacted Time" |

#### NOI (Notice of Incident) Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `noiGenerated` | bool | NOI generation status | "NOI Generated" (display) |
| `noiSentTime` | DateTime? | NOI distribution time | "NOI Sent Time" (display) |
| `noiRecipients` | string[] | Email distribution list | "NOI Recipients" (team-based suggestions, manual add/remove) |
| `noiTemplate` | string | Template used for generation | "NOI Template" (.docx file selection) |
| `noiContent` | string | Generated NOI content | "NOI Content" (preview/edit) |
| `bridgeUrl` | string? | Bridge call URL | "Bridge URL" (conditional on bridge call confirmation) |

### NOI Template Field Mapping

**Template .docx Structure**: NOI templates use the following field mappings for auto-population:

| Template Field | Source Field | Data Type | Behavior |
|----------------|--------------|-----------|----------|
| Incident Priority | `incidentPriority` | ComboBox/string | Prefilled from urgency × users matrix, manual override |
| Incident Start Time | `incidentStartTime` | DateTime | Prefilled from Basics/Main step |
| Date/Time Reported | `timeReportedToNOC` | DateTime | Prefilled from Basics/Main step |
| Date/Time of Service Restoration | (blank) | DateTime? | Display only until set on close |
| RCA Owner | (blank) | string | Display only until set on resolution |
| Incident Description | `detailedDescription` | string | Auto-generated or manual override |
| Location(s) Impacted | `locationsAffected` | string | Prefilled from Details/Main step |
| Total Number of Users Impacted | `impactedUsers` | number | Prefilled from Details/Main step |
| Primary Incident Number | `incidentNumber` | string | Prefilled from Basics/Main step |
| Team(s) Assigned | `supportTeam` | string[] | Prefilled from Checklist step |
| Known Workaround(s) | `availableWorkaround` | string | Prefilled from Details/Main step |
| Status | (auto-generated) | string | "Initial Time [Current DateTime] - Support teams are investigating the issue." |

**Auto-Generated Incident Description Template**:
```
<applicationsAffected> users are experiencing issues. The business impact is that <businessImpact>.
```
*Note: Users can manually override this auto-generated description.*

#### Operational Tracking Fields
| Field Name | Data Type | Description | User Prompt/Label |
|------------|-----------|-------------|-------------------|
| `incidentMttr` | string | Mean Time To Resolution | "MTTR" |
| `triggeredAlert` | bool | SolarWinds alert status | "Triggered Alert" (display) |
| `relatedChange` | bool | Related to change management | "Related to Change" |
| `changeNumber` | string? | Change management number | "Change Number" (conditional) |
| `bridgeNotes` | string | Meeting and troubleshooting notes | "Bridge Notes" |
| `followUpTeamMeeting` | bool | Post-incident meeting required | "Follow Up Team Meeting" |
| `bridgeAttendants` | string | Meeting participants list | "Bridge Attendants" |
| `preIncidentEmailSent` | bool | Pre-incident notification flag | "Pre-Incident Email Sent" (display) |

### Enumeration Definitions

#### IncidentStatus
```csharp
public enum IncidentStatus
{
    Draft,
    Open,
    InProgress,
    Pending,
    Resolved,
    Closed
}
```

#### IncidentPriority
```csharp
public enum IncidentPriority
{
    P1, // Critical
    P2, // High
    P3, // Medium
    P4  // Low
}
```

#### IncidentUrgency
```csharp
public enum IncidentUrgency
{
    High = 1,
    Medium = 2,
    Low = 3
}
```

#### IncidentSource
```csharp
public enum IncidentSource
{
    ServiceDesk,
    NOC,
    SME,
    BusinessEscalation,
    Other
}
```

#### IncidentType
```csharp
public enum IncidentType
{
    PreIncident,
    MajorIncident
}
```

## Priority Calculation Matrix

### Matrix Definition
The priority calculation uses a 2D matrix based on Urgency (rows) and Impacted Users (columns):

| Urgency \ Users | 1-10 | 11-50 | 51-100 | 101-500 | 500+ |
|-----------------|------|-------|--------|---------|------|
| High (1)        | P2   | P1    | P1     | P1      | P1   |
| Medium (2)      | P3   | P2    | P2     | P1      | P1   |
| Low (3)         | P4   | P3    | P3     | P2      | P2   |

### Calculation Logic
```csharp
public string CalculatePriority(IncidentUrgency urgency, int impactedUsers, string? overridePriority = null)
{
    // Override takes precedence
    if (!string.IsNullOrEmpty(overridePriority))
        return overridePriority;
    
    // Matrix lookup based on urgency and user count
    int urgencyIndex = (int)urgency - 1; // Convert to 0-based
    int userIndex = GetUserRangeIndex(impactedUsers);
    
    return _priorityMatrix[urgencyIndex, userIndex];
}
```

## Timeline and Audit Models

### Timeline Entry
| Field Name | Data Type | Description |
|------------|-----------|-------------|
| `timelineId` | string | Unique entry identifier |
| `incidentId` | string | Parent incident reference |
| `timestamp` | DateTime | Entry creation time |
| `user` | string | User who created entry |
| `action` | string | Action performed |
| `description` | string | Detailed description |
| `systemGenerated` | bool | Auto vs manual entry |

### Audit Log Entry
| Field Name | Data Type | Description |
|------------|-----------|-------------|
| `auditId` | string | Unique audit identifier |
| `incidentId` | string | Parent incident reference |
| `timestamp` | DateTime | Change timestamp |
| `user` | string | User who made change |
| `field` | string | Changed field name |
| `oldValue` | string? | Previous value |
| `newValue` | string? | New value |
| `action` | string | Type of change |

## Neurons Integration Endpoints

### Neurons Integration
**Base URL**: tenantname.ivantcloud.com
**Authentication**: login to retrieve a token to use to fetch data

#### Get Incident Details
```
GET /api/odata/businessobject/Incidents?filter=IncidentNumber eq '{incidentNumber}'
Response: IncidentDetailsDto
```

#### Search Incidents
```
GET /api/odata/businessobject/incidents/search?query={searchTerm}
Response: IncidentSearchResultDto[]
```

### Internal Service Interfaces

#### IPriorityService
```csharp
public interface IPriorityService
{
    string CalculatePriority(IncidentUrgency urgency, int impactedUsers, string? overridePriority = null);
    bool ValidateUrgency(string urgency);
    bool ValidateImpactedUsers(int users);
}
```

#### INOIService
```csharp
public interface INOIService
{
    string GenerateNOI(IncidentViewModel incident, string templateType);
    string PreviewNOI(IncidentViewModel incident);
    bool SendNOI(string content, string[] recipients);
    string[] GetAvailableTemplates();
}
```

#### IAuditService
```csharp
public interface IAuditService
{
    void LogChange(string incidentId, string field, string? oldValue, string? newValue, string user);
    void LogAction(string incidentId, string action, string description, string user);
    AuditLogEntry[] GetAuditTrail(string incidentId);
    void ExportAuditLog(string incidentId, string filePath);
}
```

## Validation Rules

### Field Validation
| Field | Rules | Error Messages |
|-------|-------|----------------|
| `title` | Required, min 5 chars, max 200 chars | "Title is required and must be 5-200 characters" |
| `incidentNumber` | Required, alphanumeric | "Incident number is required and must be alphanumeric" |
| `impactedUsers` | Required, > 0, < 5000 | "Impacted users must be between 1 and 5,000" |
| `incidentStartTime` | Required, not future date | "Start time is required and cannot be in the future" |
| `timeReportedToNOC` | Required, >= incidentStartTime | "NOC report time must be after incident start time" |

### Business Rules
- Pre-incidents can have impacted users > 100; conidtional if user overrides, incident is convert to a major -> Major Incident Workflow
- Major incidents require NOI generation recommended before closure
- Conversion from Pre to Major preserves all original data
- Audit trail cannot be modified after creation
- Priority overrides suggest fors justification in notes

---

This reference provides comprehensive field definitions and validation rules for OPERATION PRIME incident management system.
