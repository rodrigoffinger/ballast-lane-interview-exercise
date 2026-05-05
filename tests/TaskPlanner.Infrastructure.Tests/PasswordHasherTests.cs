using FluentAssertions;
using TaskPlanner.Infrastructure.Security;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Verify_ShouldReturnTrue_ForMatchingPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Demo123!");

        hash.Should().NotBe("Demo123!");
        hasher.Verify("Demo123!", hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForWrongPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Demo123!");

        hasher.Verify("Wrong123!", hash).Should().BeFalse();
    }
}

