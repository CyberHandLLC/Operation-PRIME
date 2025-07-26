using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

public class CleanArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InNamespace("OperationPrime.Domain")
            .Should().NotHaveDependencyOn("OperationPrime.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful, "Domain layer should not reference Infrastructure layer");
    }
}
