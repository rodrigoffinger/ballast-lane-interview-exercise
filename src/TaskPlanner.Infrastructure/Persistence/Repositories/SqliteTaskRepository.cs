using Microsoft.Data.Sqlite;
using TaskPlanner.Application.Abstractions;
using TaskPlanner.Domain.Tasks;
using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Infrastructure.Persistence.Repositories;

public sealed class SqliteTaskRepository : ITaskRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteTaskRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO tasks (id, user_id, title, description, status, due_date, created_at, updated_at)
            VALUES ($id, $user_id, $title, $description, $status, $due_date, $created_at, $updated_at);
            """;
        AddTaskParameters(command, task);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, user_id, title, description, status, due_date, created_at, updated_at
            FROM tasks
            WHERE user_id = $user_id
            ORDER BY due_date ASC, created_at ASC;
            """;
        command.Parameters.AddWithValue("$user_id", userId.ToString());

        var tasks = new List<TaskItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tasks.Add(MapTask(reader));
        }

        return tasks;
    }

    public async Task<TaskItem?> GetByIdForUserAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, user_id, title, description, status, due_date, created_at, updated_at
            FROM tasks
            WHERE id = $id AND user_id = $user_id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$id", taskId.ToString());
        command.Parameters.AddWithValue("$user_id", userId.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapTask(reader) : null;
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE tasks
            SET title = $title,
                description = $description,
                status = $status,
                due_date = $due_date,
                updated_at = $updated_at
            WHERE id = $id AND user_id = $user_id;
            """;
        AddTaskParameters(command, task);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM tasks WHERE id = $id AND user_id = $user_id;";
        command.Parameters.AddWithValue("$id", task.Id.ToString());
        command.Parameters.AddWithValue("$user_id", task.UserId.ToString());

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddTaskParameters(SqliteCommand command, TaskItem task)
    {
        command.Parameters.AddWithValue("$id", task.Id.ToString());
        command.Parameters.AddWithValue("$user_id", task.UserId.ToString());
        command.Parameters.AddWithValue("$title", task.Title);
        command.Parameters.AddWithValue("$description", task.Description);
        command.Parameters.AddWithValue("$status", (int)task.Status);
        command.Parameters.AddWithValue("$due_date", task.DueDate.ToString("O"));
        command.Parameters.AddWithValue("$created_at", task.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$updated_at", task.UpdatedAt.ToString("O"));
    }

    private static TaskItem MapTask(SqliteDataReader reader)
    {
        return new TaskItem(
            Guid.Parse(reader.GetString(0)),
            Guid.Parse(reader.GetString(1)),
            reader.GetString(2),
            reader.GetString(3),
            (DomainTaskStatus)reader.GetInt32(4),
            DateTimeOffset.Parse(reader.GetString(5)),
            DateTimeOffset.Parse(reader.GetString(6)),
            DateTimeOffset.Parse(reader.GetString(7)));
    }
}
