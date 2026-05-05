using TaskPlanner.Infrastructure.Security;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Verify_ShouldReturnTrue_ForMatchingPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Demo123!");

        Assert.NotEqual("Demo123!", hash);
        Assert.True(hasher.Verify("Demo123!", hash));
    }

    [Fact]
    public void Verify_ShouldReturnFalse_ForWrongPassword()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Demo123!");

        Assert.False(hasher.Verify("Wrong123!", hash));
    }
}
