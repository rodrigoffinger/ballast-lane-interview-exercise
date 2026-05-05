namespace TaskPlanner.Application.Auth;

public sealed record RegisterUserRequest(string Name, string Email, string Password);

