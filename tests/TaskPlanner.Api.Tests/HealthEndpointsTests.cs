using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TaskPlanner.Api.Tests;

public sealed class HealthEndpointsTests
{
    [Fact]
    public async Task Health_ShouldExposePublicAndSecureEndpoints()
    {
        await using var factory = new TestApiFactory();
        var client = factory.CreateClient();

        var publicResponse = await client.GetAsync("/api/health");

        publicResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secureWithoutToken = await client.GetAsync("/api/health/secure");
        secureWithoutToken.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@ballastlane.local",
            password = "Demo123!"
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody!.Data!.AccessToken);

        var secureWithToken = await client.GetAsync("/api/health/secure");
        secureWithToken.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

