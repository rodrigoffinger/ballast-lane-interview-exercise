using FluentAssertions;

namespace TaskPlanner.Api.Tests;

public sealed class ApiAssemblyTests
{
    [Fact]
    public void ApiAssembly_ShouldBeLoadable()
    {
        typeof(ApiAssemblyTests).Assembly.Should().NotBeNull();
    }
}

