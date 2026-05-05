using Microsoft.Data.Sqlite;
using TaskPlanner.Application.Abstractions;
using TaskPlanner.Domain.Users;

namespace TaskPlanner.Infrastructure.Persistence.Repositories;

public sealed class SqliteUserRepository : IUserRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteUserRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO users (id, name, email, password_hash, created_at)
            VALUES ($id, $name, $email, $password_hash, $created_at);
            """;
        command.Parameters.AddWithValue("$id", user.Id.ToString());
        command.Parameters.AddWithValue("$name", user.Name);
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$password_hash", user.PasswordHash);
        command.Parameters.AddWithValue("$created_at", user.CreatedAt.ToString("O"));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM users WHERE lower(email) = lower($email) LIMIT 1;";
        command.Parameters.AddWithValue("$email", email);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email, password_hash, created_at
            FROM users
            WHERE lower(email) = lower($email)
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, email, password_hash, created_at
            FROM users
            WHERE id = $id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
    }

    private static User MapUser(SqliteDataReader reader)
    {
        return new User(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            DateTimeOffset.Parse(reader.GetString(4)));
    }
}

