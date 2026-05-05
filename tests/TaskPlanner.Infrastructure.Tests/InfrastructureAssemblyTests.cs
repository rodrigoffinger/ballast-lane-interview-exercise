using FluentAssertions;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class InfrastructureAssemblyTests
{
    [Fact]
    public void InfrastructureAssembly_ShouldBeLoadable()
    {
        typeof(InfrastructureAssemblyTests).Assembly.Should().NotBeNull();
    }
}

