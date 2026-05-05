namespace TaskPlanner.Infrastructure.Security;

public sealed record JwtOptions(
    string Issuer,
    string Audience,
    string SigningKey,
    int ExpirationMinutes);

