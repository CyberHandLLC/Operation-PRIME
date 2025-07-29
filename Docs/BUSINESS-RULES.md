# 06 - Business Logic & Complex Systems

**Navigation**: [← Reference](./QUICK-REFERENCE.md) | [↑ Home](./README-DOCS.md)

## Overview

This document contains detailed implementations of Operation Prime's complex business logic systems, including priority calculation, NOI generation, email distribution, and workflow processing.

## Table of Contents

1. [Priority Calculation System](#priority-calculation-system)
2. [NOI Generation System](#noi-generation-system)
3. [Email Distribution Logic](#email-distribution-logic)
4. [Workflow Processing](#workflow-processing)
5. [Data Validation & Business Rules](#data-validation--business-rules)
6. [Integration Patterns](#integration-patterns)

---

## Priority Calculation System

### 4-Layer Architecture

The priority calculation system implements a robust 4-layer approach:

1. **Input Validation Layer**
2. **Matrix Computation Layer**
3. **Override Logic Layer**
4. **Real-time Integration Layer**

### Implementation

```csharp
// Domain/Services/PriorityMatrixService.cs
public class PriorityMatrixService : IPriorityMatrixService
{
    private readonly ILogger<PriorityMatrixService> _logger;
    private readonly IAuditService _auditService;
    
    // Priority Matrix: [Urgency, ImpactedUsers] -> Priority
    private readonly string[,] _priorityMatrix = new string[3, 5] 
    {
        // Users:     1-50   51-200  201-500  501-2000  2001-5000
        {"P2", "P1", "P1", "P1", "P1"},  // High urgency (1)
        {"P3", "P2", "P2", "P1", "P1"},  // Medium urgency (2)
        {"P4", "P3", "P3", "P2", "P2"}   // Low urgency (3)
    };

    public PriorityMatrixService(ILogger<PriorityMatrixService> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task<PriorityResult> CalculatePriorityAsync(PriorityRequest request)
    {
        _logger.LogInformation("Calculating priority for incident {IncidentId}", request.IncidentId);
        
        // Layer 1: Input Validation
        var validationResult = ValidateInput(request);
        if (!validationResult.IsValid)
        {
            return PriorityResult.Invalid(validationResult.Errors);
        }

        // Layer 2: Matrix Computation
        var matrixPriority = ComputeMatrixPriority(request.Urgency, request.ImpactedUsers);
        
        // Layer 3: Override Logic
        var finalPriority = ApplyOverrideLogic(matrixPriority, request);
        
        // Layer 4: Real-time Integration
        await LogPriorityCalculation(request, finalPriority);
        
        return PriorityResult.Success(finalPriority, GetPriorityExplanation(request, finalPriority));
    }

    private ValidationResult ValidateInput(PriorityRequest request)
    {
        var errors = new List<string>();
        
        if (request.Urgency < 1 || request.Urgency > 3)
            errors.Add("Urgency must be between 1 (High) and 3 (Low)");
            
        if (request.ImpactedUsers < 1 || request.ImpactedUsers > 5000)
            errors.Add("Impacted users must be between 1 and 5,000");
            
        return new ValidationResult(errors.Count == 0, errors);
    }

    private string ComputeMatrixPriority(int urgency, int impactedUsers)
    {
        var urgencyIndex = urgency - 1; // Convert to 0-based index
        var userIndex = GetUserRangeIndex(impactedUsers);
        
        return _priorityMatrix[urgencyIndex, userIndex];
    }

    private int GetUserRangeIndex(int impactedUsers)
    {
        return impactedUsers switch
        {
            <= 50 => 0,
            <= 200 => 1,
            <= 500 => 2,
            <= 2000 => 3,
            <= 5000 => 4,
            _ => 4 // Cap at highest range
        };
    }

    private string ApplyOverrideLogic(string matrixPriority, PriorityRequest request)
    {
        // Business rule overrides
        if (request.IsSecurityIncident && matrixPriority != "P1")
        {
            _logger.LogInformation("Security incident override: {Original} -> P1", matrixPriority);
            return "P1";
        }
        
        if (request.IsDataCenterOutage)
        {
            _logger.LogInformation("Data center outage override: {Original} -> P1", matrixPriority);
            return "P1";
        }
        
        return matrixPriority;
    }
}

// Supporting classes
public class PriorityRequest
{
    public string IncidentId { get; set; }
    public int Urgency { get; set; } // 1=High, 2=Medium, 3=Low
    public int ImpactedUsers { get; set; }
    public bool IsSecurityIncident { get; set; }
    public bool IsDataCenterOutage { get; set; }
    public string CreatedBy { get; set; }
}

public class PriorityResult
{
    public bool IsValid { get; set; }
    public string Priority { get; set; }
    public string Explanation { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static PriorityResult Success(string priority, string explanation) =>
        new() { IsValid = true, Priority = priority, Explanation = explanation };
        
    public static PriorityResult Invalid(List<string> errors) =>
        new() { IsValid = false, Errors = errors };
}
```

---

## NOI Generation System

### Builder Pattern Implementation

The NOI (Notice of Incident) generation system uses a builder pattern with .docx template processing:

```csharp
// Application/Services/NOIBuilder.cs
public class NOIBuilder : INOIBuilder
{
    private readonly ILogger<NOIBuilder> _logger;
    private readonly ITemplateService _templateService;
    private readonly IEmailService _emailService;
    private Incident _incident;
    private string _templatePath;
    private Dictionary<string, string> _fieldMappings = new();

    public NOIBuilder(ILogger<NOIBuilder> logger, ITemplateService templateService, IEmailService emailService)
    {
        _logger = logger;
        _templateService = templateService;
        _emailService = emailService;
    }

    public INOIBuilder WithIncident(Incident incident)
    {
        _incident = incident ?? throw new ArgumentNullException(nameof(incident));
        return this;
    }

    public INOIBuilder WithTemplate(string templatePath)
    {
        _templatePath = templatePath ?? throw new ArgumentNullException(nameof(templatePath));
        return this;
    }

    public INOIBuilder WithFieldMapping(string placeholder, string value)
    {
        _fieldMappings[placeholder] = value ?? string.Empty;
        return this;
    }

    public string BuildPreview()
    {
        ValidateBuilder();
        
        var mappings = BuildFieldMappings();
        return _templateService.ProcessTemplate(_templatePath, mappings);
    }

    public async Task ExportToWordAsync(string filePath)
    {
        ValidateBuilder();
        
        var mappings = BuildFieldMappings();
        await _templateService.ExportToWordAsync(_templatePath, mappings, filePath);
        
        _logger.LogInformation("NOI exported to {FilePath} for incident {IncidentId}", 
            filePath, _incident.IncidentNumber);
    }

    public async Task<EmailResult> SendEmailAsync(string[] recipients, string subject = "")
    {
        ValidateBuilder();
        
        var noiContent = BuildPreview();
        var emailSubject = subject ?? $"NOI - {_incident.Title} ({_incident.IncidentNumber})";
        
        var emailRequest = new EmailRequest
        {
            Recipients = recipients,
            Subject = emailSubject,
            Body = noiContent,
            IsHtml = false,
            Attachments = new List<EmailAttachment>()
        };

        var result = await _emailService.SendEmailAsync(emailRequest);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("NOI email sent successfully for incident {IncidentId} to {RecipientCount} recipients", 
                _incident.IncidentNumber, recipients.Length);
        }
        else
        {
            _logger.LogError("Failed to send NOI email for incident {IncidentId}: {Error}", 
                _incident.IncidentNumber, result.ErrorMessage);
        }
        
        return result;
    }

    private void ValidateBuilder()
    {
        if (_incident == null)
            throw new InvalidOperationException("Incident must be set before building NOI");
        if (string.IsNullOrEmpty(_templatePath))
            throw new InvalidOperationException("Template path must be set before building NOI");
    }

    private Dictionary<string, string> BuildFieldMappings()
    {
        var mappings = new Dictionary<string, string>
        {
            ["{{INCIDENT_NUMBER}}", _incident.IncidentNumber],
            ["{{TITLE}}", _incident.Title],
            ["{{PRIORITY}}", _incident.Priority],
            ["{{START_TIME}}", _incident.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "TBD"],
            ["{{BUSINESS_IMPACT}}", _incident.BusinessImpact ?? "Under investigation"],
            ["{{AFFECTED_SYSTEMS}}", string.Join(", ", _incident.AffectedSystems ?? new List<string>())],
            ["{{IMPACTED_USERS}}", _incident.ImpactedUsers?.ToString() ?? "0"],
            ["{{CREATED_BY}}", _incident.CreatedBy],
            ["{{CREATED_DATE}}", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")],
            ["{{STATUS}}", _incident.Status],
            ["{{DESCRIPTION}}", _incident.Description ?? "Investigation in progress"]
        };

        // Add custom field mappings
        foreach (var mapping in _fieldMappings)
        {
            mappings[mapping.Key] = mapping.Value;
        }

        return mappings;
    }
}

// Template Service for .docx processing
public class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ILogger<TemplateService> logger)
    {
        _logger = logger;
    }

    public string ProcessTemplate(string templatePath, Dictionary<string, string> fieldMappings)
    {
        try
        {
            var templateContent = File.ReadAllText(templatePath);
            
            foreach (var mapping in fieldMappings)
            {
                templateContent = templateContent.Replace(mapping.Key, mapping.Value);
            }
            
            return templateContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process template {TemplatePath}", templatePath);
            throw;
        }
    }

    public async Task ExportToWordAsync(string templatePath, Dictionary<string, string> fieldMappings, string outputPath)
    {
        // Implementation would use DocumentFormat.OpenXml for .docx processing
        // This is a simplified version for demonstration
        
        var processedContent = ProcessTemplate(templatePath, fieldMappings);
        await File.WriteAllTextAsync(outputPath, processedContent);
        
        _logger.LogInformation("Template exported to {OutputPath}", outputPath);
    }
}
```

---

## Email Distribution Logic

### Team-Based Email Suggestions

```csharp
// Application/Services/EmailDistributionService.cs
public class EmailDistributionService : IEmailDistributionService
{
    private readonly ILogger<EmailDistributionService> _logger;
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IConfigurationService _configurationService;

    public EmailDistributionService(
        ILogger<EmailDistributionService> logger,
        ITeamMemberRepository teamMemberRepository,
        IConfigurationService configurationService)
    {
        _logger = logger;
        _teamMemberRepository = teamMemberRepository;
        _configurationService = configurationService;
    }

    public async Task<List<string>> GetSuggestedEmailsAsync(string[] supportTeams)
    {
        var suggestedEmails = new List<string>();
        
        foreach (var team in supportTeams)
        {
            var teamMembers = await _teamMemberRepository.GetTeamMembersAsync(team);
            var teamEmails = teamMembers.Select(m => m.Email).Where(e => !string.IsNullOrEmpty(e));
            suggestedEmails.AddRange(teamEmails);
        }

        // Add default distribution lists
        var defaultLists = await _configurationService.GetDefaultDistributionListsAsync();
        suggestedEmails.AddRange(defaultLists);

        // Remove duplicates and sort
        return suggestedEmails.Distinct().OrderBy(e => e).ToList();
    }

    public async Task<EmailValidationResult> ValidateEmailListAsync(string[] emails)
    {
        var validEmails = new List<string>();
        var invalidEmails = new List<string>();

        foreach (var email in emails)
        {
            if (IsValidEmail(email))
            {
                validEmails.Add(email);
            }
            else
            {
                invalidEmails.Add(email);
            }
        }

        return new EmailValidationResult
        {
            ValidEmails = validEmails,
            InvalidEmails = invalidEmails,
            IsValid = invalidEmails.Count == 0
        };
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

// Supporting classes
public class TeamMember
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Team { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
}

public class EmailValidationResult
{
    public List<string> ValidEmails { get; set; } = new();
    public List<string> InvalidEmails { get; set; } = new();
    public bool IsValid { get; set; }
}
```

---

## Workflow Processing

### State Management & Business Rules

```csharp
// Domain/Workflows/IncidentWorkflowService.cs
public class IncidentWorkflowService : IIncidentWorkflowService
{
    private readonly ILogger<IncidentWorkflowService> _logger;
    private readonly IAuditService _auditService;
    private readonly IIncidentRepository _incidentRepository;

    // Valid state transitions
    private readonly Dictionary<IncidentStatus, List<IncidentStatus>> _validTransitions = new()
    {
        [IncidentStatus.Draft] = new() { IncidentStatus.Open },
        [IncidentStatus.Open] = new() { IncidentStatus.InProgress, IncidentStatus.Resolved },
        [IncidentStatus.InProgress] = new() { IncidentStatus.Pending, IncidentStatus.Resolved },
        [IncidentStatus.Pending] = new() { IncidentStatus.InProgress, IncidentStatus.Resolved },
        [IncidentStatus.Resolved] = new() { IncidentStatus.Closed, IncidentStatus.InProgress },
        [IncidentStatus.Closed] = new() { IncidentStatus.InProgress } // Reopening
    };

    public async Task<WorkflowResult> TransitionStatusAsync(string incidentId, IncidentStatus newStatus, string userId, string reason = "")
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            return WorkflowResult.Failure("Incident not found");
        }

        // Validate transition
        if (!IsValidTransition(incident.Status, newStatus))
        {
            return WorkflowResult.Failure($"Invalid transition from {incident.Status} to {newStatus}");
        }

        // Apply business rules
        var businessRuleResult = await ApplyBusinessRules(incident, newStatus);
        if (!businessRuleResult.IsValid)
        {
            return WorkflowResult.Failure(businessRuleResult.ErrorMessage);
        }

        // Perform transition
        var oldStatus = incident.Status;
        incident.Status = newStatus;
        incident.LastModifiedBy = userId;
        incident.LastModifiedDate = DateTimeOffset.UtcNow;

        await _incidentRepository.UpdateAsync(incident);

        // Log audit trail
        await _auditService.LogStatusChangeAsync(incidentId, oldStatus, newStatus, userId, reason);

        _logger.LogInformation("Incident {IncidentId} status changed from {OldStatus} to {NewStatus} by {UserId}", 
            incidentId, oldStatus, newStatus, userId);

        return WorkflowResult.Success($"Status changed to {newStatus}");
    }

    public async Task<PromotionResult> PromoteToMajorIncidentAsync(string incidentId, string userId)
    {
        var incident = await _incidentRepository.GetByIdAsync(incidentId);
        if (incident == null)
        {
            return PromotionResult.Failure("Incident not found");
        }

        if (incident.Type != IncidentType.PreIncident)
        {
            return PromotionResult.Failure("Only Pre-Incidents can be promoted to Major Incidents");
        }

        // Create promotion audit record
        await _auditService.LogPromotionAsync(incidentId, IncidentType.PreIncident, IncidentType.MajorIncident, userId);

        // Preserve existing data and add Major Incident requirements
        incident.Type = IncidentType.MajorIncident;
        incident.PromotedDate = DateTimeOffset.UtcNow;
        incident.PromotedBy = userId;
        incident.LastModifiedBy = userId;
        incident.LastModifiedAt = DateTimeOffset.UtcNow;
        
        // Major Incident fields become visible but retain existing values if present
        // No need to initialize - UI will handle field visibility based on Type

        await _incidentRepository.UpdateAsync(incident);

        _logger.LogInformation("Pre-Incident {IncidentId} promoted to Major Incident by {UserId}", incidentId, userId);

        return PromotionResult.Success("Successfully promoted to Major Incident");
    }

    private bool IsValidTransition(IncidentStatus currentStatus, IncidentStatus newStatus)
    {
        return _validTransitions.ContainsKey(currentStatus) && 
               _validTransitions[currentStatus].Contains(newStatus);
    }

    private async Task<BusinessRuleResult> ApplyBusinessRules(Incident incident, IncidentStatus newStatus)
    {
        // Rule: Cannot close incident without resolution details
        if (newStatus == IncidentStatus.Closed && string.IsNullOrEmpty(incident.ResolutionDetails))
        {
            return BusinessRuleResult.Invalid("Resolution details required before closing incident");
        }

        // Rule: Major incidents require NOI before closing
        if (newStatus == IncidentStatus.Closed && 
            incident.Type == IncidentType.MajorIncident && 
            !incident.NOISent)
        {
            return BusinessRuleResult.Invalid("NOI must be sent before closing Major Incident");
        }

        return BusinessRuleResult.Valid();
    }

    private List<ChecklistItem> GetDefaultChecklistItems()
    {
        return new List<ChecklistItem>
        {
            new() { Description = "Incident Commander assigned", IsRequired = true },
            new() { Description = "Technical SME engaged", IsRequired = true },
            new() { Description = "Business stakeholders notified", IsRequired = true },
            new() { Description = "Communication plan activated", IsRequired = false },
            new() { Description = "Escalation procedures followed", IsRequired = false }
        };
    }
}

// Supporting classes
public enum IncidentStatus
{
    Draft,
    Open,
    InProgress,
    Pending,
    Resolved,
    Closed
}

public enum IncidentType
{
    PreIncident,
    MajorIncident
}

public class WorkflowResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    
    public static WorkflowResult Success(string message) => new() { IsSuccess = true, Message = message };
    public static WorkflowResult Failure(string message) => new() { IsSuccess = false, Message = message };
}

public class PromotionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    
    public static PromotionResult Success(string message) => new() { IsSuccess = true, Message = message };
    public static PromotionResult Failure(string message) => new() { IsSuccess = false, Message = message };
}

public class BusinessRuleResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
    
    public static BusinessRuleResult Valid() => new() { IsValid = true };
    public static BusinessRuleResult Invalid(string error) => new() { IsValid = false, ErrorMessage = error };
}
```

---

## Data Validation & Business Rules

### Comprehensive Validation Framework

```csharp
// Application/Validation/IncidentValidationService.cs
public class IncidentValidationService : IIncidentValidationService
{
    private readonly ILogger<IncidentValidationService> _logger;

    public ValidationResult ValidateIncident(Incident incident)
    {
        var errors = new List<ValidationError>();

        // Basic field validation
        ValidateBasicFields(incident, errors);
        
        // Business rule validation
        ValidateBusinessRules(incident, errors);
        
        // Type-specific validation
        if (incident.Type == IncidentType.MajorIncident)
        {
            ValidateMajorIncidentFields(incident, errors);
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    private void ValidateBasicFields(Incident incident, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(incident.Title))
            errors.Add(new ValidationError("Title", "Title is required"));
        else if (incident.Title.Length < 5)
            errors.Add(new ValidationError("Title", "Title must be at least 5 characters"));

        if (string.IsNullOrWhiteSpace(incident.IncidentNumber))
            errors.Add(new ValidationError("IncidentNumber", "Incident number is required"));

        if (incident.Urgency < 1 || incident.Urgency > 3)
            errors.Add(new ValidationError("Urgency", "Urgency must be between 1 (High) and 3 (Low)"));

        if (incident.ImpactedUsers.HasValue && (incident.ImpactedUsers < 1 || incident.ImpactedUsers > 5000))
            errors.Add(new ValidationError("ImpactedUsers", "Impacted users must be between 1 and 5,000"));
    }

    private void ValidateBusinessRules(Incident incident, List<ValidationError> errors)
    {
        // Rule: High priority incidents must have impacted users specified
        if (incident.Priority == "P1" && !incident.ImpactedUsers.HasValue)
            errors.Add(new ValidationError("ImpactedUsers", "P1 incidents must specify impacted users"));

        // Rule: Security incidents require special handling
        if (incident.IsSecurityIncident && string.IsNullOrEmpty(incident.SecurityContact))
            errors.Add(new ValidationError("SecurityContact", "Security incidents require a security contact"));

        // Rule: SME fields required when SME contacted
        if (incident.SMEContacted && string.IsNullOrEmpty(incident.SMEName))
            errors.Add(new ValidationError("SMEName", "SME name required when SME contacted"));
    }

    private void ValidateMajorIncidentFields(Incident incident, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(incident.BusinessImpact))
            errors.Add(new ValidationError("BusinessImpact", "Business impact is required for Major Incidents"));

        if (incident.NOIRecipients?.Any() != true)
            errors.Add(new ValidationError("NOIRecipients", "NOI recipients are required for Major Incidents"));

        // Validate email addresses
        if (incident.NOIRecipients?.Any() == true)
        {
            foreach (var email in incident.NOIRecipients)
            {
                if (!IsValidEmail(email))
                    errors.Add(new ValidationError("NOIRecipients", $"Invalid email address: {email}"));
            }
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class ValidationError
{
    public string Field { get; set; }
    public string Message { get; set; }

    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; }

    public ValidationResult(bool isValid, List<ValidationError> errors)
    {
        IsValid = isValid;
        Errors = errors ?? new List<ValidationError>();
    }
}
```

---

## Integration Patterns

### Neurons API Integration

```csharp
// Infrastructure/ExternalServices/NeuronsIntegrationService.cs
public class NeuronsIntegrationService : INeuronsIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NeuronsIntegrationService> _logger;
    private readonly ITokenProviderService _tokenProvider;
    private readonly NeuronsOptions _options;

    public NeuronsIntegrationService(
        HttpClient httpClient,
        ILogger<NeuronsIntegrationService> logger,
        ITokenProviderService tokenProvider,
        IOptions<NeuronsOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tokenProvider = tokenProvider;
        _options = options.Value;
    }

    public async Task<IncidentData> FetchIncidentDataAsync(string incidentNumber)
    {
        try
        {
            var token = await _tokenProvider.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/incidents/{incidentNumber}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var incidentData = JsonSerializer.Deserialize<IncidentData>(json);
                
                _logger.LogInformation("Successfully fetched incident data for {IncidentNumber}", incidentNumber);
                return incidentData;
            }
            else
            {
                _logger.LogWarning("Failed to fetch incident data for {IncidentNumber}: {StatusCode}", 
                    incidentNumber, response.StatusCode);
                throw new ExternalServiceException($"Failed to fetch incident data: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching incident data for {IncidentNumber}", incidentNumber);
            throw;
        }
    }

    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            var token = await _tokenProvider.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate Neurons connection");
            return false;
        }
    }
}

public class NeuronsOptions
{
    public string BaseUrl { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public string ApiVersion { get; set; } = "v1";
}

public class IncidentData
{
    public string IncidentNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartTime { get; set; }
    public string Priority { get; set; }
    public List<string> AffectedSystems { get; set; }
    public int? ImpactedUsers { get; set; }
    public string Status { get; set; }
}
```

---

## Summary

This business logic documentation provides the essential implementation details for Operation Prime's core systems:

- **Priority Calculation**: 4-layer architecture with validation, matrix computation, override logic, and audit trail
- **NOI Generation**: Builder pattern with .docx template processing and email distribution
- **Email Distribution**: Team-based suggestions with validation and default lists
- **Workflow Processing**: State management with business rules and audit logging
- **Data Validation**: Comprehensive validation framework with business rule enforcement
- **Integration Patterns**: Neurons API integration with token-based authentication

These implementations ensure robust, maintainable, and auditable business logic that supports the incident management workflow requirements.

**Next**: Return to [README](./README-DOCS.md) for navigation or see [Reference](./QUICK-REFERENCE.md) for quick lookups.
