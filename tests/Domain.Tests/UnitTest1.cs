using OperationPrime.Domain.Enums;
using OperationPrime.Domain.ValueObjects;

namespace Domain.Tests;

/// <summary>
/// Tests for PriorityMatrix value object to ensure correct priority calculations.
/// </summary>
public class PriorityMatrixTests
{
    [Theory]
    [InlineData(UrgencyLevel.High, ImpactLevel.High, Priority.High)]
    [InlineData(UrgencyLevel.High, ImpactLevel.Medium, Priority.High)]
    [InlineData(UrgencyLevel.Medium, ImpactLevel.High, Priority.High)]
    [InlineData(UrgencyLevel.Medium, ImpactLevel.Medium, Priority.Medium)]
    [InlineData(UrgencyLevel.Low, ImpactLevel.High, Priority.Medium)]
    [InlineData(UrgencyLevel.High, ImpactLevel.Low, Priority.Medium)]
    [InlineData(UrgencyLevel.Low, ImpactLevel.Medium, Priority.Low)]
    [InlineData(UrgencyLevel.Medium, ImpactLevel.Low, Priority.Low)]
    [InlineData(UrgencyLevel.Low, ImpactLevel.Low, Priority.Low)]
    public void CalculatePriority_ShouldReturnCorrectPriority_ForGivenUrgencyAndImpact(
        UrgencyLevel urgency, ImpactLevel impact, Priority expectedPriority)
    {
        // Arrange & Act
        var priorityMatrix = PriorityMatrix.Create(urgency, impact);

        // Assert
        Assert.Equal(expectedPriority, priorityMatrix.CalculatedPriority);
        Assert.Equal(urgency, priorityMatrix.Urgency);
        Assert.Equal(impact, priorityMatrix.Impact);
    }

    [Fact]
    public void Constructor_ShouldCalculatePriorityCorrectly()
    {
        // Arrange
        var urgency = UrgencyLevel.High;
        var impact = ImpactLevel.Medium;

        // Act
        var priorityMatrix = new PriorityMatrix(urgency, impact);

        // Assert
        Assert.Equal(Priority.High, priorityMatrix.CalculatedPriority);
        Assert.Equal(urgency, priorityMatrix.Urgency);
        Assert.Equal(impact, priorityMatrix.Impact);
    }

    [Fact]
    public void GetCalculationRationale_ShouldReturnDescriptiveMessage()
    {
        // Arrange
        var priorityMatrix = PriorityMatrix.Create(UrgencyLevel.High, ImpactLevel.Low);

        // Act
        var rationale = priorityMatrix.GetCalculationRationale();

        // Assert
        Assert.Contains("Priority Medium", rationale);
        Assert.Contains("High urgency", rationale);
        Assert.Contains("Low impact", rationale);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameUrgencyAndImpact()
    {
        // Arrange
        var matrix1 = PriorityMatrix.Create(UrgencyLevel.Medium, ImpactLevel.High);
        var matrix2 = PriorityMatrix.Create(UrgencyLevel.Medium, ImpactLevel.High);

        // Act & Assert
        Assert.True(matrix1.Equals(matrix2));
        Assert.True(matrix1 == matrix2);
        Assert.False(matrix1 != matrix2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentUrgencyOrImpact()
    {
        // Arrange
        var matrix1 = PriorityMatrix.Create(UrgencyLevel.High, ImpactLevel.Medium);
        var matrix2 = PriorityMatrix.Create(UrgencyLevel.Medium, ImpactLevel.High);

        // Act & Assert
        Assert.False(matrix1.Equals(matrix2));
        Assert.False(matrix1 == matrix2);
        Assert.True(matrix1 != matrix2);
    }

    [Fact]
    public void GetHashCode_ShouldBeSame_ForEqualMatrices()
    {
        // Arrange
        var matrix1 = PriorityMatrix.Create(UrgencyLevel.Low, ImpactLevel.Low);
        var matrix2 = PriorityMatrix.Create(UrgencyLevel.Low, ImpactLevel.Low);

        // Act & Assert
        Assert.Equal(matrix1.GetHashCode(), matrix2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var priorityMatrix = PriorityMatrix.Create(UrgencyLevel.High, ImpactLevel.High);

        // Act
        var result = priorityMatrix.ToString();

        // Assert
        Assert.Contains("PriorityMatrix:", result);
        Assert.Contains("High Urgency", result);
        Assert.Contains("High Impact", result);
        Assert.Contains("High Priority", result);
    }
}
