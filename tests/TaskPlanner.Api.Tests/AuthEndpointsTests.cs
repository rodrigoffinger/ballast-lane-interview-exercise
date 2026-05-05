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

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.Equal(email, body.Data!.Email);
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

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.False(body!.Success);
        Assert.Contains(body.Errors, error => error.Code == "EmailAlreadyExists");
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "",
            email = "not-an-email",
            password = "123"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.False(body!.Success);
        Assert.Contains(body.Errors, error => error.Code == "NameRequired");
        Assert.Contains(body.Errors, error => error.Code == "InvalidEmail");
        Assert.Contains(body.Errors, error => error.Code == "WeakPassword");
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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        Assert.True(body!.Success);
        Assert.False(string.IsNullOrWhiteSpace(body.Data!.AccessToken));
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsWrong()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@ballastlane.local",
            password = "Wrong123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.False(body!.Success);
        Assert.Contains(body.Errors, error => error.Code == "InvalidCredentials");
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "missing@ballastlane.local",
            password = "Demo123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.False(body!.Success);
        Assert.Contains(body.Errors, error => error.Code == "InvalidCredentials");
    }

    [Fact]
    public async Task Me_ShouldReturnUnauthorized_WithoutToken()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
