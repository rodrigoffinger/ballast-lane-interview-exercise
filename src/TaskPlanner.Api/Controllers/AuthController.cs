using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPlanner.Api.Contracts;
using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Auth;

namespace TaskPlanner.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserRepository _users;

    public AuthController(AuthService authService, IUserRepository users)
    {
        _authService = authService;
        _users = users;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return CreatedFromResult(nameof(Me), null!, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse<object>.Fail([new ApiError("UserNotFound", "The authenticated user was not found.")]));
        }

        return Ok(ApiResponse<RegisteredUserResponse>.Ok(new RegisteredUserResponse(user.Id, user.Name, user.Email)));
    }
}

