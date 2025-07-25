using NetArchTest.Rules;
using System.Reflection;

namespace Domain.Tests;

/// <summary>
/// Tests to enforce Clean Architecture boundaries and dependencies.
/// Ensures layers don't violate dependency rules.
/// </summary>
public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(OperationPrime.Domain.Entities.BaseEntity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(OperationPrime.Application.Interfaces.INavigationService).Assembly;
    private static readonly Assembly InfrastructureAssembly = DomainAssembly; // Same assembly for now
    private static readonly Assembly PresentationAssembly = DomainAssembly; // Same assembly for now

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Application")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Domain layer should not depend on Application layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Infrastructure")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Domain layer should not depend on Infrastructure layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Presentation")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Domain layer should not depend on Presentation layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Application")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Infrastructure")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Application layer should not depend on Infrastructure layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Application")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Presentation")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Application layer should not depend on Presentation layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Infrastructure_ShouldNotHaveDependencyOn_PresentationLayer()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Infrastructure")
            .Should()
            .NotHaveDependencyOn("OperationPrime.Presentation")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Infrastructure layer should not depend on Presentation layer. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Entities_ShouldBeInDomainNamespace()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain.Entities")
            .Should()
            .ResideInNamespace("OperationPrime.Domain.Entities")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"All entities should be in Domain.Entities namespace. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ValueObjects_ShouldBeInDomainNamespace()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain.ValueObjects")
            .Should()
            .ResideInNamespace("OperationPrime.Domain.ValueObjects")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"All value objects should be in Domain.ValueObjects namespace. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void Interfaces_ShouldBeInApplicationNamespace()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Application.Interfaces")
            .Should()
            .ResideInNamespace("OperationPrime.Application.Interfaces")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"All application interfaces should be in Application.Interfaces namespace. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void ViewModels_ShouldBeInPresentationNamespace()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Presentation.ViewModels")
            .Should()
            .ResideInNamespace("OperationPrime.Presentation.ViewModels")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"All ViewModels should be in Presentation.ViewModels namespace. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }

    [Fact]
    public void DomainEntities_ShouldNotDependOnExternalFrameworks()
    {
        // Arrange & Act
        var result = Types.InNamespace("OperationPrime.Domain.Entities")
            .Should()
            .NotHaveDependencyOn("Microsoft.UI.Xaml")
            .And()
            .NotHaveDependencyOn("CommunityToolkit.Mvvm")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, 
            $"Domain entities should not depend on UI or MVVM frameworks. Violations: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
    }
}
