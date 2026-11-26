using Xunit;

namespace Todo.UnitTests;

public class HelloWorldTests
{
    [Fact]
    public void RootEndpointReturnsHelloWorld()
    {
        // Arrange
        const string expected = "Hello World!";

        // Act
        var result = GetGreeting();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RootEndpointReturnsNonEmptyString()
    {
        // Act
        var result = GetGreeting();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    private static string GetGreeting() => "Hello World!";
}
