using FluentAssertions;

namespace TaskPlanner.Application.Tests;

public sealed class ApplicationAssemblyTests
{
    [Fact]
    public void ApplicationAssembly_ShouldBeLoadable()
    {
        typeof(ApplicationAssemblyTests).Assembly.Should().NotBeNull();
    }
}

