using TaskPlanner.Domain.Users;

namespace TaskPlanner.Application.Abstractions;

public interface ITokenService
{
    string CreateToken(User user);
}

