using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TaskPlanner.Api.Tests;

public sealed class TaskEndpointsTests
{
    [Fact]
    public async Task GetTasks_ShouldReturnUnauthorized_WithoutToken()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TaskCrud_ShouldWork_ForAuthenticatedUser()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        await AuthenticateAsDemoUserAsync(client);

        var createResponse = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Write API tests",
            description = "Cover the authenticated task flow.",
            status = 0,
            dueDate = DateTimeOffset.UtcNow.AddDays(2)
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdBody = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TaskResponse>>();
        var createdTask = createdBody!.Data!;
        createdTask.Title.Should().Be("Write API tests");

        var listResponse = await client.GetAsync("/api/tasks");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var listBody = await listResponse.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TaskResponse>>>();
        listBody!.Data.Should().Contain(task => task.Id == createdTask.Id);

        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{createdTask.Id}", new
        {
            title = "Finish API tests",
            description = "Cover the authenticated task flow.",
            status = 2,
            dueDate = DateTimeOffset.UtcNow.AddDays(3)
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedBody = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<TaskResponse>>();
        updatedBody!.Data!.Title.Should().Be("Finish API tests");
        updatedBody.Data.Status.Should().Be(2);

        var deleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeletedResponse = await client.GetAsync($"/api/tasks/{createdTask.Id}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsMissing()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        await AuthenticateAsDemoUserAsync(client);

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "",
            description = "No title should fail.",
            status = 0,
            dueDate = DateTimeOffset.UtcNow.AddDays(1)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Errors.Should().Contain(error => error.Code == "TitleRequired");
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenStatusIsInvalid()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        await AuthenticateAsDemoUserAsync(client);

        var response = await client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Invalid status",
            description = "Status must map to the domain enum.",
            status = 99,
            dueDate = DateTimeOffset.UtcNow.AddDays(1)
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Errors.Should().Contain(error => error.Code == "InvalidTaskStatus");
    }

    [Fact]
    public async Task TaskOperations_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        await AuthenticateAsDemoUserAsync(client);
        var missingTaskId = Guid.NewGuid();

        var getResponse = await client.GetAsync($"/api/tasks/{missingTaskId}");
        var updateResponse = await client.PutAsJsonAsync($"/api/tasks/{missingTaskId}", new
        {
            title = "Missing",
            description = "This task does not exist.",
            status = 0,
            dueDate = DateTimeOffset.UtcNow.AddDays(1)
        });
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{missingTaskId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TaskOperations_ShouldReturnNotFound_WhenTaskBelongsToAnotherUser()
    {
        await using var factory = new TestApiFactory();
        var ownerClient = factory.CreateClient();
        var otherClient = factory.CreateClient();
        await RegisterAndAuthenticateAsync(ownerClient, $"owner-{Guid.NewGuid():N}@example.com");
        await RegisterAndAuthenticateAsync(otherClient, $"other-{Guid.NewGuid():N}@example.com");

        var createResponse = await ownerClient.PostAsJsonAsync("/api/tasks", new
        {
            title = "Owner only",
            description = "This task belongs to a different user.",
            status = 0,
            dueDate = DateTimeOffset.UtcNow.AddDays(1)
        });
        var created = await createResponse.Content.ReadFromJsonAsync<ApiResponse<TaskResponse>>();
        var taskId = created!.Data!.Id;

        var getResponse = await otherClient.GetAsync($"/api/tasks/{taskId}");
        var updateResponse = await otherClient.PutAsJsonAsync($"/api/tasks/{taskId}", new
        {
            title = "Should not update",
            description = "Cross-user update must not be allowed.",
            status = 2,
            dueDate = DateTimeOffset.UtcNow.AddDays(2)
        });
        var deleteResponse = await otherClient.DeleteAsync($"/api/tasks/{taskId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task AuthenticateAsDemoUserAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@ballastlane.local",
            password = "Demo123!"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Data!.AccessToken);
    }

    private static async Task RegisterAndAuthenticateAsync(HttpClient client, string email)
    {
        const string password = "Demo123!";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test User",
            email,
            password
        });
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password
        });
        loginResponse.EnsureSuccessStatusCode();
        var body = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Data!.AccessToken);
    }
}
