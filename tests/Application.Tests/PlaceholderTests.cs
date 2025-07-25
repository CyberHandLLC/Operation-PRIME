namespace Application.Tests;

/// <summary>
/// Simple test class to verify test infrastructure compiles.
/// Will be replaced with proper xUnit tests once package issues are resolved.
/// </summary>
public class PlaceholderTests
{
    public static void RunTests()
    {
        TestInfrastructureShouldWork();
        MathAdditionShouldWork();
        Console.WriteLine("All placeholder tests passed!");
    }

    public static void TestInfrastructureShouldWork()
    {
        // Arrange
        var expected = "Hello, Tests!";

        // Act
        var actual = "Hello, Tests!";

        // Assert
        if (expected != actual)
            throw new Exception($"Expected {expected}, but got {actual}");
    }

    public static void MathAdditionShouldWork()
    {
        // Test cases
        AssertEqual(2, 1 + 1);
        AssertEqual(5, 2 + 3);
        AssertEqual(0, -1 + 1);
    }

    private static void AssertEqual<T>(T expected, T actual)
    {
        if (!expected?.Equals(actual) ?? actual != null)
            throw new Exception($"Expected {expected}, but got {actual}");
    }
}
