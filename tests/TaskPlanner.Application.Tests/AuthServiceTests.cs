using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Auth;
using TaskPlanner.Application.Common;
using TaskPlanner.Domain.Users;

namespace TaskPlanner.Application.Tests;

public sealed class AuthServiceTests
{
    private readonly FakePasswordHasher _passwordHasher = new();
    private readonly FakeTokenService _tokenService = new();
    private readonly FakeUserRepository _users = new();

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        var service = CreateService();
        await _users.AddAsync(new User(Guid.NewGuid(), "Demo User", "demo@ballastlane.local", "hash", DateTimeOffset.UtcNow));

        var result = await service.RegisterAsync(new RegisterUserRequest("Other User", "demo@ballastlane.local", "Demo123!"));

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(result.Errors, error => error.Code == "EmailAlreadyExists");
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenRequestIsInvalid()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(new RegisterUserRequest("", "not-an-email", "123"));

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Code == "NameRequired");
        Assert.Contains(result.Errors, error => error.Code == "InvalidEmail");
        Assert.Contains(result.Errors, error => error.Code == "WeakPassword");
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WithHashedPassword()
    {
        var service = CreateService();

        var result = await service.RegisterAsync(new RegisterUserRequest("Demo User", "demo@ballastlane.local", "Demo123!"));

        Assert.Equal(ResultStatus.Success, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal("demo@ballastlane.local", result.Value!.Email);
        var savedUser = await _users.FindByEmailAsync("demo@ballastlane.local");
        Assert.NotNull(savedUser);
        Assert.Equal("hashed:Demo123!", savedUser!.PasswordHash);
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenCredentialsAreInvalid()
    {
        var service = CreateService();

        var result = await service.LoginAsync(new LoginRequest("missing@ballastlane.local", "Wrong123!"));

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.Contains(result.Errors, error => error.Code == "InvalidCredentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var service = CreateService();
        await service.RegisterAsync(new RegisterUserRequest("Demo User", "demo@ballastlane.local", "Demo123!"));

        var result = await service.LoginAsync(new LoginRequest("demo@ballastlane.local", "Demo123!"));

        Assert.Equal(ResultStatus.Success, result.Status);
        Assert.NotNull(result.Value);
        Assert.Equal("token-for-demo@ballastlane.local", result.Value!.AccessToken);
    }

    private AuthService CreateService()
    {
        return new AuthService(_users, _passwordHasher, _tokenService);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Any(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.SingleOrDefault(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.SingleOrDefault(user => user.Id == id));
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string CreateToken(User user)
        {
            return $"token-for-{user.Email}";
        }
    }

}
