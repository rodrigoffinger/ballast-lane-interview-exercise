using TaskPlanner.Application.Abstractions;

namespace TaskPlanner.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

