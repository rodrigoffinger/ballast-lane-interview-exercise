using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Common;
using TaskPlanner.Domain.Users;

namespace TaskPlanner.Application.Auth;

public sealed class AuthService
{
    private const int MinimumPasswordLength = 8;

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<RegisteredUserResponse>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var errors = ValidateRegistration(request);
        if (errors.Count > 0)
        {
            return Result<RegisteredUserResponse>.Failure(ResultStatus.ValidationError, errors);
        }

        var normalizedEmail = NormalizeEmail(request.Email);
        if (await _users.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return Result<RegisteredUserResponse>.Failure(
                ResultStatus.Conflict,
                [new ResultError("EmailAlreadyExists", "A user with this email already exists.")]);
        }

        var user = new User(
            Guid.NewGuid(),
            request.Name.Trim(),
            normalizedEmail,
            _passwordHasher.Hash(request.Password),
            DateTimeOffset.UtcNow);

        await _users.AddAsync(user, cancellationToken);

        return Result<RegisteredUserResponse>.Success(new RegisteredUserResponse(user.Id, user.Name, user.Email));
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _users.FindByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginResponse>.Failure(
                ResultStatus.Unauthorized,
                [new ResultError("InvalidCredentials", "The email or password is invalid.")]);
        }

        var token = _tokenService.CreateToken(user);
        return Result<LoginResponse>.Success(new LoginResponse(token, user.Id, user.Name, user.Email));
    }

    private static List<ResultError> ValidateRegistration(RegisterUserRequest request)
    {
        var errors = new List<ResultError>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add(new ResultError("NameRequired", "Name is required."));
        }

        if (!IsValidEmail(request.Email))
        {
            errors.Add(new ResultError("InvalidEmail", "A valid email is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < MinimumPasswordLength)
        {
            errors.Add(new ResultError("WeakPassword", $"Password must contain at least {MinimumPasswordLength} characters."));
        }

        return errors;
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email)
            && email.Contains('@', StringComparison.Ordinal)
            && email.Contains('.', StringComparison.Ordinal);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
