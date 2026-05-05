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
}

