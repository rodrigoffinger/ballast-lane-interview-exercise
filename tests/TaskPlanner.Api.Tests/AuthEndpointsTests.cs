using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace TaskPlanner.Api.Tests;

public sealed class AuthEndpointsTests
{
    [Fact]
    public async Task Register_ShouldReturnCreated_WithResponseEnvelope()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        var email = $"jane-{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Jane Reviewer",
            email,
            password = "Demo123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Data!.Email.Should().Be(email);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();
        var email = $"jane-{Guid.NewGuid():N}@example.com";
        var request = new
        {
            name = "Jane Reviewer",
            email,
            password = "Demo123!"
        };
        await client.PostAsJsonAsync("/api/auth/register", request);

        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body!.Success.Should().BeFalse();
        body.Errors.Should().Contain(error => error.Code == "EmailAlreadyExists");
    }

    [Fact]
    public async Task Login_ShouldReturnToken_ForSeededDemoUser()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@ballastlane.local",
            password = "Demo123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        body!.Success.Should().BeTrue();
        body.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }
}
