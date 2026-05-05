using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskPlanner.Domain.Users;
using TaskPlanner.Infrastructure.Security;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateToken_ShouldIncludeUserClaims()
    {
        var options = new JwtOptions(
            Issuer: "TaskPlanner.Tests",
            Audience: "TaskPlanner.Tests",
            SigningKey: "This is a test signing key with enough length.",
            ExpirationMinutes: 60);
        var service = new JwtTokenService(options);
        var user = new User(Guid.NewGuid(), "Demo User", "demo@ballastlane.local", "hash", DateTimeOffset.UtcNow);

        var token = service.CreateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(claim => claim.Type == ClaimTypes.NameIdentifier && claim.Value == user.Id.ToString());
        jwt.Claims.Should().Contain(claim => claim.Type == ClaimTypes.Email && claim.Value == user.Email);
        jwt.Claims.Should().Contain(claim => claim.Type == ClaimTypes.Name && claim.Value == user.Name);
    }
}

