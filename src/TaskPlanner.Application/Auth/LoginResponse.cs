namespace TaskPlanner.Application.Auth;

public sealed record LoginResponse(string AccessToken, Guid UserId, string Name, string Email);

