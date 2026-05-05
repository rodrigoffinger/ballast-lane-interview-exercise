using Microsoft.Data.Sqlite;
using TaskPlanner.Infrastructure.Persistence;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class SqliteDatabaseInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ShouldCreateUsersAndTasksTables()
    {
        using var database = TestDatabase.Create();
        var initializer = new SqliteDatabaseInitializer(database.ConnectionFactory);

        await initializer.InitializeAsync();

        await using var connection = database.ConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        var tableNames = new List<string>();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name;";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tableNames.Add(reader.GetString(0));
        }

        Assert.Contains("tasks", tableNames);
        Assert.Contains("users", tableNames);
    }
}

internal sealed class TestDatabase : IDisposable
{
    private readonly string _databasePath;

    private TestDatabase(string databasePath)
    {
        _databasePath = databasePath;
        ConnectionFactory = new SqliteConnectionFactory($"Data Source={databasePath};Pooling=False");
    }

    public SqliteConnectionFactory ConnectionFactory { get; }

    public static TestDatabase Create()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"taskplanner-{Guid.NewGuid():N}.db");
        return new TestDatabase(databasePath);
    }

    public void Dispose()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
