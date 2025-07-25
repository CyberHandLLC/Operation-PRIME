using OperationPrime.Domain.Entities;
using OperationPrime.Domain.Enums;

namespace Domain.Tests;

/// <summary>
/// Tests for Incident entity hierarchy to ensure proper business logic and validation.
/// </summary>
public class IncidentEntityTests
{
    [Fact]
    public void PreIncident_ShouldInheritFromIncident()
    {
        // Arrange & Act
        var preIncident = new PreIncident();

        // Assert
        Assert.IsAssignableFrom<Incident>(preIncident);
        Assert.IsAssignableFrom<BaseEntity>(preIncident);
    }

    [Fact]
    public void MajorIncident_ShouldInheritFromIncident()
    {
        // Arrange & Act
        var majorIncident = new MajorIncident();

        // Assert
        Assert.IsAssignableFrom<Incident>(majorIncident);
        Assert.IsAssignableFrom<BaseEntity>(majorIncident);
    }

    [Fact]
    public void Incident_CalculatePriority_ShouldUsePriorityMatrix()
    {
        // Arrange
        var incident = new PreIncident
        {
            Urgency = UrgencyLevel.High,
            Impact = ImpactLevel.Medium
        };

        // Act
        incident.CalculatePriority();

        // Assert
        Assert.Equal(Priority.High, incident.Priority);
    }

    [Fact]
    public void Incident_GetPriorityRationale_ShouldReturnExplanation()
    {
        // Arrange
        var incident = new MajorIncident
        {
            Urgency = UrgencyLevel.Medium,
            Impact = ImpactLevel.Low
        };

        // Act
        var rationale = incident.GetPriorityRationale();

        // Assert
        Assert.Contains("Priority Low", rationale);
        Assert.Contains("Medium urgency", rationale);
        Assert.Contains("Low impact", rationale);
    }

    [Fact]
    public void BaseEntity_ShouldHaveAuditProperties()
    {
        // Arrange & Act
        var incident = new PreIncident();

        // Assert
        Assert.True(incident.Id > 0); // Should have default ID
        Assert.True(incident.CreatedAt <= DateTime.UtcNow);
        Assert.True(incident.UpdatedAt <= DateTime.UtcNow);
        Assert.NotNull(incident.CreatedBy);
        Assert.NotNull(incident.UpdatedBy);
    }

    [Fact]
    public void BaseEntity_UpdateAuditFields_ShouldUpdateTimestampAndUser()
    {
        // Arrange
        var incident = new PreIncident();
        var originalUpdatedAt = incident.UpdatedAt;
        var testUser = "TestUser";

        // Act
        Thread.Sleep(10); // Ensure time difference
        incident.UpdateAuditFields(testUser);

        // Assert
        Assert.True(incident.UpdatedAt > originalUpdatedAt);
        Assert.Equal(testUser, incident.UpdatedBy);
    }

    [Fact]
    public void PreIncident_ShouldHaveSpecificProperties()
    {
        // Arrange & Act
        var preIncident = new PreIncident
        {
            IdentifiedBy = "John Doe",
            PotentialImpact = "System slowdown",
            PreventiveActions = "Monitor CPU usage",
            RiskAssessment = "Medium risk"
        };

        // Assert
        Assert.Equal("John Doe", preIncident.IdentifiedBy);
        Assert.Equal("System slowdown", preIncident.PotentialImpact);
        Assert.Equal("Monitor CPU usage", preIncident.PreventiveActions);
        Assert.Equal("Medium risk", preIncident.RiskAssessment);
    }

    [Fact]
    public void MajorIncident_ShouldHaveSpecificProperties()
    {
        // Arrange & Act
        var majorIncident = new MajorIncident
        {
            IncidentCommander = "Jane Smith",
            BusinessImpact = "Service unavailable",
            AffectedUsersCount = 1000,
            PIRRequired = true
        };

        // Assert
        Assert.Equal("Jane Smith", majorIncident.IncidentCommander);
        Assert.Equal("Service unavailable", majorIncident.BusinessImpact);
        Assert.Equal(1000, majorIncident.AffectedUsersCount);
        Assert.True(majorIncident.PIRRequired);
    }

    [Theory]
    [InlineData(IncidentStatus.New)]
    [InlineData(IncidentStatus.InProgress)]
    [InlineData(IncidentStatus.Resolved)]
    [InlineData(IncidentStatus.Closed)]
    public void Incident_ShouldAcceptAllValidStatuses(IncidentStatus status)
    {
        // Arrange & Act
        var incident = new PreIncident
        {
            Status = status
        };

        // Assert
        Assert.Equal(status, incident.Status);
    }

    [Theory]
    [InlineData(Priority.Low)]
    [InlineData(Priority.Medium)]
    [InlineData(Priority.High)]
    public void Incident_ShouldAcceptAllValidPriorities(Priority priority)
    {
        // Arrange & Act
        var incident = new MajorIncident
        {
            Priority = priority
        };

        // Assert
        Assert.Equal(priority, incident.Priority);
    }
}
